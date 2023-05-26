using OpenTK.Mathematics;

namespace RayTracing;

public class Camera
{
    private Vector3 _lookAt;
    private Vector3 _right;
    private Vector3 _up;
    public Vector3 Position;
    public ScreenPlane ScreenPlane;

    public Camera(Vector3 position, Vector3 lookAt, Vector3 up)
    {
        Position = position;
        _lookAt = lookAt.Normalized();
        _up = up.Normalized();
        var midScreen = position + lookAt;
        _right = Vector3.Cross(lookAt, up);
        ScreenPlane = new ScreenPlane
        {
            TopLeft = midScreen + up - _right,
            TopRight = midScreen + up + _right,
            BottomLeft = midScreen - up - _right,
            BottomRight = midScreen - up + _right
        };
    }

    public Vector3 LookAt
    {
        get => _lookAt;
        set
        {
            _lookAt = value.Normalized();
            _right = Vector3.Cross(value, Up);
        }
    }

    public Vector3 Up
    {
        get => _up;
        set
        {
            _up = value.Normalized();
            _right = Vector3.Cross(_lookAt, value);
        }
    }
}

public struct ScreenPlane
{
    // ( 0, 0, 0)
    public Vector3 TopLeft; // (-1, +1, -1)
    public Vector3 TopRight; // (+1, +1, -1)
    public Vector3 BottomLeft; // (-1, -1, -1)
    public Vector3 BottomRight; // (+1, -1, -1)
}