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
class PreStageTwoDialogueScreen : DialogueScreen {

    #region Initialization


    /// <summary>
    /// Constructor.
    /// </summary>
    public PreStageTwoDialogueScreen() : base() {}


    /// <summary>
    /// Loads graphics content for this dialogue screen and add in panels
    /// </summary>
    public override void LoadContent() {
        base.LoadContent();

        BackgroundTexture = Content.Load<Texture2D>("background");
        Texture2D GuraFace = Content.Load<Texture2D>("GameplayAssets/Dialogue Assets/gawr_face2");
        Texture2D AmeFace = Content.Load<Texture2D>("GameplayAssets/Dialogue Assets/amelia_face");

        List<Texture2D> JustGura = new List<Texture2D>();
        List<Texture2D> GuraAme = new List<Texture2D>();
        List<Texture2D> AmeGura = new List<Texture2D>();
        JustGura.Add(GuraFace);
        GuraAme.Add(GuraFace);
        GuraAme.Add(AmeFace);

        Panels.Enqueue(new Panel(
            "test name",
            "hello world!",
            JustGura
        ));
        Panels.Enqueue(new Panel(
            "test name",
            "hello world! AGAIN! Screen should load next after hitting next",
            GuraAme
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
                     new StageTwoBackgroundScreen(),
                     new StageTwoGameScreen("Content/Beatmaps/Meconopsis/Meconopsis.json"));
    } 


    /// <summary>
    /// Draws the background screen.
    /// </summary>
    public override void Draw(GameTime gameTime) {

        base.Draw(gameTime);  // base dialogue screen handles drawing everything
    }
    #endregion
}