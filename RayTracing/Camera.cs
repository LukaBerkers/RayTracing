using OpenTK.Mathematics;

namespace RayTracing;

public class Camera
{
    private Vector3 _lookAt;
    private Vector3 _right;
    private Vector3 _up;
    public Vector3 Position;
    public ScreenPlane ScreenPlane;

    public Camera(Vector3 position, Vector3 lookAt, Vector3 up, float aspectRatio = 1.0f, float degrees = 60.0f)
    {
        Position = position;
        _lookAt = lookAt.Normalized();
        _up = up.Normalized();
        
        // Clamp the degree value to be within the range [60, 90] degrees
        float clampedDegree = degrees < 60.0f ? 60.0f : (degrees > 90.0f ? 90.0f : degrees);
        // Transitioning the degrees to a scalar
        float scalar = 1 / (MathF.Tan(MathF.PI * clampedDegree / 180.0f / 2.0f));
        
        var midScreen = position + lookAt * scalar;
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