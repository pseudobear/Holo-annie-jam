#region Using Statements
using Microsoft.Xna.Framework;
#endregion

static class PanelConstants {
    public const int TEXT_PANEL_WIDTH = 500;
    public const int TEXT_PANEL_HEIGHT = 1000;

    public static Vector2 GetTextPanelOrigin() {
        return new Vector2(200, 200);
    }
}