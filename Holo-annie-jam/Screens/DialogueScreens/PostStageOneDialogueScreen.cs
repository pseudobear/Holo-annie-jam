#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
#endregion

/// <summary>
/// The dialogue screen that appears right before stage 1
/// </summary>
class PostStageOneDialogueScreen : DialogueScreen {

    #region Initialization


    /// <summary>
    /// Constructor.
    /// </summary>
    public PostStageOneDialogueScreen() : base() {}


    /// <summary>
    /// Loads graphics content for this dialogue screen and add in panels
    /// </summary>
    public override void LoadContent() {
        base.LoadContent();

        BackgroundTexture = Content.Load<Texture2D>("background");
        Texture2D gradient = Content.Load<Texture2D>("GWAR_GURA_1");

        List<Texture2D> singleCharacterList = new List<Texture2D>();
        singleCharacterList.Add(gradient);

        Panels.Enqueue(new Panel(
            "test name",
            "hello world!",
            singleCharacterList
        ));
        Panels.Enqueue(new Panel(
            "test name",
            "hello world! AGAIN! Screen should load next after hitting next",
            singleCharacterList
        ));
    }

    #endregion

    #region Update and Draw

    /// <summary>
    /// Updates the main dialogue screen, let base dialogue handle transition.
    /// </summary>
    public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                    bool coveredByOtherScreen) {
        base.Update(gameTime, otherScreenHasFocus, false);
    }

    public override void OnCompletePanels(PlayerIndex? playerIndex) {
        LoadingScreen.Load(ScreenManager, true, playerIndex,
                     new GameplayBackgroundScreen(),
                     new MainGameScreen("Content/Beatmaps/Reflect/Reflect.json"));
    } 


    /// <summary>
    /// Draws the background screen.
    /// </summary>
    public override void Draw(GameTime gameTime) {

        base.Draw(gameTime);  // base dialogue screen handles drawing everything
    }
    #endregion
}