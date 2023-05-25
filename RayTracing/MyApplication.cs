namespace RayTracing;

public class MyApplication
{
    private readonly RayTracer _rayTracer;

    // constructor
    public MyApplication(Surface screen)
    {
        // Just one light, I don't care about its properties for now
        var lights = new List<Light> { new() };
        // Red sphere in front
        var redSphere = new Sphere((-2, 0, -4), 1, (1, 0, 0));
        var shapes = new List<Primitive> { redSphere };

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
        // _rayTracer.Render();
        _rayTracer.Debug();
    }
}