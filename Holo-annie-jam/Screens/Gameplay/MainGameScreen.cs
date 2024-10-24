#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Threading;
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
    static readonly float BOTTOM_WIDTH = BOTTOM_RIGHT.X - BOTTOM_LEFT.X;
    static readonly float TOP_WIDTH = TOP_RIGHT.X - TOP_LEFT.X;
    static readonly float WIDTH_DIFFERENCE = BOTTOM_WIDTH - TOP_WIDTH;
    static readonly float HEIGHT = BOTTOM_LEFT.Y - TOP_LEFT.Y;

    ContentManager content;
    SpriteFont gameFont;
    Texture2D note;
    const int NOTE_HALF_WIDTH = 50;
    const int NOTE_HALF_HEIGHT = 50;
    BeatmapPlayer beatmapPlayer;
    Beatmap beatmap;
    string beatmapFilename;

    Vector2 playerPosition = new Vector2(100, 100);
    Vector2 enemyPosition = new Vector2(100, 100);

    Random random = new Random();

    float pauseAlpha;

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

        // A real game would probably have more content than this sample, so
        // it would take longer to load. We simulate that by delaying for a
        // while, giving you a chance to admire the beautiful loading screen.
        Thread.Sleep(1000);

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

        // once the load has finished, we use ResetElapsedTime to tell the game's
        // timing mechanism that we have just finished a very long frame, and that
        // it should not try to catch up.
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

        if (IsActive) {
            // Apply some random jitter to make the enemy move around.
            const float randomization = 10;

            enemyPosition.X += (float) (random.NextDouble() - 0.5) * randomization;
            enemyPosition.Y += (float) (random.NextDouble() - 0.5) * randomization;

            // Apply a stabilizing force to stop the enemy moving off the screen.
            Vector2 targetPosition = new Vector2(
                ScreenManager.GraphicsDevice.Viewport.Width / 2 - gameFont.MeasureString("Insert Gameplay Here").X / 2,
                200);

            enemyPosition = Vector2.Lerp(enemyPosition, targetPosition, 0.05f);
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

        spriteBatch.Begin();

        Vector2 screen = new(ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height);

        //spriteBatch.DrawString(gameFont, "TOP LEFT", TOP_LEFT * screen, Color.Green);
        //spriteBatch.DrawString(gameFont, "TOP RIGHT", TOP_RIGHT * screen * 0.8f, Color.Green);
        //spriteBatch.DrawString(gameFont, "BOTTOM LEFT", BOTTOM_LEFT * screen * 0.8f, Color.Green);
        //spriteBatch.DrawString(gameFont, "BOTTOM RIGHT", BOTTOM_RIGHT * screen * 0.8f, Color.Green);

        VisibleBeatmapEvents visibleEvents = beatmapPlayer.GetVisibleEvents();
        long totalTicksVisible = beatmapPlayer.VisibleTimespanTicks + beatmapPlayer.TrailingTimespanTicks;
        long startingTickVisible = visibleEvents.Tick - beatmapPlayer.TrailingTimespanTicks;
        foreach (RhythmEvent rhythmEvent in visibleEvents.RhythmEvents ?? Array.Empty<RhythmEvent>()) {
            // number of lanes is currently hardcoded to 3: later, we should store this in the beatmap file

            double relativeX = (2 * rhythmEvent.Lane - 1) / 6.0;
            double relativeY = 1 - (double) (rhythmEvent.Tick - startingTickVisible) / totalTicksVisible;

            double widthAtLine = BOTTOM_WIDTH - (WIDTH_DIFFERENCE * relativeY);
            double edgeX = BOTTOM_LEFT.X + (WIDTH_DIFFERENCE * relativeY);
            int x = (int) Math.Round((widthAtLine * relativeX + edgeX) * screen.X);
            int y = (int) Math.Round((TOP_LEFT.Y + (relativeY * HEIGHT)) * screen.Y);
            // TODO zoom
            spriteBatch.Draw(note, new Rectangle(x - NOTE_HALF_WIDTH, y - NOTE_HALF_HEIGHT, 2 * NOTE_HALF_WIDTH, 2 * NOTE_HALF_HEIGHT), Color.White);
        }

        spriteBatch.End();

        // If the game is transitioning on or off, fade it out to black.
        if (TransitionPosition > 0 || pauseAlpha > 0) {
            float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

            ScreenManager.FadeBackBufferToBlack(alpha);
        }
    }


    #endregion
}