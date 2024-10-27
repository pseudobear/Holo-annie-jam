#region Using Statements
using Microsoft.Xna.Framework;
#endregion

static class PanelConstants {
    public const int TEXT_PANEL_WIDTH = 980;
    public const int TEXT_PANEL_HEIGHT = 250;

    public static Vector2 GetTextPanelOrigin() {
        return new Vector2(150, 450);
    }
}