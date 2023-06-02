using OpenTK.Mathematics;

namespace RayTracing;

public class Camera
{
    private Vector3 _lookAt;
    private Vector3 _up;
    public Vector3 Position;
    public ScreenPlane ScreenPlane;

    public Camera(Vector3 position, Vector3 lookAt, Vector3 up, float aspectRatio = 1.0f)
    {
        Position = position;
        _lookAt = lookAt.Normalized();
        _up = up.Normalized();
        var midScreen = position + lookAt;
        Right = Vector3.Cross(lookAt, up);
        var scaledRight = aspectRatio * Right;
        ScreenPlane = new ScreenPlane
        {
            TopLeft = midScreen + up - scaledRight,
            TopRight = midScreen + up + scaledRight,
            BottomLeft = midScreen - up - scaledRight,
            BottomRight = midScreen - up + scaledRight
        };
    }

    public Vector3 LookAt
    {
        get => _lookAt;
        set
        {
            _lookAt = value.Normalized();
            Right = Vector3.Cross(value, Up);
        }
    }

    public Vector3 Up
    {
        get => _up;
        set
        {
            _up = value.Normalized();
            Right = Vector3.Cross(_lookAt, value);
        }
    }

    public Vector3 Right { get; private set; }
}

public struct ScreenPlane
{
    // ( 0, 0, 0)
    public Vector3 TopLeft; // (-1, +1, -1)
    public Vector3 TopRight; // (+1, +1, -1)
    public Vector3 BottomLeft; // (-1, -1, -1)
    public Vector3 BottomRight; // (+1, -1, -1)

    public Vector3 TopLR => TopRight - TopLeft;
    public Vector3 TBLeft => BottomLeft - TopLeft;
}