using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;

using Engine.Utility;

public static class ActionExtensions {
    
    public static Action CombineAction(this Action action, Action actionOther) {
        return ActionUtil.CombineAction(action, actionOther);
    }

    public static Action<ISomeInterface> CombineAction<ISomeInterface>(
        this Action<ISomeInterface> action, Action<ISomeInterface> actionOther) {
        return ActionUtil.CombineAction<ISomeInterface>(action, actionOther);
    }

    public static Action CombineActions(
        this Action action, IEnumerable<Action> actionOthers) {
        return ActionUtil.CombineActions(action, actionOthers);
    }

    public static Action<ISomeInterface> CombineActions<ISomeInterface>(
        this Action<ISomeInterface> action, IEnumerable<Action<ISomeInterface>> actionOthers) {
        return ActionUtil.CombineActions<ISomeInterface>(action, actionOthers);
    }
}
