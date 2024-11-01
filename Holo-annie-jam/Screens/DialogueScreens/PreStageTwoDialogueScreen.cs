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
            "Gura",
            new List<string> { "Wow, this cave is pretty spooky." },
            JustGura
        ));
        Panels.Enqueue(new Panel(
            "Gura",
            new List<string> { "Seems like the kinda place people do weird rituals." },
            JustGura
        )); 
        Panels.Enqueue(new Panel(
            "Ina",
            new List<string> { "Hey, my rituals aren't weird." },
            InaGura
        ));
        Panels.Enqueue(new Panel(
            "Gura",
            new List<string> { "Ahh, not again!" },
            GuraIna
        ));
        Panels.Enqueue(new Panel(
            "Ina",
            new List<string> { "Oh Gura! Long time no see!" },
            InaGura
        ));
        Panels.Enqueue(new Panel(
            "Gura",
            new List<string> { "Am I supposed to know you?" },
            GuraIna
        ));
        Panels.Enqueue(new Panel(
            "Ina",
            new List<string> { "Uhm, nope, I don't think so." },
            InaGura
        ));
        Panels.Enqueue(new Panel(
            "Gura",
            new List<string> { "Ok then. What are you doing in this cave?" },
            GuraIna
        ));
        Panels.Enqueue(new Panel(
            "Ina",
            new List<string> { "I have a quest for you!" },
            InaGura
        )); 
        Panels.Enqueue(new Panel(
            "Ina",
            new List<string> { "My friend AO-chan went missing somewhere", "in this cave, and I need your help to find them." },
            InaGura
        ));
        Panels.Enqueue(new Panel(
            "Gura",
            new List<string> { "Ooh, I can help you find them! What does", "AO-chan look like?" },
            GuraIna
        ));
        Panels.Enqueue(new Panel(
            "Ina",
            new List<string> { "AO-chan is the only thing in the cave that", "isn't a Takodachi." },
            InaGura
        ));
        Panels.Enqueue(new Panel(
            "Gura",
            new List<string> { "A what now?" },
            GuraIna
        ));
        Panels.Enqueue(new Panel(
            "Ina",
            new List<string> { "A takodachi. It looks like a purple slime.", "Now shoo, I need to keep drawing." },
            InaGura
        ));
        Panels.Enqueue(new Panel(
            "Gura",
            new List<string> { "Got it! I'll be right back!" },
            GuraIna
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