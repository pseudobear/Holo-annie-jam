#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

/// <summary>
/// A quad object to get drawn
/// </summary>
class Quad {

    public VertexPositionNormalTexture[] Vertices {
        get { return vertices; }
        protected set { vertices = value; }
    }
    VertexPositionNormalTexture[] vertices;

    public short[] Indices {
        get { return indices;  }
    }
    short[] indices; 


    public Vector3 Origin {
        get { return origin; } 
        set {
            origin = value;
            FillVertices();
        }
    }
    Vector3 origin;

    Vector3 normal;
    Vector3 up;
    Vector3 left;
    Vector3 upperCenter;
    Vector3 upperLeft;
    Vector3 upperRight;
    Vector3 lowerLeft;
    Vector3 lowerRight;
    float width;
    float height;

    public Quad(Vector3 origin, Vector3 normal, Vector3 up, float width, float height) {
        vertices = new VertexPositionNormalTexture[4];
        indices = new short[6];
        this.origin = origin;
        this.normal = normal;
        this.up = up;
        this.width = width;
        this.height = height;

        FillVertices();
    }

    private void FillVertices() {
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

        // Set the position and texture coordinate for each
        // vertex
        vertices[0].Position = lowerLeft;
        vertices[0].TextureCoordinate = textureLowerLeft;
        vertices[1].Position = upperLeft;
        vertices[1].TextureCoordinate = textureUpperLeft;
        vertices[2].Position = lowerRight;
        vertices[2].TextureCoordinate = textureLowerRight;
        vertices[3].Position = upperRight;
        vertices[3].TextureCoordinate = textureUpperRight;

        // Set the index buffer for each vertex, using
        // clockwise winding
        indices[0] = 0;
        indices[1] = 1;
        indices[2] = 2;
        indices[3] = 2;
        indices[4] = 1;
        indices[5] = 3;
    }
}