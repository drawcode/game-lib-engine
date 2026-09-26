using UnityEngine;

using Engine.UI.Bitty;
using Engine.Utility;

namespace Engine.UI {

    // The Unity text source for UITokens (bitty gap-list step 2). The parse in UITokens.LoadFromJson
    // is engine-free; only finding the text is per engine.
    public static class UITokensLoader {

        // A missing file is not an error: the built-in defaults stand.
        public static bool LoadFromResources(string resourcePath = UITokens.defaultResourcePath) {

            TextAsset asset = Resources.Load<TextAsset>(resourcePath);

            if (asset == null) {
                LogUtil.Log("UITokens: no token file at Resources/" + resourcePath
                    + " — using built-in defaults");
                return false;
            }

            return UITokens.LoadFromJson(asset.text);
        }
    }
}
