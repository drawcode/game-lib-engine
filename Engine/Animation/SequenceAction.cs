using System;
using System.Collections.Generic;

namespace Engine.Animation {

    public class SequenceActionType {
        public static string ACTION_SCENE = "action-scene";
        public static string ACTION_OPEN_BROWSER = "action-open-browser";
        public static string ACTION_OPEN_WEB_VIEW = "action-open-web-view";
    }

    public class SequenceAction : SequenceBase {
        public string actionType = SequenceActionType.ACTION_SCENE;
        public string actionName;
        public string actionValue;

        public SequenceAction() {
            Reset();
        }

        public void Reset() {
            type = "";
            attributes = new Dictionary<string, object>();
            actionType = SequenceActionType.ACTION_SCENE;
            actionName = "";
            actionValue = "";
        }
    }
}