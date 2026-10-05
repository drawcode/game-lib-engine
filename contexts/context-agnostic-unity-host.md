---
name: context-agnostic-unity-host
description: The Unity reference host for the agnostic core (plan-agnostic-common-usages P3 step 1, 2026-09-26) — Engine/AgnosticHost/ implements all 12 IHost* from game-lib-bitty-base over the house facades (GameObjectHelper, AudioSystem, SystemPrefUtil, FileSystemUtil, AssetUtil, DeviceUtil) plus UnityHostDriver, which reproduces NullHost.Frame's order from Unity's Update. Carries the glTF<->Unity frame rule (mirror z; quat (-x,-y,z,w)), the handle-table rules (counter ids, reference-keyed, descendants dead the moment Destroy returns), why Spawn does not use the house pool, why 2D voices parent to AudioSystem, the name collisions to alias, and the in-Editor probe + profiler numbers (0 B/frame, ~4 us). L3 below contexts/gamedev.
metadata:
  type: design
  area: gamedev
  level: L3
  status: v0
  created: 2026-09-26
  plan: plan-agnostic-common-usages
---

# Agnostic Unity host (P3 step 1)

Code: `Engine/AgnosticHost/` in this repo, namespace `Engine.AgnosticHost`, compiled into
Assembly-CSharp. The contract it implements lives in game-lib-bitty-base (`Agnostic/Host/HostContract.cs`,
design records `context-agnostic-contract-v0` and `context-agnostic-core-p2` in that repo).

## Why here, and why the core is now auto-referenced

- The facades the adapters sit on (`GameObjectHelper`, `AudioSystem`, `SystemPrefUtil`, `FileSystemUtil`)
  all compile into **Assembly-CSharp**. An asmdef cannot reference Assembly-CSharp, so an adapter
  assembly of its own could not call them. The adapters live in game-lib-engine instead.
- For Assembly-CSharp to see the core, `Agnostic.asmdef` is now **`autoReferenced: true`**
  (was `false` in P2). `noEngineReferences: true` is unchanged, so the core still references
  `netstandard` only.
