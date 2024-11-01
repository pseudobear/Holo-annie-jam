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

    #region Properties

    public Texture2D Texture {
        get { return texture;  }
    }
    Texture2D texture;

    public float ZoneWidth {
        get { return zoneWidth; }
    }
    float zoneWidth;

    public float ZoneHeight {
        get { return zoneHeight; }
    }
    float zoneHeight;

    public int NumRows {
        get { return numRows; }
    }
    int numRows;

    public int NumCols {
        get { return numCols; }
    }
    int numCols;

    public int Width {
        get { return texture.Width; }
    }

    public int Height {
        get { return texture.Height; }
    }
    #endregion

    #region fields
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
    /// Returns a rectangle specifying the texture coords to be used in Quad in pixels
    /// This needs to be converted into a percentage of the texture to be used with Quad
    /// (origin.X, origin.Y, width, height)
    /// </summary>
    public Rectangle GetZone(int zone) {
        int x = (zone + 1) % numCols;
        int y = (zone + 1) / numCols;
        return new Rectangle(
            (int) ((x - 1) * zoneWidth),
            (int) ((y - 1) * zoneHeight),
            (int) zoneWidth,
            (int) zoneHeight
        );
    }
}
