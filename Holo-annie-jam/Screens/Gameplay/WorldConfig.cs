#region Using Statements
using Microsoft.Xna.Framework;
#endregion

static class GameConstants {
    public const int NOTE_WIDTH = 500;
    public const int NOTE_HEIGHT = 1000;
    public const int NOTE_HORIZON_DISTANCE = 8000;

    public const int PLAYER_WIDTH = 500;
    public const int PLAYER_HEIGHT = 1000;

    public const int SHADOW_MAX_LEN = 2200;
    public const int SHADOW_MIN_LEN = 200;

    public const int TARGET_LINE_Y = 400;

    // update position -1,600,000 ticks early compared to actual tick
    public const long DEFAULT_VISUAL_OFFSET_TICKS = -1600000; 
}

static class GameplayTransforms {
    static Vector3 cameraPosition = new Vector3(0f, -3000f, 1400f);
    static Vector3 cameraTarget = new Vector3(0.0f, 0.0f, 900f); // Look back at the origin

    static float fovAngle = MathHelper.ToRadians(75);
    static float aspectRatio = 4 / 3;
    static float near = 0.01f; // the near clipping plane distance
    static float far = 10000f; // the far clipping plane distance

    // y+ is forward, x+ is right, z+ is up, try to get y=0 at bottom of screen

    public static Matrix GetWorldMatrix(int viewportHeight) {
        return Matrix.CreateTranslation(0.0f, -(viewportHeight) - 1100, 0.0f);
    }

    public static Matrix GetViewMatrix() {
        return Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.Up);
    }

    public static Matrix GetProjectionMatrix() {
        return Matrix.CreatePerspectiveFieldOfView(fovAngle, aspectRatio, near, far);
    }
}

static class StageOne {
    public const float WALL_OFFSET = 50f;
    public const int NUM_WALLS = 5;
    public static Color BackgroundColor = new Color(195, 205, 191);

    public const long BOB_RATE = 3000000;   // bob rate in ticks
}
