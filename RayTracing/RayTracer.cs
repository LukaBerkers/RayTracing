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
            if (float.IsNaN(circleRadius))
                break;
            DebugDrawCircle(sphere.Position.Xz, circleRadius);
        }
    }

    private Vector2i DebugWorldToScreen(Vector3 vec)
    {
        // First flatten; assume the vector is in the right y plane
        var flat = vec.Xz;

        // Amount of pixels per unit lenght
        const int scale = 32;
        // Let the origin be at the middle-bottom
        Vector2i originInScreenSpace = (Display.Width / 2, Display.Height * 7 / 8);

        // Then convert to integer representation
        var pixel = originInScreenSpace + (Vector2i)(flat * scale);
        
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

    private void DebugDrawCircle(Vector2 center, float radius)
    {
        throw new NotImplementedException();
    }

    #endregion
}

public struct Ray
{
    public Vector3 Base;
    public Vector3 Direction;
}