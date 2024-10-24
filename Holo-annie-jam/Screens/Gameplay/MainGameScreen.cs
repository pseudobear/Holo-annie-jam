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
    const int NOTE_WIDTH = 500;
    const int NOTE_HEIGHT = 1000;
    const int NOTE_HORIZON_DISTANCE = 10000;
    BeatmapPlayer beatmapPlayer;
    Beatmap beatmap;
    string beatmapFilename;
    VertexDeclaration vertexDeclaration;

    // rhythm events 
    VisibleBeatmapEvents visibleEvents;
    Dictionary<RhythmEvent, Quad> rhythmQuadMap = new Dictionary<RhythmEvent, Quad>();

    Vector2 playerPosition = new Vector2(100, 100);
    Vector2 enemyPosition = new Vector2(100, 100);

    Random random = new Random();

    float pauseAlpha;

    // 3d graphics processing
    BasicEffect uprightObjectEffect;

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

        this.beatmap = Beatmap.LoadFromFile(beatmapFilename);
        this.beatmapPlayer = new BeatmapPlayer(beatmap);

        // generate sample beatmap
        //Beatmap.Builder builder = new("Sample Song", "Content/Beatmaps/Sample Beatmap/sample_song.ogg");
        //builder.RhythmEvents.Add(new RhythmEvent() {
        //    Tick = 0,
        //    Type = RhythmEventType.BpmChange,
        //    Value = 144
        //});
        //builder.RhythmEvents.Add(new RhythmEvent() {
        //    Tick = 3840,
        //    Type = RhythmEventType.Normal,
        //    Lane = 1
        //});
        //builder.RhythmEvents.Add(new RhythmEvent() {
        //    Tick = 3840,
        //    Type = RhythmEventType.Normal,
        //    Lane = 3
        //});
        //builder.RhythmEvents.Add(new RhythmEvent() {
        //    Tick = 4800,
        //    Type = RhythmEventType.Normal,
        //    Lane = 1
        //});
        //builder.RhythmEvents.Add(new RhythmEvent() {
        //    Tick = 5760,
        //    Type = RhythmEventType.Normal,
        //    Lane = 1
        //});
        //builder.RhythmEvents.Add(new RhythmEvent() {
        //    Tick = 6240,
        //    Type = RhythmEventType.Normal,
        //    Lane = 3
        //});
        //builder.WriteToFile("sample_beatmap_builder.json");
        //builder.Build().WriteToFile("sample_beatmap.bin");

        // transform setups

        Viewport viewport = ScreenManager.GraphicsDevice.Viewport;

        uprightObjectEffect = new BasicEffect(ScreenManager.GraphicsDevice);

        Vector3 cameraPosition = new Vector3(0f, -3000f, 1000f);
        Vector3 cameraTarget = new Vector3(0.0f, 0.0f, 0.0f); // Look back at the origin

        float fovAngle = MathHelper.ToRadians(75);
        float aspectRatio = 4 / 3;
        float near = 0.01f; // the near clipping plane distance
        float far = 10000f; // the far clipping plane distance

        // y+ is forward, x+ is right, z+ is up, try to get world's y=0 at bottom of screen
        Matrix world = Matrix.CreateTranslation(0.0f, -(viewport.Height) - 1600, 0.0f);
        Matrix view = Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.Up);
        Matrix projection = Matrix.CreatePerspectiveFieldOfView(fovAngle, aspectRatio, near, far);
        uprightObjectEffect.World = world;
        uprightObjectEffect.View = view;
        uprightObjectEffect.Projection = projection;
        uprightObjectEffect.TextureEnabled = true;
        uprightObjectEffect.Texture = note;

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
    /// Creates a Quad for holding an enemy sprite starting from NOTE_HORIZON_DISTANCE
    /// Note that we want to draw our enemnies upright, so normal is y-, up is z+, which contradicts vector3's stuff
    /// </summary>
    private Quad MakeNewEnemyQuad(uint lane, int distanceBetweenLanes) {
        int x = (int)(lane - 2) * (int)distanceBetweenLanes;
        return new Quad(
            new Vector3(x, NOTE_HORIZON_DISTANCE, 0), 
            new Vector3(0, -1, 0), 
            new Vector3(0, 0, 1), 
            NOTE_WIDTH, 
            NOTE_HEIGHT
        );
    }

    #endregion

    #region Update and Draw


    /// <summary>
    /// Updates the state of the game. This method checks the GameScreen.IsActive
    /// property, so the game will stop updating when the pause menu is active,
    /// or if you tab away to a different application.
    /// </summary>
    public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                    bool coveredByOtherScreen) {
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
                rhythmQuadMap[rhythmEvent].Origin = new Vector3(
                    rhythmQuadMap[rhythmEvent].Origin.X,
                    (float)(NOTE_HORIZON_DISTANCE - Math.Round(relativeY * NOTE_HORIZON_DISTANCE)),
                    rhythmQuadMap[rhythmEvent].Origin.Z
                );
            } else {
                rhythmQuadMap.Add(rhythmEvent, MakeNewEnemyQuad(rhythmEvent.Lane, ScreenManager.GraphicsDevice.Viewport.Width));
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
            // Otherwise move the player position.
            Vector2 movement = Vector2.Zero;

            if (keyboardState.IsKeyDown(Keys.Left))
                movement.X--;

            if (keyboardState.IsKeyDown(Keys.Right))
                movement.X++;

            if (keyboardState.IsKeyDown(Keys.Up))
                movement.Y--;

            if (keyboardState.IsKeyDown(Keys.Down))
                movement.Y++;

            Vector2 thumbstick = gamePadState.ThumbSticks.Left;

            movement.X += thumbstick.X;
            movement.Y -= thumbstick.Y;

            if (movement.Length() > 1)
                movement.Normalize();

            playerPosition += movement * 2;
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

        foreach (EffectPass pass in uprightObjectEffect.CurrentTechnique.Passes) {
            pass.Apply();

            foreach (KeyValuePair<RhythmEvent, Quad> entry in rhythmQuadMap) {
                ScreenManager.GraphicsDevice.DrawUserIndexedPrimitives
                    <VertexPositionNormalTexture>(
                    PrimitiveType.TriangleList,
                    entry.Value.Vertices, 0, 4,
                    entry.Value.Indices, 0, 2);

                /* list all of the coordinates in the quad
                System.Diagnostics.Debug.WriteLine("-------------------------------------------");
                foreach (var v in entry.Value.Vertices) {
                    System.Diagnostics.Debug.WriteLine(v);
                }
                */ 
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