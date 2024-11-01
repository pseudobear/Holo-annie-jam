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
class PostStageThreeDialogueScreen : DialogueScreen {

    #region Initialization


    /// <summary>
    /// Constructor.
    /// </summary>
    public PostStageThreeDialogueScreen() : base() {}


    /// <summary>
    /// Loads graphics content for this dialogue screen and add in panels
    /// </summary>
    public override void LoadContent() {
        base.LoadContent();

        BackgroundTexture = Content.Load<Texture2D>("background");
        Texture2D GuraFace = Content.Load<Texture2D>("GameplayAssets/Dialogue Assets/gawr_face2");
        Texture2D AmeFace = Content.Load<Texture2D>("GameplayAssets/Dialogue Assets/amelia_face");

        List<Texture2D> JustGura = new List<Texture2D>();
        List<Texture2D> JustAme = new List<Texture2D>();
        JustGura.Add(GuraFace);
        JustAme.Add(AmeFace);

        Panels.Enqueue(new Panel(
            "Gura",
            "Wahahahah, I'm free!! Watson you jerk, you thought you could pull one over good ol' Gura huh?",
            JustGura
        ));
        Panels.Enqueue(new Panel(
            "Gura",
            "Well, I'm out to play now and nothing you do will be able to stop me!",
            JustGura
        ));
        Panels.Enqueue(new Panel(
            "Gura",
            "Wahahahaha!",
            JustGura
        ));
        Panels.Enqueue(new Panel(
            "Ame",
            "Oh, oops, guess she wasn't as good at rhythm games as I thought she was. Darn, now she's getting canceled.",
            JustAme
        ));
        Panels.Enqueue(new Panel(
            "Gura",
            "Guess I'll try again in a different timeline.",
            JustAme
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
        LoadingScreen.Load(ScreenManager, false, playerIndex,
            new MenuBackgroundScreen(),
            new StageSelectScreen()
        );
    } 


    /// <summary>
    /// Draws the background screen.
    /// </summary>
    public override void Draw(GameTime gameTime) {

        base.Draw(gameTime);  // base dialogue screen handles drawing everything
    }
    #endregion
}