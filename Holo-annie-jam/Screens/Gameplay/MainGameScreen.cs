#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
#endregion

/// <summary>
/// This screen implements the actual game logic. It is just a
/// placeholder to get the idea across: you'll probably want to
/// put some more interesting gameplay in here!
/// </summary>
class MainGameScreen : GameScreen {
    #region Fields

    static readonly Vector2 TOP_LEFT = new(0, 0);
    static readonly Vector2 TOP_RIGHT = new(1, 0);
    static readonly Vector2 BOTTOM_LEFT = new(0, 1);
    static readonly Vector2 BOTTOM_RIGHT = new(1, 1);

    ContentManager content;
    SpriteFont gameFont;
    Texture2D note;
    Texture2D note_shadow;
    BeatmapPlayer beatmapPlayer;
    Beatmap beatmap;
    string beatmapFilename;
    VertexDeclaration vertexDeclaration;

    // rhythm events 
    VisibleBeatmapEvents visibleEvents;
    
    // [0] is enemy sprite, [1] is shadow
    Dictionary<RhythmEvent, List<Quad>> rhythmQuadMap = new Dictionary<RhythmEvent, List<Quad>>();

    float pauseAlpha;

    // 3d graphics processing
    BasicEffect uprightObjectEffect;
    BasicEffect shadowObjectEffect;

    #endregion

    #region Initialization


    /// <summary>
    /// Constructor.
    /// </summary>
    public MainGameScreen(string beatmapFilename) : base() {
        TransitionOnTime = TimeSpan.FromSeconds(1.5);
        TransitionOffTime = TimeSpan.FromSeconds(0.5);
        this.beatmapFilename = beatmapFilename;
    }


