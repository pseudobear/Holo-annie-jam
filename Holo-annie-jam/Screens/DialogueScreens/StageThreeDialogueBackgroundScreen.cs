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
class StageThreeDialogueBackgroundScreen : GameScreen {
    #region Fields

    ContentManager content;
    TextureSheet wallSheet;
    BasicEffect basicEffect;

    float parallaxScroll;

    List<Quad> background = new List<Quad>();

    #endregion

    #region Initialization


    /// <summary>
    /// Constructor.
    /// </summary>
    public StageThreeDialogueBackgroundScreen() : base() {
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

        parallaxScroll = 0;

        Viewport viewport = ScreenManager.GraphicsDevice.Viewport;

        wallSheet = new TextureSheet(content.Load<Texture2D>("Beatmaps/Chikutaku/Assets/ChikuTaku_Wall_Background"), StageThree.NUM_WALLS + 1, 1);

        basicEffect = new BasicEffect(ScreenManager.GraphicsDevice);
        basicEffect.TextureEnabled = true;
        basicEffect.Texture = wallSheet.Texture;

        for (int i = 0; i < StageThree.NUM_WALLS; i++) {
            background.Add(new Quad(
                new Vector3(0, 0, 0),
                new Vector3(0, 0, 1),
                new Vector3(0, 1, 0),
                2.3f,
                2.3f
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
        parallaxScroll += 5f;

        // scroll wall
        for (int i = 0; i < StageThree.NUM_WALLS; i++) {
            background[i].SetTextureCoords(
                new Vector2(
                    (parallaxScroll / (15 + 5 * (StageThree.NUM_WALLS - i - 1)) + wallSheet[i].Width) / wallSheet.Width,
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
        ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, StageThree.BackgroundColor, 0, 0);

        SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
        Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
        Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);

        // sampler wraps textures to scroll evenly
        ScreenManager.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
        foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes) {
            pass.Apply();

            // draw background
            for (int i = 0; i < StageThree.NUM_WALLS; i++) {
                // if (i != 1) continue;
                ScreenManager.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(
                    PrimitiveType.TriangleList,
                    background[i].Vertices, 0, 4,
                    background[i].Indices, 0, 2
                );
            }
        }
    }
    #endregion
}