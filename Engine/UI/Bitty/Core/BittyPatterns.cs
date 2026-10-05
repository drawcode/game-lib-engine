using System.Collections.Generic;

namespace Engine.UI.Bitty {

    // The pattern catalog. AGNOSTIC — expands a `pattern` node into a concrete BittyNode subtree
    // BEFORE any backend builds it, so the element factory only ever sees plain elements.
    //
    // v1 IMPLEMENTS one pattern for real — option-row — because 3A (settings) is its first real
    // consumer and the plan's abstraction-overreach guard says a pattern is designed against a
    // real panel or not at all. The other five names (dialog/list/chrome/hud-widget/overlay) are
    // pinned in BittySchema.Patterns and land structurally with their own waves (3D list, 3F
    // dialog, 3B chrome, 3H hud-widget); here they resolve to their shared common.uss class and
    // recurse, which is all they need until a panel exercises them.
    public static class BittyPatterns {

        // Depth-first: children expand before the node, so a pattern that consumes its children
        // (option-row reads its control child) sees already-concrete children.
        public static BittyNode Expand(BittyNode node) {

            if (node == null) {
                return null;
            }

            for (int i = 0; i < node.children.Count; i++) {
                node.children[i] = Expand(node.children[i]);
            }

            if (string.IsNullOrEmpty(node.pattern)) {
                return node;
            }

            if (node.pattern == BittySchema.Patterns.optionRow) {
                return ExpandOptionRow(node);
            }

            if (node.pattern == BittySchema.Patterns.list) {
                return ExpandList(node);
            }

            return ExpandGeneric(node);
        }

        // A named-but-not-yet-structural pattern: attach its class (common.uss already styles all
        // six) and clear `pattern` so it is not re-expanded. Structure arrives with the pattern's
        // own wave.
        private static BittyNode ExpandGeneric(BittyNode node) {

            if (!node.classes.Contains(node.pattern)) {
                node.classes.Add(node.pattern);
            }

            node.pattern = null;

            return node;
        }

        // OPTION-ROW — a labeled value control. The two live shapes (audio, controls) drive the
        // two variants:
        //
        //   * slider control  -> STACKED: an external label sits ABOVE a full-width control
        //                        (audio: "MUSIC VOLUME" over the bar).
        //   * toggle control  -> INLINE: the checkbox uses `label` as its OWN text and sits beside
        //                        it (controls: the box + "Vibrate on HITS"), because UI Toolkit's
        //                        Toggle renders its label natively — a separate label element would
        //                        double it.
        //
        // The control child keeps its own `name` (the broadcast/bind key, e.g.
        // "SliderAudioMusicVolume"); only the wrapper takes the row's name. So the value bus and
        // BindElements still key on the widget, not the row.
        private static BittyNode ExpandOptionRow(BittyNode n) {

            BittyNode row = new BittyNode();
            row.type = BittySchema.Types.container;
            row.name = n.name;
            row.bind = n.bind;
            row.events = n.events;
            row.classes.Add("option-row");
            row.classes.AddRange(n.classes);

            BittyNode control = FindControl(n);
            bool inline = control != null && control.type == BittySchema.Types.toggle;

            row.classes.Add(inline ? "option-row--inline" : "option-row--stacked");

            if (inline) {

                // Toggle carries the label as its native text; author `text` wins if given.
                if (control != null
                        && string.IsNullOrEmpty(control.text)
                        && !string.IsNullOrEmpty(n.label)) {
                    control.text = n.label;
                }

                for (int i = 0; i < n.children.Count; i++) {
                    row.children.Add(n.children[i]);
                }

                return row;
            }

            // STACKED: external label above the control.
            if (!string.IsNullOrEmpty(n.label)) {

                BittyNode lbl = new BittyNode();
                lbl.type = BittySchema.Types.label;
                lbl.name = string.IsNullOrEmpty(n.name) ? null : n.name + "__label";
                lbl.text = n.label;
                lbl.classes.Add("option-row__label");

                row.children.Add(lbl);
            }

            for (int i = 0; i < n.children.Count; i++) {
                row.children.Add(n.children[i]);
            }

            return row;
        }

        // LIST — a scrollable item collection (wave 3D; first consumer panel-settings-help,
        // dynamic consumer panel-game-mode-missions). The declared children are the rows:
        // static rows are authored directly; a dynamic list declares ONE row named
        // "<X>Template" with class "list-item-template" (display:none in common.uss) that the
        // backend row API (IUIBackend.AddListItem) rebuilds per data item at runtime.
        //
        // Expansion follows the as-built ScrollView recipe: the node becomes a `scrollview`
        // (the builder auto-tags .ngui-scrollview, so ConfigureScrollViews wires the themed
        // scroller + ScrollDrag), and the rows wrap in ONE content container (.list-content)
        // so the scroll height is the content's real height.
        private static BittyNode ExpandList(BittyNode n) {

            BittyNode sv = new BittyNode();
            sv.type = BittySchema.Types.scrollview;
            sv.name = n.name;
            sv.bind = n.bind;
            sv.events = n.events;
            sv.classes.Add("list");
            sv.classes.AddRange(n.classes);

            BittyNode content = new BittyNode();
            content.type = BittySchema.Types.container;
            content.name = string.IsNullOrEmpty(n.name) ? null : n.name + "__content";
            content.classes.Add("list-content");

            for (int i = 0; i < n.children.Count; i++) {
                content.children.Add(n.children[i]);
            }

            sv.children.Add(content);

            return sv;
        }

        private static BittyNode FindControl(BittyNode n) {

            for (int i = 0; i < n.children.Count; i++) {

                string t = n.children[i].type;

                if (t == BittySchema.Types.slider
                        || t == BittySchema.Types.toggle
                        || t == BittySchema.Types.textfield
                        || t == BittySchema.Types.dropdown) {
                    return n.children[i];
                }
            }

            return null;
        }
    }
}