    /// <summary>
    /// Load graphics content for the game.
    /// </summary>
    public override void LoadContent() {
        if (content == null)
            content = new ContentManager(ScreenManager.Game.Services, "Content");

        gameFont = content.Load<SpriteFont>("gamefont");
        note = content.Load<Texture2D>("GameplayAssets/Beatmap Objects/note");
        note_shadow = content.Load<Texture2D>("GameplayAssets/Beatmap Objects/note_shadows");

        this.beatmap = Beatmap.LoadFromFile(beatmapFilename);
        this.beatmapPlayer = new BeatmapPlayer(beatmap);

        // transform setups

        Viewport viewport = ScreenManager.GraphicsDevice.Viewport;

        float enemyFogStart = GameConstants.NOTE_HORIZON_DISTANCE / 5;
        float enemyFogEnd = GameConstants.NOTE_HORIZON_DISTANCE - 1000f;

        uprightObjectEffect = new BasicEffect(ScreenManager.GraphicsDevice);
        uprightObjectEffect.World = GameplayTransforms.GetWorldMatrix(viewport.Height);
        uprightObjectEffect.View = GameplayTransforms.GetViewMatrix();
        uprightObjectEffect.Projection = GameplayTransforms.GetProjectionMatrix();
        uprightObjectEffect.TextureEnabled = true;
        uprightObjectEffect.Texture = note;
        uprightObjectEffect.FogEnabled = true;
        uprightObjectEffect.FogColor = Color.CornflowerBlue.ToVector3();
        uprightObjectEffect.FogStart = 0.1f;
        uprightObjectEffect.FogEnd = GameConstants.NOTE_HORIZON_DISTANCE - 1000f;

        // same as objectEffect, but shadow texture instead
        shadowObjectEffect = new BasicEffect(ScreenManager.GraphicsDevice);
        shadowObjectEffect.World = GameplayTransforms.GetWorldMatrix(viewport.Height);
        shadowObjectEffect.View = GameplayTransforms.GetViewMatrix();
        shadowObjectEffect.Projection = GameplayTransforms.GetProjectionMatrix();
        shadowObjectEffect.TextureEnabled = true;
        shadowObjectEffect.Texture = note_shadow;
        shadowObjectEffect.FogEnabled = true;
        shadowObjectEffect.FogColor = Color.CornflowerBlue.ToVector3();
        shadowObjectEffect.FogStart = enemyFogStart;
        shadowObjectEffect.FogEnd = enemyFogEnd;

        vertexDeclaration = new VertexDeclaration(new VertexElement[] {
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
                new VertexElement(24, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
            }
        );

        ScreenManager.Game.ResetElapsedTime();
    }

    public override void OnLoad() {
        this.beatmapPlayer.Start();
    }

    /// <summary>
    /// Unload graphics content used by the game.
    /// </summary>
    public override void UnloadContent() {
        content.Unload();
    }


    #endregion

    #region helpers

    /// <summary>
    /// Creates a Quad for holding an enemy sprite starting from GameConstants.NOTE_HORIZON_DISTANCE
    /// Note that we want to draw our enemnies upright, so normal is y-, up is z+, which contradicts vector3's stuff
    /// </summary>
    private Quad MakeNewEnemyQuad(uint lane, int distanceBetweenLanes) {
        int x = (int)(lane - 2) * (int)distanceBetweenLanes;
        return new Quad(
            new Vector3(x, GameConstants.NOTE_HORIZON_DISTANCE, (GameConstants.NOTE_HEIGHT / 2 ) + 0.001f),  // slightly above ground to avoid Z fighting with ground
            new Vector3(0, -1, 0), 
            new Vector3(0, 0, 1), 
            GameConstants.NOTE_WIDTH, 
            GameConstants.NOTE_HEIGHT
        );
    }

    /// <summary>
    /// Creates a Quad for holding an enemy sprite shadow based on starting position of MakeNewEnemyQuad (hardcoded)
    /// </summary>
    private Quad MakeNewEnemyQuadShadow(uint lane, int distanceBetweenLanes) {
        int x = (int)(lane - 2) * (int)distanceBetweenLanes;
        return new Quad(
            new Vector3(x, GameConstants.NOTE_HORIZON_DISTANCE + (GameConstants.SHADOW_MAX_LEN / 2), 0.001f),  // slightly above ground to avoid Z fighting with ground
            new Vector3(0, 0, 1), 
            new Vector3(0, 1, 0), 
            GameConstants.NOTE_WIDTH, 
            GameConstants.SHADOW_MAX_LEN
        );
    }

    #endregion

    #region Update and Draw


    /// <summary>
    /// Updates the state of the game. This method checks the GameScreen.IsActive
    /// property, so the game will stop updating when the pause menu is active,
    /// or if you tab away to a different application.
    /// </summary>
    public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen) {
        base.Update(gameTime, otherScreenHasFocus, false);

        // Gradually fade in or out depending on whether we are covered by the pause screen.
        if (coveredByOtherScreen)
            pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
        else
            pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

        // update visible events and assign quads to visible rhythm events
        visibleEvents = beatmapPlayer.GetVisibleEvents();
        long totalTicksVisible = beatmapPlayer.VisibleTimespanTicks + beatmapPlayer.TrailingTimespanTicks;
        long startingTickVisible = visibleEvents.Tick - beatmapPlayer.TrailingTimespanTicks;

        foreach (RhythmEvent rhythmEvent in visibleEvents.RhythmEvents ?? Array.Empty<RhythmEvent>()) { 
            double relativeY = 1 - (double) (rhythmEvent.Tick - startingTickVisible) / totalTicksVisible;

            if (rhythmQuadMap.ContainsKey(rhythmEvent)) {
                List<Quad> enemyElements = rhythmQuadMap[rhythmEvent];

                // update enemy sprite position
                enemyElements[0].Origin = new Vector3(
                    enemyElements[0].Origin.X,
                    (float)(
                        GameConstants.NOTE_HORIZON_DISTANCE - 
                        Math.Round(relativeY * GameConstants.NOTE_HORIZON_DISTANCE)
                    ),
                    enemyElements[0].Origin.Z
                );

                // update enemy shadow position and size 
                enemyElements[1].Height = (
                    (float)(relativeY * GameConstants.SHADOW_MIN_LEN) + 
                    (float)((1 - relativeY) * GameConstants.SHADOW_MAX_LEN)
                );
                enemyElements[1].Origin = new Vector3(
                    enemyElements[1].Origin.X,
                    (float)(
                        GameConstants.NOTE_HORIZON_DISTANCE + (enemyElements[1].Height / 2) - 
                        Math.Round(relativeY * GameConstants.NOTE_HORIZON_DISTANCE)
                    ),
                    enemyElements[1].Origin.Z
                );
            } else {
                List<Quad> enemyElements = new List<Quad>();
                enemyElements.Add(MakeNewEnemyQuad(rhythmEvent.Lane, ScreenManager.GraphicsDevice.Viewport.Width));
                enemyElements.Add(MakeNewEnemyQuadShadow(rhythmEvent.Lane, ScreenManager.GraphicsDevice.Viewport.Width));
                rhythmQuadMap.Add(rhythmEvent, enemyElements);
            }
        }

        // deload invisible events
        foreach (RhythmEvent rhythmEvent in rhythmQuadMap.Keys.ToList()) {
            if (!(visibleEvents.RhythmEvents ?? Array.Empty<RhythmEvent>()).Contains(rhythmEvent)) {
                rhythmQuadMap.Remove(rhythmEvent);
            }
        }
    }


