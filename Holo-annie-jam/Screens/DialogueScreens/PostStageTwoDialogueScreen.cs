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
class PostStageTwoDialogueScreen : DialogueScreen {

    #region Initialization


    /// <summary>
    /// Constructor.
    /// </summary>
    public PostStageTwoDialogueScreen() : base() {}


    /// <summary>
    /// Loads graphics content for this dialogue screen and add in panels
    /// </summary>
    public override void LoadContent() {
        base.LoadContent();

        BackgroundTexture = Content.Load<Texture2D>("background"); 
        Texture2D GuraFace = Content.Load<Texture2D>("GameplayAssets/Dialogue Assets/gawr_face2");
        Texture2D InaFace = Content.Load<Texture2D>("GameplayAssets/Dialogue Assets/ina'nis_face");

        List<Texture2D> JustGura = new List<Texture2D>();
        List<Texture2D> GuraIna = new List<Texture2D>();
        List<Texture2D> InaGura = new List<Texture2D>();
        JustGura.Add(GuraFace);
        GuraIna.Add(GuraFace);
        GuraIna.Add(InaFace);
        InaGura.Add(InaFace);
        InaGura.Add(GuraFace);

        Panels.Enqueue(new Panel(
            "Ina",
            new List<string> {"Oh, are you 'right back' now?" },
            InaGura
        ));
        Panels.Enqueue(new Panel(
            "Gura",
            new List<string> {"Huh?" },
            GuraIna
        ));
        Panels.Enqueue(new Panel(
            "Ina",
            new List<string> {"Oh don't worry about it. I see you found AO-chan!" },
            InaGura
        ));
        Panels.Enqueue(new Panel(
            "Gura",
            new List<string> {"You could've mentioned AO-chan was a book." },
            GuraIna
        ));
        Panels.Enqueue(new Panel(
            "Ina",
            new List<string> {"AO-chan is a tablet sometimes too!" },
            InaGura
        ));
        Panels.Enqueue(new Panel(
            "Gura",
            new List<string> {"I see. Is there anything else you lost in this here cave?" },
            GuraIna
        ));
        Panels.Enqueue(new Panel(
            "Ina",
            new List<string> {"No, AO-chan wandered off, but everything else is here." },
            InaGura
        ));
        Panels.Enqueue(new Panel(
            "Gura",
            new List<string> {"Alright then, I'm gonna go then." },
            GuraIna
        ));
        Panels.Enqueue(new Panel(
            "Ina",
            new List<string> {"Wait, I'll come with you.", "I've been stuck in this cave for a long time." },
            InaGura
        ));
        Panels.Enqueue(new Panel(
            "Ina",
            new List<string> {"I guess you could say I'm excited to leave", "my house for the first time 'Ina' while." },
            InaGura
        ));
        Panels.Enqueue(new Panel(
            "Gura",
            new List<string> {"..." },
            GuraIna
        ));
        Panels.Enqueue(new Panel(
            "Ina",
            new List<string> {"Hehehe..." },
            InaGura
        ));
        Panels.Enqueue(new Panel(
            "Gura",
            new List<string> {"I'm leaving." },
            GuraIna
        ));
        Panels.Enqueue(new Panel(
            "Ina",
            new List<string> { "Wait for me." },
            InaGura
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
            new StageThreeDialogueBackgroundScreen(),
            new PreStageThreeDialogueScreen()
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