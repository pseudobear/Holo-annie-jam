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
class PreStageOneDialogueScreen : DialogueScreen {

    #region Initialization


    /// <summary>
    /// Constructor.
    /// </summary>
    public PreStageOneDialogueScreen() : base() {}


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
            new List<string> { "Man, did anyone get the number of the whale that hit me?" },
            JustGura
        ));
        Panels.Enqueue(new Panel(
            "Gura",
            new List<string> { "Huh? Why am I in a forest? Where am I?" },
            JustGura
        ));
        Panels.Enqueue(new Panel(
            "Gura",
            new List<string> {"I swear this is Watson's fault somehow." },
            JustGura
        ));
        Panels.Enqueue(new Panel(
            "Gura",
            new List<string> {"Watson? Do I know a Watson?" },
            JustGura
        ));
        Panels.Enqueue(new Panel(
            "Ame",
            new List<string> {"Did someone sayyyy Watson?" },
            AmeGura
        ));
        Panels.Enqueue(new Panel(
            "Gura",
            new List<string> {"*Shrieks Incoherently*" },
            GuraAme
        ));
        Panels.Enqueue(new Panel(
            "Ame",
            new List<string> {"Ehehehe, gotcha! It’s been a while since I got to do that." },
            AmeGura
        ));
        Panels.Enqueue(new Panel(
            "Gura",
            new List<string> {"Who are you?!?" },
            GuraAme
        ));
        Panels.Enqueue(new Panel(
            "Ame",
            new List<string> {"Don't worry about it Goomba, the important thing is that you lost", "your memories, so you need to go on an adventure!" },
            AmeGura
        ));
        Panels.Enqueue(new Panel(
            "Gura",
            new List<string> {"Huh? What?" },
            GuraAme
        ));
        Panels.Enqueue(new Panel(
            "Ame",
            new List<string> {"Don't worry about it! Oh yeah, it’s dangerous to go alone, take this!" },
            AmeGura
        ));
        Panels.Enqueue(new Panel(
            "Gura",
            new List<string> {"a" },
            GuraAme
        ));
        Panels.Enqueue(new Panel(
            "Ame",
            new List<string> {"Oh, I broke her. Oh well, she'll figure it out, won’t she chat." },
            AmeGura
        ));
        Panels.Enqueue(new Panel(
            "Ame",
            new List<string> {"Get ready, here they come!" },
            AmeGura
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
                     new StageOneBackgroundScreen(),
                     new StageOneGameScreen("Content/Beatmaps/Chikutaku/Chikutaku.json"));
    } 


    /// <summary>
    /// Draws the background screen.
    /// </summary>
    public override void Draw(GameTime gameTime) {

        base.Draw(gameTime);  // base dialogue screen handles drawing everything
    }
    #endregion
}