- Additive only: new files, no existing facade changed. "Facades delegate inward" (the plan's wording)
  is **not done yet** — today the adapters call the facades, not the reverse. Flip that per facade only
  when a caller needs the core path, and measure it.

## Files

| File | Holds |
| --- | --- |
| `UnityFrame.cs` | `UnityFrame` conversions, `ReferenceComparer`, `UnityHandleTable` |
| `UnityWorld.cs` | `UnityWorld` (U-O), `UnityProps` (U-M1 apply), `UnityAnim` (U-M2), `UnityAssets` (U-C1/C2), `UnityPhysics` + `UnityContactRelay` (U-X) |
| `UnityAudioStorage.cs` | `UnityAudio` (U-A), `UnityStorage` (U-P1-P3) |
| `UnityInput.cs` | `UnityInput` (U-I raw side, legacy Input Manager) |
| `UnityHost.cs` | `UnityHost` aggregate + `Frame`, `UnityHostDriver`, `UnityClock`, `UnityLog`, `UnityDisplay`, `UnityPlatform` |

## Rules the code depends on

- **Frame: mirror z.** glTF (RH, Y-up, forward -Z) <-> Unity (LH, Y-up, forward +Z):
  `pos (x,y,z) -> (x,y,-z)`, `quat (x,y,z,w) -> (-x,-y,z,w)`, both directions. Unity's forward maps to
  the core's -Z, and Godot (already glTF-like) will need no conversion. Scale does not mirror. Never
  cast a `Quat` to a `Quaternion`: both are xyzw, so the cast compiles and every character faces the
  wrong way.
- **Transforms are local** (relative to the parent, world at the root), as in the null host.
- **Handles come from a counter**, never an engine object id. The table is keyed by **reference**
  (`ReferenceComparer`): `UnityEngine.Object.Equals` treats every destroyed object as equal to null, and
  `GetInstanceID` is a **compile error** (CS0619) in Unity 6000.5.
- **`Destroy` releases the whole subtree at once.** `Object.Destroy` lands at end of frame, but the
  contract (and `NullWorld`) says a destroyed parent's children answer `deadHandle` immediately.
- **Spawn/Destroy do not use the house object pool.** A pooled destroy leaves the GameObject alive,
  so `Alive` would lie. Reuse belongs to the core (`HandlePool` over Spawn/SetVisible).
- **Colours go through one `MaterialPropertyBlock`**: `_BaseColor`, then `_Color`. A colour tween never
  instantiates a material. Both sides are sRGB, so no conversion.
- **Audio voices are (AudioSource, clip) pairs.** AudioSystem plays through pooled `audio-item`s that
  get recycled, so a voice is dead once its source plays a different clip or a one-shot stops.
  **2D voices are parented to AudioSystem's own object** (DontDestroyOnLoad). Parented to the
  scene-bound `_SoundContainerDisposable`, a loop started at boot died with the boot scene.
  `Prune()` runs every frame so finished one-shots never pile up.
- **Storage roots:** `save:/` persistentDataPath, `cache:/` temporaryCachePath, `content:/`
  streamingAssetsPath (read-only; a sync read on Android is `ioError`). A `..` segment is
  `invalidArgument`. `WriteAtomic` writes `<path>.tmp` and then `File.Replace`/`File.Move`.
  KV maps 1:1 onto PlayerPrefs through SystemPrefUtil.
- **Input polls only watched controls and pushes changes**, since the action map keeps raw state.
  `WatchMap(def)` watches every binding, including composite parts and `.x`/`.y` of 2D controls.
  `Input.GetJoystickNames` allocates, so device changes are checked once a second. Pad axes need
  `SetAxisName(control, inputManagerAxis)`. The project's default `Horizontal`/`Vertical` also read
  the keyboard, so they are NOT mapped by default. The Input System (1.20.0) was installed in action-bots on
  2026-09-26 with the handler left on Both, but `UnityInput` does not use it yet. `manifest.json` is gitignored there,
  and `ENABLE_INPUT_SYSTEM` tracks the player setting, not the package, so any Input System code needs a real guard. Pointer 0 is the mouse, and touches are `fingerId + 1`.
  Positions are in design space: top-left origin, `/ (pixelHeight / 640)`.
- **Frame order** (`UnityHost.Frame`): `input.Poll` -> `GameClock.Tick` -> `assets.PumpAsync` ->
  `physics.DeliverContacts` -> `OnFrame` -> `OnFixed` x steps -> `Scheduler.Advance` -> `bus.Drain(8)` ->
  `audio.Prune`. This is NullHost.Frame, with raw input first and contacts before any OnFixed.
  Focus loss or pause calls `input.ReleaseAll()`, because the OS never sends the key-up.
- **Name collisions to alias:** `UnityEngine.DisplayInfo` (use `using DisplayInfo = Agnostic.Host.DisplayInfo;`).
  Inside `namespace Engine.*`, write `UnityEngine.Physics` and `UnityEngine.Animation`, because
  `Engine.Animation` and friends are namespaces. Never name a namespace `Engine.Agnostic`, or
  `Agnostic.Core` stops resolving.

## Verified 2026-09-26 (Unity 6000.5.2f1, Editor, action-bots)

- **Compile:** clean, with no warnings from the new files. `Engine.AgnosticHost.UnityHost` loads.
- **Frame math:** over 2000 random rotations × vectors, `mirror(q ⊗ v) == mirror(q) ⊗ mirror(v)` to
  **1.8e-7**, and the round-trip is the identity. `Vector3.forward` becomes core `(0,0,-1)`. A core
  +45° about +Y reads Unity euler Y 315.
- **World:** spawn, parented spawn, a missing key (`#0`), Find below a handle and at scene root,
  SetTransform/GetTransform round-trip, SetVisible toggles `activeSelf` while staying alive, Attach
  refuses a cycle (`invalidArgument`), and after Destroy the root, child and grandchild are all
  `deadHandle` immediately.
- **Physics:** the ray hits the spawned cube's handle at 8.59 (the 45°-rotated, 2×-scaled box
  corner, 10 − √2), overlap returns `[#1,#2]`, the reverse ray misses, and a capsule sweep hits at 8.29.
- **Props:** colour and alpha land in the property block (`RGBA(1,0,0,0.5)`) with the shared
  material untouched. An unknown property returns `notFound`. Anim on a cube returns `unsupported`.
- **Assets:** zero synchronous callbacks. On later frames: asset `ok` with a handle, missing
  `notFound`, world `notFound`.
- **Audio:** one-shot and loop play, and a bad clip returns `#0`. A loop started at **frame 1**
  survives the boot→`GameSceneDynamic` load and stops `ok`.
- **Storage/KV:** write, overwrite, read, list, delete, then delete again (`notFound`). `..`, a
  `content:/` write and an unrooted path are all `invalidArgument`. Probe files and keys were removed.
- **Cost (profiler, `UnityHostDriver.Update` over 201 frames, 7 watched controls, 2 repeating
  timers):** **0 GC.Alloc, ~4 µs/frame avg** in the Editor. It started at 128–180 B per firing frame.
  The cause was the core `Scheduler`'s `List.Sort`: both `Sort(Comparison)` and `Sort(IComparer)`
  allocate on Unity's Mono. It is now a hand-rolled insertion sort in game-lib-bitty-base, and the
  vectors are 17/17. The only remaining allocation is the joystick check, about 32 B/s.
- **Not measured:** device numbers, and before/after on gameplay. Nothing in gameplay calls the host yet.

## First adopter: dasher input (P3.2, 2026-09-26)

