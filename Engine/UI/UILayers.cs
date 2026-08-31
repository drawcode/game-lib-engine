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

        // BELOW every flow screen. Shared scenery that panels draw ON TOP of — the character
        // preview card the header owns, which main/game-mode/results/customize-character all
        // show while drawing their own arrows and plates over it. It cannot live in the chrome
        // band (that is above panels, so the card would bury their content) and it cannot live
        // in a panel's own view (four screens would each author, and fight over, one shared rig).
        public const int backdrop = 50;

        // Flow-scoped screens (settings, main, results...). The auto band starts here.
        public const int panel = 100;

        // ABOVE every flow screen, below the always-on chrome. The other half of a shared
        // backdrop: the character rig's BACKER belongs behind a panel's content, but the bot
        // itself and its CUSTOMIZE button belong in front of it — that is the legacy NGUI
        // z-order (see the coop baseline, where the bot's legs and the button both draw over
        // the green mode buttons while the dark card sits behind them). One band each side of
        // `panel` reproduces it without any panel having to know the rig exists.
        public const int foreground = 9000;

        // Always-on chrome (header/footer) — above screens, below overlays. Headroom is large so
        // a long session of auto-assigned panels can never climb into this band.
        public const int chrome = 10000;

        // Dialogs / loading / transition overlays — above everything.
        public const int overlay = 20000;

        // Toasts that must sit above a dialog as well as above a screen — the achievement /
        // point / tip / error notification.
        //
        // This band exists because the shared PanelSettings renders in OVERLAY mode
        // (m_RenderMode: 0), so EVERY toolkit view composites after every camera. A toast left
        // on NGUI therefore draws under the toolkit header no matter what its NGUI depth is —
        // which is exactly the "achievements header sort" report: the legacy toast slid down
        // from the top and the header band, the coin count and the FPS readout drew straight
        // over it. It also matches the legacy camera order, where OverlayCamera (55) sits above
        // DialogCamera (15).
        public const int notification = 30000;
    }
}