    /// <summary>
    /// Lets the game respond to player input. Unlike the Update method,
    /// this will only be called when the gameplay screen is active.
    /// </summary>
    public override void HandleInput(InputManager input) {
        if (input == null)
            throw new ArgumentNullException("input");

        // Look up inputs for the active player profile.
        int playerIndex = (int) ControllingPlayer.Value;

        KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
        GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

        // The game pauses either if the user presses the pause button, or if
        // they unplug the active gamepad. This requires us to keep track of
        // whether a gamepad was ever plugged in, because we don't want to pause
        // on PC if they are playing with a keyboard and have no gamepad at all!
        bool gamePadDisconnected = !gamePadState.IsConnected &&
                                    input.GamePadWasConnected[playerIndex];

        if (input.IsPauseGame(ControllingPlayer) || gamePadDisconnected) {
            ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
        } else {
            // Otherwise do game stuff:
        }
    }


    /// <summary>
    /// Draws the gameplay screen.
    /// </summary>
    public override void Draw(GameTime gameTime) {
        // Clear handled by GameplayBackgroundScreen
        // ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.CornflowerBlue, 0, 0);

        SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
        Vector2 screen = new(ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height);

        // draw shadows on  objects
        foreach (EffectPass pass in shadowObjectEffect.CurrentTechnique.Passes) {
            pass.Apply();

            // TODO: draw these in correct order, may need to use ordered dictionary or maintain a stack of rhythmevents
            foreach (KeyValuePair<RhythmEvent, List<Quad>> entry in rhythmQuadMap) {
                ScreenManager.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(
                    PrimitiveType.TriangleList,
                    entry.Value[1].Vertices, 0, 4,
                    entry.Value[1].Indices, 0, 2
                );
            }
        }

        // draw upright objects
        foreach (EffectPass pass in uprightObjectEffect.CurrentTechnique.Passes) {
            pass.Apply();

            // TODO: draw these in correct order, may need to use ordered dictionary or maintain a stack of rhythmevents
            foreach (KeyValuePair<RhythmEvent, List<Quad>> entry in rhythmQuadMap) {
                ScreenManager.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(
                    PrimitiveType.TriangleList,
                    entry.Value[0].Vertices, 0, 4,
                    entry.Value[0].Indices, 0, 2
                );
            }
        }

        // If the game is transitioning on or off, fade it out to black.
        if (TransitionPosition > 0 || pauseAlpha > 0) {
            float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

            ScreenManager.FadeBackBufferToBlack(alpha);
        }
    }


    #endregion
}