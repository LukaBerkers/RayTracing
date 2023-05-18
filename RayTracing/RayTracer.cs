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
        _camera = new Camera(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);
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
}

public struct Ray
{
    public Vector3 Base;
    public Vector3 Direction;
}