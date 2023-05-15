using OpenTK.Mathematics;

namespace RayTracing;

public class Camera
{
    public Vector3 Position;
    public Vector3 LookAt;
    public Vector3 Up;
    public ScreenPlane ScreenPlane;

    public Camera(Vector3 position, Vector3 lookAt, Vector3 up)
    {
        Position = position;
        LookAt = lookAt;
        Up = up;
        ScreenPlane screenPlane = new ScreenPlane
        {
            TopLeft = (-1, +1, +1),
            TopRight = (+1, +1, +1),
            BottomLeft = (-1, -1, +1),
            BottomRight = (+1, -1, +1)
        };
    }

}

public struct ScreenPlane
{
    // ( 0, 0, 0)
    public Vector3 TopLeft; // (-1, +1, +1)
    public Vector3 TopRight; // (+1, +1, +1)
    public Vector3 BottomLeft; // (-1, -1, +1)
    public Vector3 BottomRight; // (+1, -1, +1)
    
}

