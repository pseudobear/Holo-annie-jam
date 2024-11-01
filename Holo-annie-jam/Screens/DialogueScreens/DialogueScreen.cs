#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
        /*
        return (PanelConstants.GetTextPanelOrigin() + 
            Vector2.UnitX * (
                (PanelConstants.TEXT_PANEL_WIDTH * (index + 1)) / (1 + sprites.Count) - 
                sprites[index].Width / 2
            ) + Vector2.UnitY * PanelConstants.TEXT_PANEL_HEIGHT
        );
        */
        return new Vector2(
            ((2000 * (index + 1) / (1 + sprites.Count)) - PanelConstants.SPRITE_WIDTH / 2) - 360, 
            720 - PanelConstants.SPRITE_HEIGHT
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

    public Queue<Panel> Panels {
        get { return panels; }
    }
    Queue<Panel> panels = new Queue<Panel>();

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
    /// Lets the game respond to player input. Unlike the Update method,
    /// this will only be called when the gameplay screen is active.
    /// </summary>
    public override void HandleInput(InputManager input) {
        if (input == null)
            throw new ArgumentNullException("input");

        KeyboardState keyboardState = input.CurrentKeyboardStates[(int)ControllingPlayer.Value];
        GamePadState gamePadState = input.CurrentGamePadStates[(int)ControllingPlayer.Value];

        // The game pauses either if the user presses the pause button, or if
        // they unplug the active gamepad. This requires us to keep track of
        // whether a gamepad was ever plugged in, because we don't want to pause
        // on PC if they are playing with a keyboard and have no gamepad at all!
        bool gamePadDisconnected = !gamePadState.IsConnected &&
                                    input.GamePadWasConnected[(int)ControllingPlayer.Value];

        PlayerIndex playerIndex;

        if (input.IsMenuSelect(ControllingPlayer, out playerIndex)) {
            if (Panels.Count > 1) {
                Panels.Dequeue();
            }
            else {
                OnCompletePanels(ControllingPlayer);
            }
        }

        if (input.IsPauseGame(ControllingPlayer) || gamePadDisconnected) {
            ScreenManager.AddScreen(new PauseMenuScreen(() => {}), ControllingPlayer);
        }
        else {
            // Otherwise do game stuff:
        }
    }

    public virtual void OnCompletePanels(PlayerIndex? playerIndex) { }

    /// <summary>
    /// Draws the dialogue screen. Text, sprites and background
    /// </summary>
    public override void Draw(GameTime gameTime) {
        SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
        SpriteFont font = ScreenManager.Font;
        Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
        Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);

        Panel activePanel = Panels.Peek();

        spriteBatch.Begin();

        for (int i = 0; i < activePanel.Sprites.Count; i++) {
            spriteBatch.Draw(
                activePanel.Sprites[i],
                new Rectangle(
                    (int)activePanel.GetSpriteOrigin(i).X,
                    (int)activePanel.GetSpriteOrigin(i).Y,
                    PanelConstants.SPRITE_WIDTH,
                    PanelConstants.SPRITE_HEIGHT
                ),
                new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha)
            );
        }
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
        spriteBatch.DrawString(
            font, 
            activePanel.Name, 
            PanelConstants.GetTextPanelOrigin(),
            Color.White
        );
        spriteBatch.DrawString(
            font, 
            activePanel.Text, 
            PanelConstants.GetTextPanelOrigin() + Vector2.UnitY * 100,
            Color.White
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