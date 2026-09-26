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
  the keyboard, so they are NOT mapped by default. The new Input System is not installed
  (`activeInputHandler: 2`, no package). Pointer 0 is the mouse, and touches are `fingerId + 1`.
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

## Gaps (v0)

- No per-OS gamepad layout (legacy joystick buttons 0–7 are Xbox order). A real pad wants the Input System.
- `Share` returns `unsupported`, because the house has no share sheet. `Vibrate` ignores the pattern (DeviceUtil has one buzz).
- `Find` walks `Transform.Find`. Contacts need `UnityPhysics.WatchContacts(handle)` per object.
- `SetEngineTimeScale` writes `Time.timeScale` directly, so it would fight any house code that also writes it.
