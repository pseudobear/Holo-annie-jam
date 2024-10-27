#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
#endregion

class Panel {

    public string Name {
        get { return name; }
        set { name = value; }
    }
    string name;

    public string Text {
        get { return text; }
        set { text = value; }
    }
    string text;

    public List<Texture2D> Sprites {
        get { return sprites; }
        set { sprites = value; }
    }
    List<Texture2D> sprites;

    public Panel(string name, string text, List<Texture2D> sprites) {
        this.name = name;
        this.text = text;
        this.sprites = sprites;
    }
    
    /// <summary>
    /// Grabs the location vector of the sprite at 'index' in the sprites list 
    /// This is calculated to properly space out the sprites depending on how many there are
    /// </summary>
    public Vector2 GetSpriteOrigin(int index) {
        return (PanelConstants.GetTextPanelOrigin() + 
            Vector2.UnitX * (
                (PanelConstants.TEXT_PANEL_WIDTH * (index + 1)) / (1 + sprites.Count) - 
                sprites[index].Width / 2
            ) + Vector2.UnitY * PanelConstants.TEXT_PANEL_HEIGHT
        );
    }
}

/// <summary>
/// A screen that contains all of the functions needed for dialogue. Draws a text panel with
/// the contents specified by the panels stack, user inputs pop the panels out of the stack until
/// the last one, which triggers a load onto the next screen
/// </summary>
class DialogueScreen : GameScreen {
    #region Fields

    Texture2D blank;
    float pauseAlpha;

    #endregion

    #region Properties

    public ContentManager Content {
        get { return content; }
    }
    ContentManager content;

    public Texture2D BackgroundTexture { 
        get { return backgroundTexture; } 
        set { backgroundTexture = value; }
    }
    Texture2D backgroundTexture;

    public Stack<Panel> Panels {
        get { return panels; }
    }
    Stack<Panel> panels;

    #endregion

    #region Initialization


    /// <summary>
    /// Constructor.
    /// </summary>
    public DialogueScreen() : base() {
        TransitionOnTime = TimeSpan.FromSeconds(2);
        TransitionOffTime = TimeSpan.FromSeconds(0.5);
    }


    /// <summary>
    /// Loads graphics content for this screen. The background texture is quite
    /// big, so we use our own local ContentManager to load it. This allows us
    /// to unload before going from the menus into the game itself, wheras if we
    /// used the shared ContentManager provided by the Game class, the content
    /// would remain loaded forever.
    /// </summary>
    public override void LoadContent() {
        if (content == null)
            content = new ContentManager(ScreenManager.Game.Services, "Content");

        blank = content.Load<Texture2D>("blank");
    }


    /// <summary>
    /// Unloads graphics content for this screen.
    /// </summary>
    public override void UnloadContent() {
        content.Unload();
    }


    #endregion

    #region Update and Draw


    /// <summary>
    /// Updates the dialogue screen.
    /// </summary>
    public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                    bool coveredByOtherScreen) {
        base.Update(gameTime, otherScreenHasFocus, false);

        // Gradually fade in or out depending on whether we are covered by the pause screen.
        if (coveredByOtherScreen)
            pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
        else
            pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);
    }


    /// <summary>
    /// Draws the dialogue screen. Text, sprites and background
    /// </summary>
    public override void Draw(GameTime gameTime) {
        ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.CornflowerBlue, 0, 0);

        SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
        Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
        Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);

        spriteBatch.Begin();
        spriteBatch.Draw(backgroundTexture, fullscreen,
                            new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));
        spriteBatch.Draw(
            blank, 
            new Rectangle(
                (int)PanelConstants.GetTextPanelOrigin().X,
                (int)PanelConstants.GetTextPanelOrigin().Y,
                PanelConstants.TEXT_PANEL_WIDTH,
                PanelConstants.TEXT_PANEL_HEIGHT
            ), 
            Color.Black * 0.6f
        );
        spriteBatch.End();

        // If the game is transitioning on or off, fade it out to black.
        if (TransitionPosition > 0 || pauseAlpha > 0) {
            float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

            ScreenManager.FadeBackBufferToBlack(alpha);
        }
    }
    #endregion
}