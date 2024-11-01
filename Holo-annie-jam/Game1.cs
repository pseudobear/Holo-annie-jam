using ManagedBass;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

public class Game1 : Game {
    // hacky way to ensure we get these working for now - persists while game is open
    public static int Score = 0;
    public static int Corruption = 0;

    private GraphicsDeviceManager _graphics;
    private ScreenManager _screenManager;
    private SpriteBatch _spriteBatch;

    public Game1() {
        _graphics = new GraphicsDeviceManager(this);
        _screenManager = new ScreenManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        Components.Add(_screenManager);

        // start in main menu for now
        _screenManager.AddScreen(new MenuBackgroundScreen(), null);
        _screenManager.AddScreen(new MainMenuScreen(), null);
    }

    protected override void Initialize() {
        _graphics.IsFullScreen = false;
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
        _graphics.PreferMultiSampling = true;
        _graphics.ApplyChanges();

        if (!Bass.Init()) {
            throw new ContentLoadException("failed to initialize audio manager");
        }

        base.Initialize();
    }

    protected override void Draw(GameTime gameTime) {
        base.Draw(gameTime);
    }
}