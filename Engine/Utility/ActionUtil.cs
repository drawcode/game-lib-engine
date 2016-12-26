using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;

namespace Engine.Utility {

    public class ActionUtil {

        public static Action CombineAction(Action action, Action actionOther) {
            action += actionOther;
            return action;
        }

        public static Action<ISomeInterface> CombineAction<ISomeInterface>(
            Action<ISomeInterface> action, Action<ISomeInterface> actionOther) {
            action += actionOther;
            return action;
        }

        public static Action CombineActions(
            Action action, IEnumerable<Action> actionOthers) {

            IEnumerable<Action> actions = actionOthers;
            action = (Action)
                Delegate.Combine(actions.ToArray());
            return action;
        }

        public static Action<ISomeInterface> CombineActions<ISomeInterface>(
            Action<ISomeInterface> action, IEnumerable<Action<ISomeInterface>> actionOthers) {

            IEnumerable<Action<ISomeInterface>> actions = actionOthers;
            action = (Action<ISomeInterface>)
                Delegate.Combine(actions.ToArray());
            return action;
        }
    }

}