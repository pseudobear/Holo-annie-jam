#region Using Statements
using Microsoft.Xna.Framework;
using System;
#endregion

/// <summary>
/// The pause menu comes up over the top of the game,
/// giving the player options to resume or quit.
/// </summary>
class PauseMenuScreen : MenuScreen {
    #region Initialization

    private readonly Action _onResume;

    /// <summary>
    /// Constructor.
    /// </summary>
    public PauseMenuScreen(Action onResume)
        : base("Paused") {
        this._onResume = onResume;

        // Create our menu entries.
        MenuEntry resumeGameMenuEntry = new MenuEntry("Resume Game");
        MenuEntry quitGameMenuEntry = new MenuEntry("Quit Game");

        // Hook up menu event handlers.
        resumeGameMenuEntry.Selected += OnCancel;
        quitGameMenuEntry.Selected += QuitGameMenuEntrySelected;

        // Add entries to the menu.
        MenuEntries.Add(resumeGameMenuEntry);
        MenuEntries.Add(quitGameMenuEntry);
    }


    #endregion

    #region Handle Input


    /// <summary>
    /// Event handler for when the Quit Game menu entry is selected.
    /// </summary>
    void QuitGameMenuEntrySelected(object sender, PlayerIndexEventArgs e) {
        const string message = "Are you sure you want to quit this game?";

        MessageBoxScreen confirmQuitMessageBox = new MessageBoxScreen(message);

        confirmQuitMessageBox.Accepted += ConfirmQuitMessageBoxAccepted;

        ScreenManager.AddScreen(confirmQuitMessageBox, ControllingPlayer);
    }


    /// <summary>
    /// Event handler for when the user selects ok on the "are you sure
    /// you want to quit" message box. This uses the loading screen to
    /// transition from the game back to the main menu screen.
    /// </summary>
    void ConfirmQuitMessageBoxAccepted(object sender, PlayerIndexEventArgs e) {
        LoadingScreen.Load(ScreenManager, false, null, 
            new MenuBackgroundScreen(),
            new MainMenuScreen()
        );
    }

    protected override void OnCancel(PlayerIndex playerIndex) {
        this._onResume();
        base.OnCancel(playerIndex);
    }


    #endregion
}