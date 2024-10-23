#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
#endregion

/// <summary>
/// The background screen sits behind all the other menu screens.
/// It draws a background image that remains fixed in place regardless
/// of whatever transitions the screens on top of it may be doing.
/// </summary>
class GameplayBackgroundScreen : GameScreen {
    #region Fields

    ContentManager content;
    Texture2D backgroundTexture;
    Texture2D groundTexture;
    Texture2D gradientTexture;
    BasicEffect basicEffect;

    float groundScrollX;
    float groundScrollY;

    #endregion

    #region Initialization


    /// <summary>
    /// Constructor.
    /// </summary>
    public GameplayBackgroundScreen() : base() {
        TransitionOnTime = TimeSpan.FromSeconds(0.5);
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

        Viewport viewport = ScreenManager.GraphicsDevice.Viewport;

        backgroundTexture = content.Load<Texture2D>("background");
        groundTexture = content.Load<Texture2D>("GameplayAssets/Background/cobblestone_3");
        gradientTexture = content.Load<Texture2D>("gradient");

        basicEffect = new BasicEffect(ScreenManager.GraphicsDevice) {
            TextureEnabled = true,
            VertexColorEnabled = true,
        };

        Vector3 cameraPosition = new Vector3(0f, -3000f, 1000f);
        Vector3 cameraTarget = new Vector3(0.0f, 0.0f, 0.0f); // Look back at the origin

        float fovAngle = MathHelper.ToRadians(75);
        float aspectRatio = 4 / 3;
        float near = 0.01f; // the near clipping plane distance
        float far = 10000f; // the far clipping plane distance

        // y+ is forward, x+ is right, z+ is up, try to get y=0 at bottom of screen
        Matrix world = Matrix.CreateTranslation(0.0f, -(viewport.Height) - 1600, 0.0f);
        Matrix view = Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.Up);
        Matrix projection = Matrix.CreatePerspectiveFieldOfView(fovAngle, aspectRatio, near, far);

        basicEffect.World = world;
        basicEffect.View = view;
        basicEffect.Projection = projection;
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
    /// Updates the background screen. Unlike most screens, this should not
    /// transition off even if it has been covered by another screen: it is
    /// supposed to be covered, after all! This overload forces the
    /// coveredByOtherScreen parameter to false in order to stop the base
    /// Update method wanting to transition off.
    /// </summary>
    public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                    bool coveredByOtherScreen) {
        base.Update(gameTime, otherScreenHasFocus, false);

        // Scroll ground
        groundScrollY += 40f;
    }


    /// <summary>
    /// Draws the background screen.
    /// </summary>
    public override void Draw(GameTime gameTime) {
        SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
        Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
        Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);

        spriteBatch.Begin();

        spriteBatch.Draw(backgroundTexture, fullscreen,
                            new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));

        spriteBatch.End();

        // Draw ground
        spriteBatch.Begin(
            sortMode: SpriteSortMode.Deferred,
            blendState: null,
            samplerState: SamplerState.LinearWrap,
            depthStencilState: null,
            rasterizerState: RasterizerState.CullNone,
            effect: basicEffect
        );
        spriteBatch.Draw(
            groundTexture,
            new Vector2(-groundTexture.Width, 0),
            new Rectangle((int) this.groundScrollX, (int) this.groundScrollY, groundTexture.Width * 2, groundTexture.Height * 5),
            Color.White
        );
        spriteBatch.End();
    }


    #endregion
}