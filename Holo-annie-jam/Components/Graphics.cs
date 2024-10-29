#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

/// <summary>
/// A quad object to get drawn
/// </summary>
class Quad {

    #region properties
    public VertexPositionNormalTexture[] Vertices {
        get { return vertices; }
        protected set { vertices = value; }
    }
    VertexPositionNormalTexture[] vertices;

    public short[] Indices {
        get { return indices; }
    }
    short[] indices;


    public Vector3 Origin {
        get { return origin; }
        set {
            origin = value;
            FillVertices(false);
        }
    }
    Vector3 origin;

    public Vector3 Normal {
        get { return normal; }
        set {
            normal = value;
            FillVertices(false);
        }
    }
    Vector3 normal;

    public Vector3 Up {
        get { return up; }
        set {
            up = value;
            FillVertices(false);
        }
    }

    public float Width {
        get { return width; }
        set {
            width = value;
            FillVertices(false);
        }
    }
    float width;

    public float Height {
        get { return height; }
        set {
            height = value;
            FillVertices(false);
        }
    }
    float height;

    #endregion

    #region fields
    Vector3 up;
    Vector3 left;
    Vector3 upperCenter;
    Vector3 upperLeft;
    Vector3 upperRight;
    Vector3 lowerLeft;
    Vector3 lowerRight;
    #endregion

    /// <summary>
    /// Constructor, origin is the center of the quad 
    /// </summary>
    public Quad(Vector3 origin, Vector3 normal, Vector3 up, float width, float height) {
        vertices = new VertexPositionNormalTexture[4];
        indices = new short[6];
        this.origin = origin;
        this.normal = normal;
        this.up = up;
        this.width = width;
        this.height = height;

        FillVertices(true);
    }

    /// <summary>
    /// Constructor with texture coords
    /// </summary>
    public Quad(Vector3 origin, Vector3 normal, Vector3 up, float width, float height, 
        Vector2 texOrigin, float texWidth, float texHeight) : this(origin, normal, up, width, height) {
        this.SetTextureCoords(texOrigin, texWidth, texHeight);
    }

    private void FillVertices(bool fillTextureCoords=false) {
        // Calculate the quad corners
        left = Vector3.Cross(normal, up);
        upperCenter = (up * height / 2) + origin;
        upperLeft = upperCenter + (left * width / 2);
        upperRight = upperCenter - (left * width / 2);
        lowerLeft = upperLeft - (up * height);
        lowerRight = upperRight - (up * height);

        // Provide a normal for each vertex
        for (int i = 0; i < vertices.Length; i++) {
            vertices[i].Normal = normal;
        }

        // Fill in texture coordinates to display full texture
        // on quad
        Vector2 textureUpperLeft = new Vector2(0.0f, 0.0f);
        Vector2 textureUpperRight = new Vector2(1.0f, 0.0f);
        Vector2 textureLowerLeft = new Vector2(0.0f, 1.0f);
        Vector2 textureLowerRight = new Vector2(1.0f, 1.0f);


        // Set the position and texture coordinates
        vertices[0].Position = lowerLeft;
        vertices[1].Position = upperLeft;
        vertices[2].Position = lowerRight;
        vertices[3].Position = upperRight;

        if (fillTextureCoords) {
            vertices[0].TextureCoordinate = textureLowerLeft;
            vertices[1].TextureCoordinate = textureUpperLeft;
            vertices[2].TextureCoordinate = textureLowerRight;
            vertices[3].TextureCoordinate = textureUpperRight;
        }

        // Set the index buffer for each vertex, using
        // clockwise winding
        indices[0] = 0;
        indices[1] = 1;
        indices[2] = 2;
        indices[3] = 2;
        indices[4] = 1;
        indices[5] = 3;
    }

    /// <summary>
    /// Updates the texture coordinates drawn in this quad. 
    /// In this method, origin is intepretted as the lower left corner.
    /// coordinates are given as multiples of the texture. ie. origin=(0.5f,0.5f) is the midpoint of the texture
    /// and width=(3f,3f) is 3x the texture width-wise and 3x the texture height-wise
    /// therefore, to calculate for pixels, target_pixels / texture_pixel_dimension
    /// </summary>
    public void SetTextureCoords(Vector2 origin, float width, float height) {
        // we flip the texture upside down, so upper <--> lower
        Vector2 textureUpperLeft = origin;
        Vector2 textureUpperRight = origin + (Vector2.UnitX * width);
        Vector2 textureLowerLeft = origin + (Vector2.UnitY * height);
        Vector2 textureLowerRight = origin + (Vector2.UnitX * width) + (Vector2.UnitY * height);

        vertices[0].TextureCoordinate = textureLowerLeft;
        vertices[1].TextureCoordinate = textureUpperLeft;
        vertices[2].TextureCoordinate = textureLowerRight;
        vertices[3].TextureCoordinate = textureUpperRight;
    }
}