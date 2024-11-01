#region Using Statements
using Microsoft.Xna.Framework;
#endregion

/// <summary>
/// The main menu screen is the first thing displayed when the game starts up.
/// </summary>
class StageSelectScreen : MenuScreen {
    #region Initialization


    /// <summary>
    /// Constructor fills in the menu contents.
    /// </summary>
    public StageSelectScreen() : base("Stage Select") {
        // Create our menu entries.
        MenuEntry stageOneMenuEntry = new MenuEntry("Stage One");
        MenuEntry stageTwoMenuEntry = new MenuEntry("Stage Two");
        MenuEntry stageThreeMenuEntry = new MenuEntry("Stage Three");
        MenuEntry stageFourMenuEntry = new MenuEntry("Stage Four");
        MenuEntry exitMenuEntry = new MenuEntry("Exit");

        // Hook up menu event handlers.
        stageOneMenuEntry.Selected += StageOneMenuEntrySelected;
        stageTwoMenuEntry.Selected += StageTwoMenuEntrySelected;
        stageThreeMenuEntry.Selected += StageThreeMenuEntrySelected;
        stageFourMenuEntry.Selected += StageFourMenuEntrySelected;
        exitMenuEntry.Selected += OnCancel;

        // Add entries to the menu.
        MenuEntries.Add(stageOneMenuEntry);
        MenuEntries.Add(stageTwoMenuEntry);
        MenuEntries.Add(stageThreeMenuEntry);
        MenuEntries.Add(stageFourMenuEntry);
        MenuEntries.Add(exitMenuEntry);
    }


    #endregion

    #region Handle Input

    void StageOneMenuEntrySelected(object sender, PlayerIndexEventArgs e) {
        LoadingScreen.Load(ScreenManager, false, e.PlayerIndex,
            new StageOneDialogueBackgroundScreen(),
            new PreStageOneDialogueScreen()
        );
    }
    void StageTwoMenuEntrySelected(object sender, PlayerIndexEventArgs e) {
        LoadingScreen.Load(ScreenManager, false, e.PlayerIndex,
            new PreStageTwoDialogueScreen()
        );
    }
    void StageThreeMenuEntrySelected(object sender, PlayerIndexEventArgs e) {
        LoadingScreen.Load(ScreenManager, false, e.PlayerIndex,
            new PreStageThreeDialogueScreen()
        );
    }
    void StageFourMenuEntrySelected(object sender, PlayerIndexEventArgs e) {
        LoadingScreen.Load(ScreenManager, false, e.PlayerIndex,
            new PreStageFourDialogueScreen()
        );
    }

    protected override void OnCancel(PlayerIndex playerIndex) {
        LoadingScreen.Load(ScreenManager, false, playerIndex,
            new MenuBackgroundScreen(),
            new MainMenuScreen()
        );
    }

    #endregion
}