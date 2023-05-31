using OpenTK.Mathematics;

namespace RayTracing;

public class MyApplication
{
    private readonly RayTracer _rayTracer;

    // constructor
    public MyApplication(Surface screen)
    {
        // Just one light, I don't care about its properties for now
        var lights = new List<Light> { new((3, 4, 0), (8, 24, 24)), new((-1, 5, -1), (24, 8, 8)) };

        var plane = new Plane(Vector3.UnitY, -1.0f, (0.5f, 0.125f, 0.5f));
        Vector3 copperColor = (13.0f / 18.0f, 9.0f / 20.0f, 0.2f);
        var copper = new Sphere((-2, 0, -4), 1, copperColor, Primitive.MaterialType.Metal);
        var greenSphere = new Sphere((2, 1, -6), 4, (0, 0.5f, 0), Primitive.MaterialType.Plastic);

        var shapes = new List<Primitive> { plane, copper, greenSphere };

        _rayTracer = new RayTracer(screen, lights, shapes);
    }

    public Surface Screen => _rayTracer.Display;

    // initialize
    public void Init()
    {
    }

    // tick: renders one frame
    public void Tick()
    {
        _rayTracer.Render();
        // _rayTracer.Debug();
    }
}