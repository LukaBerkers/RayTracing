using OpenTK.Mathematics;

namespace RayTracing;

public class RayTracer
{
    public readonly Surface Display;
    private Camera _camera;
    private Scene _scene;

    public RayTracer(Surface display, IEnumerable<Light> lightSources, IEnumerable<Primitive> primitives)
    {
        Display = display;
        _camera = new Camera(Vector3.Zero, -Vector3.UnitZ, Vector3.UnitY);
        _scene = new Scene
        {
            LightSources = new List<Light>(lightSources),
            Primitives = new List<Primitive>(primitives)
        };
    }

    public void Render()
    {
        Display.Clear(0);

        throw new NotImplementedException();
    }

    private int ConvertColor(Vector3 color)
    {
        var r = Convert.ToInt32(color.X * 255) << 16;
        var g = Convert.ToInt32(color.Y * 255) << 8;
        var b = Convert.ToInt32(color.Z * 255);

        return r | g | b;
    }

    #region Debug

    public void Debug(float height = 0.0f)
    {
        Display.Clear(0);

        // Draw camera in green
        var camera = DebugWorldToScreen(_camera.Position);
        Display.Bar(camera.X - 1, camera.Y - 1, camera.X + 1, camera.Y + 1, 0x00_ff_00);

        // Draw screen plane in white
        // Get mid point of top and bottom corners
        var screen = _camera.ScreenPlane;
        var leftVec = (screen.TopLeft + screen.BottomLeft) / 2;
        var rightVec = (screen.TopRight + screen.BottomRight) / 2;
        var left = DebugWorldToScreen(leftVec);
        var right = DebugWorldToScreen(rightVec);
        // Draw a line between them
        Display.Line(left.X, left.Y, right.X, right.Y, 0xff_ff_ff);

        // Draw the spheres
        foreach (var sphere in _scene.Primitives.OfType<Sphere>())
        {
            var circleRadius = DebugSpherePlaneIntersectionRadius(sphere, height);
            if (!float.IsNaN(circleRadius))
                DebugDrawCircle(sphere.Position.Xz, circleRadius, sphere.Color);
        }

        // To test: move green sphere up
        switch (_scene.Primitives[1])
        {
            case Sphere sphere:
                sphere.Position.Y += 1.0f / 32.0f;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private Vector2i DebugWorldToScreen(Vector3 vec)
    {
        // Flatten; assume the vector is in the right y plane
        return DebugWorldToScreen(vec.Xz);
    }

    private Vector2i DebugWorldToScreen(Vector2 vec)
    {
        // Amount of pixels per unit lenght
        const int scale = 32;
        // Let the origin be at the middle-bottom
        Vector2i originInScreenSpace = (Display.Width / 2, Display.Height * 7 / 8);

        // Convert to integer representation
        var pixel = originInScreenSpace + (Vector2i)(vec * scale);

        return pixel;
    }

    /// <summary>
    ///     Find the radius of the circle that forms the intersection of a sphere with a certain y-plane.
    /// </summary>
    /// <param name="sphere">The sphere to calculate the intersection for.</param>
    /// <param name="height">The y-value of the intersection plane.</param>
    /// <returns>
    ///     The radius when there is an intersection.
    ///     NaN if there is no intersection.
    /// </returns>
    private static float DebugSpherePlaneIntersectionRadius(Sphere sphere, float height)
    {
        // The radius we are looking for is:
        //  sqrt( r^2 - (a - k)^2 )
        // where: r is the radius of the sphere
        //        a is the plane height value
        //        k is the y-value of the sphere center

        return float.Sqrt(sphere.Radius * sphere.Radius - (height - sphere.Position.Y) * (height - sphere.Position.Y));
    }

    private void DebugDrawCircle(Vector2 center, float radius, Vector3 color)
    {
        const int segments = 64;
        var displayColor = ConvertColor(color);

        for (var i = 0; i < segments; i++)
        {
            var angleStart = i * 2 * float.Pi / segments;
            var angleEnd = (i + 1) * 2 * float.Pi / segments;
            var segmentStart = center + Matrix2.CreateRotation(angleStart) * (radius * Vector2.UnitX);
            var segmentEnd = center + Matrix2.CreateRotation(angleEnd) * (radius * Vector2.UnitX);
            var lineStart = DebugWorldToScreen(segmentStart);
            var lineEnd = DebugWorldToScreen(segmentEnd);
            Display.Line(lineStart.X, lineStart.Y, lineEnd.X, lineEnd.Y, displayColor);
        }
    }

    #endregion
}

public struct Ray
{
    public Vector3 Base;
    public Vector3 Direction;
}