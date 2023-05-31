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
        var mirrorWall = new Plane((1.0f, 0.0f, 1.0f), -10.0f, Vector3.One, Primitive.MaterialType.Mirror);
        Vector3 copperColor = (13.0f / 18.0f, 9.0f / 20.0f, 0.2f);
        var copperBall = new Sphere((-2, 0, -4), 1, copperColor, Primitive.MaterialType.Metal);
        var greenSphere = new Sphere((2, 1, -6), 4, (0, 0.5f, 0), Primitive.MaterialType.Plastic);
        var mirrorBall = new Sphere((-2, 2, -6), 2, Vector3.One, Primitive.MaterialType.Mirror);

        var shapes = new List<Primitive> { plane, copperBall, greenSphere, mirrorBall, mirrorWall };

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
        const bool debug = true;

        if (debug)
        {
            _rayTracer.Debug();

            // To test: move green sphere up
            switch (_rayTracer.Scene.Primitives[2])
            {
                case Sphere sphere:
                    sphere.Position.Y += 1.0f / 32.0f;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return;
        }

        _rayTracer.Render();

        // To test: shrink the green sphere
        switch (_rayTracer.Scene.Primitives[2])
        {
            case Sphere sphere:
                sphere.Radius = float.Max(sphere.Radius - 1.0f / 32.0f, 0.0f);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}