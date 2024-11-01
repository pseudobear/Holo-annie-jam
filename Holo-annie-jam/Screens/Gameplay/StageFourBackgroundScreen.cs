#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
#endregion

/// <summary>
/// The background screen sits behind all the other menu screens.
/// It draws a background image that remains fixed in place regardless
/// of whatever transitions the screens on top of it may be doing.
/// </summary>
class StageFourBackgroundScreen : GameScreen {
    #region Fields

    ContentManager content;
    Texture2D groundTexture;
    TextureSheet wallSheet;
    BasicEffect basicEffect;

    float groundScrollX;
    float groundScrollY;
    float groundWidth;
    float wallWidth;
    float wallHeight;

    List<Quad> leftWall = new List<Quad>();
    List<Quad> rightWall = new List<Quad>();
    Quad background;

    #endregion

    #region Initialization


    /// <summary>
    /// Constructor.
    /// </summary>
    public StageFourBackgroundScreen() : base() {
        TransitionOnTime = TimeSpan.FromSeconds(1.5);
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

        groundTexture = content.Load<Texture2D>("GameplayAssets/Background/cobblestone_3");
        wallSheet = new TextureSheet(content.Load<Texture2D>("Beatmaps/NonFiction/Assets/Non_Fiction_Wall_Background"), StageFour.NUM_WALLS, 1);

        basicEffect = new BasicEffect(ScreenManager.GraphicsDevice);
        basicEffect.TextureEnabled = true;
        basicEffect.Texture = wallSheet.Texture;
        basicEffect.World = GameplayTransforms.GetWorldMatrix(viewport.Height);
        basicEffect.View = GameplayTransforms.GetViewMatrix();
        basicEffect.Projection = GameplayTransforms.GetProjectionMatrix();
        basicEffect.FogEnabled = true;
        basicEffect.FogColor = StageFour.BackgroundColor.ToVector3();
        basicEffect.FogStart = 0.1f;
        basicEffect.FogEnd = GameConstants.NOTE_HORIZON_DISTANCE;

        groundWidth = groundTexture.Width * 5;

        wallWidth = GameConstants.NOTE_HORIZON_DISTANCE;
        wallHeight = viewport.Height * 9;

        background = new Quad(
            new Vector3(0, GameConstants.NOTE_HORIZON_DISTANCE - 1200f, (wallHeight / 2) + 500f),
            new Vector3(0, -1, 0),
            new Vector3(0, 0, 1),
            groundWidth,
            wallHeight,
            new Vector2(0, (float) wallSheet[2].Y / (float) wallSheet.Height),
            (float) wallSheet[2].Width / (float) wallSheet.Width,
            (float) wallSheet[2].Height / (float) wallSheet.Height
        );

        for (int i = 0; i < StageFour.NUM_WALLS; i++) {
            leftWall.Add(new Quad(
                new Vector3(-(groundWidth / 2) + StageFour.WALL_OFFSET * i, wallWidth / 2, wallHeight / 2),
                new Vector3(1, 0, 0),
                new Vector3(0, 0, 1),
                wallWidth,
                wallHeight
            ));

            rightWall.Add(new Quad(
                new Vector3((groundWidth / 2) - StageFour.WALL_OFFSET * i, wallWidth / 2, wallHeight / 2),
                new Vector3(-1, 0, 0),
                new Vector3(0, 0, 1),
                wallWidth,
                wallHeight
            ));
        }
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

        // Scroll ground (and everything else)
        groundScrollY += 40f;

        // scroll walls
        for (int i = 0; i < StageFour.NUM_WALLS; i++) {
            leftWall[i].SetTextureCoords(
                new Vector2(
                    (groundScrollY / (15 + 5 * (StageFour.NUM_WALLS - i - 1)) + wallSheet[i].Width) / wallSheet.Width,
                    ((float)wallSheet[i].Y / (float)wallSheet.Height)
                ),
                (float)wallSheet[i].Width / (float)wallSheet.Width,
                (float)wallSheet[i].Height / (float)wallSheet.Height
            );

            rightWall[i].SetTextureCoords(
                new Vector2(
                    -(groundScrollY / (15 + 5 * (StageFour.NUM_WALLS - i - 1)) + wallSheet[i].Width) / wallSheet.Width,
                    ((float)wallSheet[i].Y / (float)wallSheet.Height)
                ),
                (float)wallSheet[i].Width / (float)wallSheet.Width,
                (float)wallSheet[i].Height / (float)wallSheet.Height
            );
        }
    }


    /// <summary>
    /// Draws the background screen.
    /// </summary>
    public override void Draw(GameTime gameTime) {
        ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, StageFour.BackgroundColor, 0, 0);

        SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
        Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
        Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);

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
            new Rectangle((int) this.groundScrollX, (int) this.groundScrollY, (int) groundWidth, GameConstants.NOTE_HORIZON_DISTANCE),
            Color.White
        );
        spriteBatch.End();

        // sampler wraps textures to scroll evenly
        ScreenManager.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
        foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes) {
            pass.Apply();

            // draw background
            ScreenManager.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(
                PrimitiveType.TriangleList,
                background.Vertices, 0, 4,
                background.Indices, 0, 2
            );

            // draw walls
            for (int i = 1; i < StageFour.NUM_WALLS; i++) {
                // if (i != 1) continue;
                ScreenManager.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(
                    PrimitiveType.TriangleList,
                    leftWall[i].Vertices, 0, 4,
                    leftWall[i].Indices, 0, 2
                );

                ScreenManager.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(
                    PrimitiveType.TriangleList,
                    rightWall[i].Vertices, 0, 4,
                    rightWall[i].Indices, 0, 2
                );
            }
        }
    }
    #endregion
}