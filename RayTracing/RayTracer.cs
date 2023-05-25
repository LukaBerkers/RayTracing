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
        Display.Line(left.X, left.Y, right.X, right.Y, 0xff_ff_ff);
    }

    private Vector2i DebugWorldToScreen(Vector3 vec)
    {
        // First flatten; assume the vector is in the right y plane
        Vector2 flat = (vec.X, vec.Z);

        // Amount of pixels per unit lenght
        const int scale = 32;
        // Let the origin be at the middle-bottom
        Vector2i originInScreenSpace = (Display.Width / 2, Display.Height * 7 / 8);

        // Then convert to integer representation
        var pixel = originInScreenSpace + (Convert.ToInt32(flat.X * scale), Convert.ToInt32(flat.Y * scale));

        return pixel;
    }
}

public struct Ray
{
    public Vector3 Base;
    public Vector3 Direction;
}