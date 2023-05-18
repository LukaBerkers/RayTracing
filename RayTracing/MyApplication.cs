namespace RayTracing;

public class MyApplication
{
    private readonly RayTracer _rayTracer;

    // constructor
    public MyApplication(Surface screen)
    {
        var lights = new List<Light> { new() };
        var shapes = new List<Primitive> { new Plane(), new Sphere() };
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
    }
}