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
class PreStageFourDialogueScreen : DialogueScreen {

    #region Initialization


    /// <summary>
    /// Constructor.
    /// </summary>
    public PreStageFourDialogueScreen() : base() {}


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
            "Gura",
            new List<string> { "Ugh, Ame, did you get the plate of the whale that hit me?" },
            GuraAme
        ));
        Panels.Enqueue(new Panel(
            "Ame",
            new List<string> { "Nope." },
            AmeGura
        ));
        Panels.Enqueue(new Panel(
            "Gura",
            new List<string> { "Darn. Eurgh, what did Koyori even put in that potion? I can still taste it!" },
            GuraAme
        ));
        Panels.Enqueue(new Panel(
            "Ina",
            new List<string> { "I think she mentioned finding an old stash of AsaCoco products." },
            InaGura
        ));
        Panels.Enqueue(new Panel(
            "Gura",
            new List<string> { "Ohhhh. That makes sense." },
            GuraIna
        ));
        Panels.Enqueue(new Panel(
            "Ame",
            new List<string> { "Anyway, congratulations on getting your memories back!" },
            AmeGura
        ));
        Panels.Enqueue(new Panel(
            "Gura",
            new List<string> { "Huh? Oh right, that's what we were doing, I forgot about that." },
            GuraAme
        ));
        Panels.Enqueue(new Panel(
            "Gura",
            new List<string> { "Yeah!!!!" },
            JustGura
        ));
        Panels.Enqueue(new Panel(
            "Ame",
            new List<string> { "Eh, it's ok, the plot was kinda rushed anyway." },
            AmeGura
        ));
        Panels.Enqueue(new Panel(
            "Ina",
            new List<string> { "Hey, you shouldn't say that out loud Ame." },
            InaAme
        ));
        Panels.Enqueue(new Panel(
            "Ame",
            new List<string> { "Oops, my bad. Chat probably figured it out a while ago anyway." },
            AmeIna
        ));
        Panels.Enqueue(new Panel(
            "Gura",
            new List<string> { "Also, weren't Calli and Kiara supposed to be here too?", "Why didn't they come with me like Ina did?" },
            GuraAme
        ));
        Panels.Enqueue(new Panel(
            "Gura",
            new List<string> { "And, wasn't there supposed to be a final boss or something?" },
            GuraAme
        ));
        Panels.Enqueue(new Panel(
            "Ame",
            new List<string> { "Stop asking questions Gura! The others will show up at some point!" },
            AmeGura
        ));
        Panels.Enqueue(new Panel(
            "Ame",
            new List<string> { "Look, a suspiciously evil corporation that's trying", "to take our subscribers! After them!" },
            AmeGura
        ));
        Panels.Enqueue(new Panel(
            "Gura",
            new List<string> { "Ooh, where?" },
            JustGura
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
                     new StageFourBackgroundScreen(),
                     new StageFourGameScreen("Content/Beatmaps/NonFiction/NonFiction.json"));
    } 


    /// <summary>
    /// Draws the background screen.
    /// </summary>
    public override void Draw(GameTime gameTime) {

        base.Draw(gameTime);  // base dialogue screen handles drawing everything
    }
    #endregion
}