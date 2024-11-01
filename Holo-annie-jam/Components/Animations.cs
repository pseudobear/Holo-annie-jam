#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

/// <summary>
/// Wrappers around Texture2D which allows us to access certain "zones" on the texture.
/// We allow for columns, rows, or grids where we read indices left to right, up and down, returning the
/// texture corner floats to be used in Quad
/// </summary>


/// <summary>
/// |0      |1       |2       |
/// |       |        |        |
/// |-------|--------|--------|
/// |3      |4       |5...    |
/// |       |        |        |
/// </summary>
class TextureSheet {

    #region fields
    Texture2D texture;
    int numRows;
    int numCols;
    float zoneWidth;
    float zoneHeight;
    #endregion

    public Rectangle this[int key] {
        get => GetZone(key);
    }

    /// <summary>
    /// Constructor
    /// </summary>
    public TextureSheet(Texture2D texture, int numRows, int numCols) {
        this.texture = texture;
        this.numRows = numRows;
        this.numCols = numCols;
        zoneWidth = texture.Width / numCols;
        zoneHeight = texture.Height / numRows;
    }

    /// <summary>
    /// Returns a rectangle specifying the texture coords to be used in Quad
    /// (origin.X, origin.Y, width, height)
    /// </summary>
    public Rectangle GetZone(int zone) {
        return new Rectangle(
            (int) (zone * zoneWidth),
            (int) ((numRows - (zone + 1)) * zoneHeight),
            (int) zoneWidth,
            (int) zoneHeight
        );
    }
}
