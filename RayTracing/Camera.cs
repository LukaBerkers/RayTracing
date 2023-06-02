using OpenTK.Mathematics;

namespace RayTracing;

public class Camera
{
    private Vector3 _lookAt;
    private Vector3 _right;
    private Vector3 _up;
    public Vector3 Position;
    public ScreenPlane ScreenPlane;

    public Camera(Vector3 position, Vector3 lookAt, Vector3 up, float aspectRatio = 1.0f, float fov = 60.0f)
    {
        Position = position;
        _lookAt = lookAt.Normalized();
        _up = up.Normalized();

        // Clamp the fov value to be within the range [60, 120] degrees
        var degrees = MathHelper.Clamp(fov, 60.0f, 120.0f);
        // Transitioning the degrees to a scalar
        var screenDistance = 1 / float.Tan(MathHelper.DegreesToRadians(degrees) / 2.0f);

        var midScreen = position + lookAt * screenDistance;
        _right = Vector3.Cross(lookAt, up);
        var scaledRight = aspectRatio * _right;
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

    public Vector3 TopLR => TopRight - TopLeft;
    public Vector3 TBLeft => BottomLeft - TopLeft;
}