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
    float groundWidth;

    Quad leftWall;
    Quad rightWall;

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

        groundScrollX = 0;
        groundScrollY = 0;

        Viewport viewport = ScreenManager.GraphicsDevice.Viewport;

        backgroundTexture = content.Load<Texture2D>("background");
        groundTexture = content.Load<Texture2D>("GameplayAssets/Background/cobblestone_3");
        gradientTexture = content.Load<Texture2D>("gradient");

        basicEffect = new BasicEffect(ScreenManager.GraphicsDevice);
        basicEffect.TextureEnabled = true;
        basicEffect.Texture = groundTexture;
        basicEffect.World = GameplayTransforms.GetWorldMatrix(viewport.Height);
        basicEffect.View = GameplayTransforms.GetViewMatrix();
        basicEffect.Projection = GameplayTransforms.GetProjectionMatrix();
        basicEffect.FogEnabled = true;
        basicEffect.FogColor = Color.CornflowerBlue.ToVector3();
        basicEffect.FogStart = 0.1f;
        basicEffect.FogEnd = GameConstants.NOTE_HORIZON_DISTANCE - 1000f;

        groundWidth = groundTexture.Width * 4;

        float wallWidth = GameConstants.NOTE_HORIZON_DISTANCE;
        float wallHeight = viewport.Height * 4;

        leftWall = new Quad(
            new Vector3(-(groundWidth / 2), wallWidth / 2, wallHeight / 2),
            new Vector3(1, 0, 0),
            new Vector3(0, 0, 1),
            wallWidth,
            wallHeight
        );

        rightWall = new Quad(
            new Vector3(groundWidth / 2, wallWidth / 2, wallHeight / 2),
            new Vector3(-1, 0, 0),
            new Vector3(0, 0, 1),
            wallWidth,
            wallHeight
        );
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

        // scroll walls
        // dont ask me why, I just lucked on this number and it's perfect for this texture for now
        // will need to actually do the math to figure out how to get this to correlate to whatever we choose
        leftWall.SetTextureCoords(
            new Vector2(groundScrollY/10000, 0f),
            1f,
            1f
        );
        rightWall.SetTextureCoords(
            new Vector2(-groundScrollY/10000, 0f),
            1f,
            1f
        );
    }


    /// <summary>
    /// Draws the background screen.
    /// </summary>
    public override void Draw(GameTime gameTime) {
        ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.CornflowerBlue, 0, 0);

        SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
        Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
        Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);

        // sampler wraps textures to scroll evenly
        ScreenManager.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
        foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes) {
            pass.Apply();

            // draw walls
            ScreenManager.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(
                PrimitiveType.TriangleList,
                leftWall.Vertices, 0, 4,
                leftWall.Indices, 0, 2
            );

            ScreenManager.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(
                PrimitiveType.TriangleList,
                rightWall.Vertices, 0, 4,
                rightWall.Indices, 0, 2
            );
        }

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
            new Vector2(-(groundWidth / 2), 0),
            new Rectangle((int) this.groundScrollX, (int) this.groundScrollY, (int)groundWidth, GameConstants.NOTE_HORIZON_DISTANCE),
            Color.White
        );
        spriteBatch.End();
    }
    #endregion
}