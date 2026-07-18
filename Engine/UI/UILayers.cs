namespace Engine.UI {

    // Draw-order bands for toolkit views. AGNOSTIC — plain ints, no backend types.
    //
    // Why this exists: the toolkit backend used to stamp every view with an auto-incrementing
    // sortingOrder, i.e. draw order == LOAD order. That is fine while every view is a flow-scoped
    // screen, but always-on chrome (header/footer) loads EARLY and must still draw ABOVE screens
    // loaded later. So a panel declares which band it belongs to and the backend honours it.
    //
    // This is deliberately NOT a lifetime/persistence mechanism. Lifetime stays a per-panel
    // property of enable/disable: a view loads on first show and FreeToolkitView releases it on
    // OnDisable. Chrome stays resident only because its GameObject stays enabled while its flow is
    // active; when the game disables it (leaving the menu flow) it frees through the same path as
    // every other panel. Dynamic load/unload remains the default for everything.
    public class UILayers {

        // "Let the backend auto-assign" — preserves the original load-order behavior. Default.
        public const int auto = -1;

        // Flow-scoped screens (settings, main, results...). The auto band starts here.
        public const int panel = 100;

        // Always-on chrome (header/footer) — above screens, below overlays. Headroom is large so
        // a long session of auto-assigned panels can never climb into this band.
        public const int chrome = 10000;

        // Dialogs / loading / transition overlays — above everything.
        public const int overlay = 20000;
    }
}
