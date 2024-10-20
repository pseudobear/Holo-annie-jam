using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class Game1 : Game
{
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
		_screenManager.AddScreen(new MainMenuScreen(), null);
	}

	protected override void Initialize() {
		// TODO: Add your initialization logic here

		base.Initialize();
	}

	protected override void Draw(GameTime gameTime) {
		GraphicsDevice.Clear(Color.CornflowerBlue);

		// this is just refresh, ScreenManager does all the drawing
		base.Draw(gameTime);
	}
}