- **`ActionMapJson`** (`Engine/AgnosticHost/ActionMapJson.cs`) maps an `input-action-map.v1` document onto
  `ActionMapDef` with LitJson, applying the same field rules as the vector runner's `ParseMap`. LitJson's indexer throws on a
  missing key and stores `1` as an int, which a `(double)` cast rejects, so both are guarded. A bad document logs once and returns
  null, and nothing throws out of it.
- **`UnityHostDriver` is `[DefaultExecutionOrder(-1000)]`.** Gameplay reads actions in its own Update, so the
  evaluate must come first or objects disagree about the frame.
- The adopter is **`GameInputActions`** in game-lib-games (`Game/Events/`). It boots in `AfterSceneLoad` only if
  `Resources/agnostic/input-action-map.json` exists (the game opts in through data), and it rebuilds every play
  because domain reload is off. `enabled` is the kill switch. The actions are `move` (WASD + arrows, sensitivity 0.99 to match
  the legacy 0.99 per axis) and `run` (legacy Fire3: left-cmd, mouse.middle, pad.west).
- Call sites: `GameTouchInputAxis` (both keyboard branches → `KeyAxis()`) and
  `BaseGamePlayerThirdPersonController`'s Fire3 read → `IsRunHeld()`. The touch sticks still send directly from
  `BaseGameHUD`, because routing them through the map would clamp the aim stick's unclamped value (it slows
  movement by `v2/10`).
- **Verified in a live round** by injecting raw keys into the map (`map.OnButton(1, "key.d", true)`). The
  host only pushes changes, so an injected state persists. Results: d → player h=0.99 and moving; d+w → (0.70,0.70); a+d → cancel;
  cmd → run; release → 0 and it stops. With `enabled=false` the same injection does nothing, because legacy only reads the real keyboard.
- **Profiler, 201 frames, idle input:** `GameTouchInputAxis.Update` went from 4.7 µs and **248 B/frame** to 3.0 µs and **0 B**. The
  garbage was `IsEqualLowercase` (both strings `ToLower()`'d, per pad per frame), which is now an ordinal ignore-case compare.
  `UnityHostDriver.Update` costs 11.1 µs/frame with 11 watched controls, and 64 B in total (the joystick check).

## Pads through the Input System (P3 step 4, 2026-09-26)

- **`Engine/AgnosticHost/InputSystemPads/`** is its own asmdef, `Engine.AgnosticHost.InputSystemPads`, with
  `versionDefines` on `com.unity.inputsystem` giving `AGNOSTIC_INPUT_SYSTEM`, and `defineConstraints` on that define.
  On a machine without the package the define is unmet, so Unity skips the assembly and nothing fails to resolve.
  `manifest.json` is gitignored, and `ENABLE_INPUT_SYSTEM` tracks the player setting, which is why neither can be the guard.
- **`UnityInput` binds it by name** (`Type.GetType` + `Delegate.CreateDelegate` once, in static fields), since
  Assembly-CSharp cannot reference an assembly that may not exist. `UnityInput.inputSystemPads` reports whether it bound.
  When it has, **legacy `pad.*` watches are skipped**, so the two sources never fight over one name.
  `[assembly: AlwaysLinkAssembly]` + `[Preserve]` keep it from being stripped, because nothing references it statically.
- It pushes `pad.south/east/west/north`, shoulders, select/start, stick presses, `pad.dpad-*`,
  `pad.left-stick.x/.y`, `pad.right-stick.x/.y`, and `pad.left-trigger`/`pad.right-trigger`, all as **unprocessed**
  values. The core owns the dead zone, and the Input System's own stick dead zone would stack on top of it. It pushes changes only,
  and a pad unplugged mid-press is released. The state is static and domain reload is off, so the `UnityInput`
  constructor clears it.
- **Verified with a virtual `Gamepad`** (`InputSystem.AddDevice<Gamepad>` + `QueueStateEvent`) in a live
  round: left stick → `move` → the player walks, 0.1 is inside the 0.15 dead zone and reads 0, right stick → `aim` → the attack
  axis (h2=1), west → `run`, neutral → 0, and removing the device mid-deflection → 0 and the player stops.
- **Found by the profiler:** a map with stick bindings cost **~220 B/frame**, pad or no pad. `ActionMap.Raw2d`
  built `control + ".x"` per Evaluate. game-lib-bitty-base now caches the part names per control, so the driver is
  back to 0 B/frame at ~11 µs idle and ~16 µs with a pad held. `GameTouchInputAxis` still shows ~64 B/frame
  **while an axis is held**; that is the per-frame `SendInputAxisMessage` broadcast a held key always paid.

## Gaps (v0)

- Without the Input System package, pads fall back to legacy joystick buttons 0–7 (Xbox order, no per-OS layout).
- `Share` returns `unsupported`, because the house has no share sheet. `Vibrate` ignores the pattern (DeviceUtil has one buzz).
- `Find` walks `Transform.Find`. Contacts need `UnityPhysics.WatchContacts(handle)` per object.
- `SetEngineTimeScale` writes `Time.timeScale` directly, so it would fight any house code that also writes it.
