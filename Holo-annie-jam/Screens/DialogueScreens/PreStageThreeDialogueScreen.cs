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
class PreStageThreeDialogueScreen : DialogueScreen {

    #region Initialization


    /// <summary>
    /// Constructor.
    /// </summary>
    public PreStageThreeDialogueScreen() : base() {}


    /// <summary>
    /// Loads graphics content for this dialogue screen and add in panels
    /// </summary>
    public override void LoadContent() {
        base.LoadContent();

        BackgroundTexture = Content.Load<Texture2D>("background");
        Texture2D GuraFace = Content.Load<Texture2D>("GameplayAssets/Dialogue Assets/gawr_face2");
        Texture2D AmeFace = Content.Load<Texture2D>("GameplayAssets/Dialogue Assets/amelia_face");
        Texture2D InaFace = Content.Load<Texture2D>("GameplayAssets/Dialogue Assets/ina'nis_face");


        List<Texture2D> JustGura = new List<Texture2D>();
        List<Texture2D> JustAme = new List<Texture2D>();
        List<Texture2D> GuraAme = new List<Texture2D>();
        List<Texture2D> AmeGura = new List<Texture2D>();
        List<Texture2D> GuraIna = new List<Texture2D>();
        List<Texture2D> InaGura = new List<Texture2D>();
        List<Texture2D> InaAme = new List<Texture2D>();
        List<Texture2D> AmeIna = new List<Texture2D>();

        JustGura.Add(GuraFace);
        JustAme.Add(AmeFace);
        GuraAme.Add(GuraFace);
        GuraAme.Add(AmeFace);
        GuraIna.Add(GuraFace);
        GuraIna.Add(InaFace);
        InaAme.Add(InaFace);
        InaAme.Add(AmeFace);
        AmeGura.Add(AmeFace);
        AmeGura.Add(GuraFace);
        InaGura.Add(InaFace);
        InaGura.Add(GuraFace);
        AmeIna.Add(AmeFace);
        AmeIna.Add(InaFace);

        Panels.Enqueue(new Panel(
            "Ina",
            new List<string> { "So did Ame ever tell you when your adventure would be over?" },
            InaGura
        ));
        Panels.Enqueue(new Panel(
            "Gura",
            new List<string> { "Nah, I figured she'd pop up at some point to", "tell me I was done or something." },
            GuraIna
        ));
        Panels.Enqueue(new Panel(
            "Gura",
            new List<string> { "She still hasn’t shown up though, and I've helped you,", "Calli and Kiara already!" },
            GuraIna
        ));
        Panels.Enqueue(new Panel(
            "Gura",
            new List<string> { "I thought for sure she’d have shown up to mess with me by now." },
            GuraIna
        ));
        Panels.Enqueue(new Panel(
            "Ame",
            new List<string> { "That's because you're too slow sharky.", "I've been waiting here for ages." },
            AmeGura
        ));
        Panels.Enqueue(new Panel(
            "Gura",
            new List<string> { "Gah, not again!" },
            GuraAme
        ));
        Panels.Enqueue(new Panel(
            "Ame",
            new List<string> { "Heh, never gets old. Anyway, good job!" },
            AmeGura
        ));
        Panels.Enqueue(new Panel(
            "Ame",
            new List<string> { "You found and helped everyone I asked you to.", "Woooo, Good job!!" },
            AmeGura
        ));
        Panels.Enqueue(new Panel(
            "Gura",
            new List<string> { "You asked me to help people?" },
            GuraAme
        ));
        Panels.Enqueue(new Panel(
            "Ame",
            new List<string> { "Obviously dummy. What else would you be doing", "on an adventure?" },
            AmeGura
        ));
        Panels.Enqueue(new Panel(
            "Gura",
            new List<string> { "Oh, good point." },
            GuraAme
        ));
        Panels.Enqueue(new Panel(
            "Ame",
            new List<string> { "As a reward for your good deeds, you get the chance", "to regain your memories!" },
            AmeGura
        ));
        Panels.Enqueue(new Panel(
            "Gura",
            new List<string> { "Yeah, let's go! ...How am I gonna do that?" },
            GuraAme
        ));
        Panels.Enqueue(new Panel(
            "Ame",
            new List<string> { "Oh, just drink this." },
            AmeGura
        ));
        Panels.Enqueue(new Panel(
            "Gura",
            new List<string> { "Okee Doke! Down the hatch." },
            GuraAme
        ));
        Panels.Enqueue(new Panel(
            "Gura",
            new List<string> { "*glug glug*" },
            JustGura 
        ));
        Panels.Enqueue(new Panel(
            "Gura",
            new List<string> { "Ewww, why did that taste so gross?" },
            JustGura
        ));
        Panels.Enqueue(new Panel(
            "Gura",
            new List<string> { "Why am I feeling so... Sleepy...?" },
            JustGura
        ));
        Panels.Enqueue(new Panel(
            "Ina",
            new List<string> { "Ame, didn’t Koyo-chan give you that like a year ago." },
            InaAme
        ));
        Panels.Enqueue(new Panel(
            "Ame",
            new List<string> { "Don’t worry about it Ina, I went back and took it from the", "me that got it from her the day after. I'm not just a detective after all!" },
            AmeIna
        ));
        Panels.Enqueue(new Panel(
            "Ame",
            new List<string> { "Oh, and Goomba, I hope you didn’t reset too", "much! Good Luck, Sweet Dreams!" },
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
        LoadingScreen.Load(ScreenManager, true, playerIndex,
                     new StageThreeBackgroundScreen(),
                     new StageThreeGameScreen("Content/Beatmaps/Reflect/Reflect.json"));
    } 


    /// <summary>
    /// Draws the background screen.
    /// </summary>
    public override void Draw(GameTime gameTime) {

        base.Draw(gameTime);  // base dialogue screen handles drawing everything
    }
    #endregion
}