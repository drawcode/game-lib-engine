using System.Collections.Generic;

using UnityEngine;

namespace Engine.UI {

    // Sprite table for UIQuadSprite: name -> outer/inner UV rect inside one atlas texture.
    // AGNOSTIC — no UI backend types.
    //
    // Why this exists: a baked quad carries the UVs of the sprite it was baked from, which is
    // enough until a sprite is RENAMED at runtime (action-zone icons swap icon per action). With
    // the legacy atlas gone there is nothing left to look the new name up in, so the baker writes
    // the legacy atlas's sprite rects into this asset and UIQuadSprite.SetSprite reads them.
    //
    // Rects are normalized texture coordinates (same space as UIQuadSprite.uvs), origin bottom-left.
    public class UIQuadSpriteAtlas : ScriptableObject {

        [System.Serializable]
        public class Entry {
            public string name;
            public Rect outer;
            public Rect inner;
        }

        public List<Entry> sprites = new List<Entry>();

        Dictionary<string, Entry> lookup;

        // The baker rewrites `sprites` in the Editor; drop the cache with it.
        void OnValidate() {
            lookup = null;
        }

        public Entry Get(string spriteName) {

            if (string.IsNullOrEmpty(spriteName)) {
                return null;
            }

            // Built once (a count compare would rebuild forever on one duplicate name).
            if (lookup == null) {

                lookup = new Dictionary<string, Entry>(sprites.Count);

                foreach (Entry e in sprites) {
                    if (e != null && !string.IsNullOrEmpty(e.name)) {
                        lookup[e.name] = e;
                    }
                }
            }

            Entry found;
            return lookup.TryGetValue(spriteName, out found) ? found : null;
        }
    }
}
