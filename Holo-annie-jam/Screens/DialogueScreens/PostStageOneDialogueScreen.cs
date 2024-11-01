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
        Texture2D GuraFace = Content.Load<Texture2D>("GameplayAssets/Dialogue Assets/gawr_face2");
        Texture2D AmeFace = Content.Load<Texture2D>("GameplayAssets/Dialogue Assets/amelia_face");

        List<Texture2D> JustGura = new List<Texture2D>();
        List<Texture2D> GuraAme = new List<Texture2D>();
        List<Texture2D> AmeGura = new List<Texture2D>();
        JustGura.Add(GuraFace);
        GuraAme.Add(GuraFace);
        GuraAme.Add(AmeFace);
        AmeGura.Add(AmeFace);
        AmeGura.Add(GuraFace);

        Panels.Enqueue(new Panel(
            "Gura",
            new List<string> { "What the heck was that! Why were clocks running at me???" },
            GuraAme
        ));
        Panels.Enqueue(new Panel(
            "Ame",
            new List<string> { "That's just my thing, you did pretty good though." },
            AmeGura
        ));
        Panels.Enqueue(new Panel(
            "Gura",
            new List<string> { "That doesn't answer my question." },
            GuraAme
        ));
        Panels.Enqueue(new Panel(
            "Ame",
            new List<string> { "And it wasn't supposed to!" },
            AmeGura
        ));
        Panels.Enqueue(new Panel(
            "Gura",
            new List<string> { "Oh, ok." },
            GuraAme
        ));
        Panels.Enqueue(new Panel(
            "Ame",
            new List<string> { "Anyway, you need to finish going on an adventure!", "I think you should go... That-a-way!" },
            AmeGura
        ));
        Panels.Enqueue(new Panel(
            "Gura",
            new List<string> { "Are you gonna explain anything else?" },
            GuraAme
        ));
        Panels.Enqueue(new Panel(
            "Ame",
            new List<string> { "Don't reset too many times or you might regret it. Hehehe." },
            AmeGura
        ));
        Panels.Enqueue(new Panel(
            "Ame",
            new List<string> { "OK byeeeeeee..." },
            AmeGura
        ));
        Panels.Enqueue(new Panel(
            "Gura",
            new List<string> { "That's not helpful at all!" },
            GuraAme
        ));
        Panels.Enqueue(new Panel(
            "Gura",
            new List<string> { "Well, Guess I have nothing better to do.", "Adventure time for Goomba!" },
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
        LoadingScreen.Load(ScreenManager, false, playerIndex,
            new StageTwoDialogueBackgroundScreen(),
            new PreStageTwoDialogueScreen()
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