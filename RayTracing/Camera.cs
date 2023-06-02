using OpenTK.Mathematics;

namespace RayTracing;

public class Camera
{
    private Vector3 _lookAt;
    private Vector3 _position;
    private readonly float _aspectRatio;
    private readonly float _screenDistance;
    public ScreenPlane ScreenPlane { get; private set; }

    public Camera(Vector3 position, Vector3 lookAt, Vector3 up, float aspectRatio = 1.0f, float fov = 60.0f)
    {
        _position = position;
        _lookAt = lookAt.Normalized();
        _aspectRatio = aspectRatio;
        _aspectRatio = aspectRatio;
        Up = up.Normalized();
        Right = Vector3.Cross(_lookAt, Up);
        
        // Clamp the fov value to be within the range [60, 120] degrees
        var degrees = MathHelper.Clamp(fov, 60.0f, 120.0f);
        // Transitioning the degrees to a scalar
        _screenDistance = 1 / float.Tan(MathHelper.DegreesToRadians(degrees) / 2.0f);

        var midScreen = _position + _lookAt * _screenDistance;
        var scaledRight = _aspectRatio * Right;
        ScreenPlane = new ScreenPlane(midScreen, up, scaledRight);
    }

    public Vector3 LookAt
    {
        get => _lookAt;
        set
        {
            _lookAt = value.Normalized();
            Right = Vector3.Cross(_lookAt, Up);
            var midScreen = _position + _lookAt * _screenDistance;
            var scaledRight = _aspectRatio * Right;
            ScreenPlane = new ScreenPlane(midScreen, Up, scaledRight);
        }
    }

    public Vector3 Up { get; }

    public Vector3 Right { get; private set; }

    public Vector3 Position
    {
        get => _position;
        set
        {
            _position = value;
            var midScreen = _position + _lookAt * _screenDistance;
            var scaledRight = _aspectRatio * Right;
            ScreenPlane = new ScreenPlane(midScreen, Up, scaledRight);
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

    public ScreenPlane(Vector3 midScreen, Vector3 up, Vector3 scaledRight)
    {
        TopLeft = midScreen + up - scaledRight;
        TopRight = midScreen + up + scaledRight;
        BottomLeft = midScreen - up - scaledRight;
        BottomRight = midScreen - up + scaledRight;
    }
}