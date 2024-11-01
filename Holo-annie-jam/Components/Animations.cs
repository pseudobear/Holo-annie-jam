#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
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
        int y = zone / numCols;
        return new Rectangle(
            (int) ((x - 1) * zoneWidth),
            (int) (y * zoneHeight),
            (int) zoneWidth,
            (int) zoneHeight
        );
    }
}

class Animation {

    long tickDelay;
    long lastFrameTick;

    public bool Active;

    public int CurrentFrameIdx {
        get { return currentFrameIdx; }
        set { currentFrameIdx = value; }
    }
    int currentFrameIdx;

    public List<int> Frames {
        get { return frames; }
    }
    List<int> frames = new List<int>();

    public Animation(long tickDelay) {
        this.tickDelay = tickDelay;
        currentFrameIdx = 0;
        Active = false;
        lastFrameTick = 0;
    }

    public void Update(long tick) {
        if (!Active) return;
        if (tick >= lastFrameTick + tickDelay) {
            lastFrameTick = tick;
            currentFrameIdx++;
            if (currentFrameIdx == frames.Count) {
                currentFrameIdx = 0;
                Active = false;
            }
        }
    }

    public void Start() {
        Active = true;
        currentFrameIdx = 0;
        lastFrameTick = 0;
    }

    public void SetTextureCoords(ref Quad quad, TextureSheet sheet) {
        System.Diagnostics.Debug.WriteLine(sheet[currentFrameIdx]);
        quad.SetTextureCoords(
            new Vector2(
                (float)sheet[currentFrameIdx].X / (float)sheet.Width,
                (float)sheet[currentFrameIdx].Y / (float)sheet.Height
            ),
            (float)sheet[currentFrameIdx].Width / (float)sheet.Width,
            (float)sheet[currentFrameIdx].Height / (float)sheet.Height
        );
    }
}
