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

    Vector3 Origin;
    Vector3 Normal;
    Vector3 Up;
    Vector3 Left;
    Vector3 uppercenter;
    Vector3 UpperLeft;
    Vector3 UpperRight;
    Vector3 LowerLeft;
    Vector3 LowerRight;

    public Quad(Vector3 origin, Vector3 normal, Vector3 up, float width, float height) {
        vertices = new VertexPositionNormalTexture[4];
        indices = new short[6];
        Origin = origin;
        Normal = normal;
        Up = up;

        // Calculate the quad corners
        Left = Vector3.Cross(normal, Up);
        uppercenter = (Up * height / 2) + origin;
        UpperLeft = uppercenter + (Left * width / 2);
        UpperRight = uppercenter - (Left * width / 2);
        LowerLeft = UpperLeft - (Up * height);
        LowerRight = UpperRight - (Up * height);

        Fillvertices();

        // Provide a normal for each vertex
        for (int i = 0; i < vertices.Length; i++) {
            vertices[i].Normal = Normal;
        }
    }

    private void Fillvertices() {
        // Fill in texture coordinates to display full texture
        // on quad
        Vector2 textureUpperLeft = new Vector2(0.0f, 0.0f);
        Vector2 textureUpperRight = new Vector2(1.0f, 0.0f);
        Vector2 textureLowerLeft = new Vector2(0.0f, 1.0f);
        Vector2 textureLowerRight = new Vector2(1.0f, 1.0f);

        // Set the position and texture coordinate for each
        // vertex
        vertices[0].Position = LowerLeft;
        vertices[0].TextureCoordinate = textureLowerLeft;
        vertices[1].Position = UpperLeft;
        vertices[1].TextureCoordinate = textureUpperLeft;
        vertices[2].Position = LowerRight;
        vertices[2].TextureCoordinate = textureLowerRight;
        vertices[3].Position = UpperRight;
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