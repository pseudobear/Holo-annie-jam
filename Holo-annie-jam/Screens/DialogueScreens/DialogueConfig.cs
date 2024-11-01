#region Using Statements
using Microsoft.Xna.Framework;
#endregion

static class PanelConstants {
    public const int TEXT_PANEL_WIDTH = 980;
    public const int TEXT_PANEL_HEIGHT = 250;

    public const int SPRITE_WIDTH = 550;
    public const int SPRITE_HEIGHT = 550;

    public static Vector2 GetTextPanelOrigin() {
        return new Vector2(150, 450);
    }
}