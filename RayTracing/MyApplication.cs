using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace RayTracing;

public class MyApplication
{
    private readonly RayTracer _rayTracer;

    // constructor
    public MyApplication(Surface screen)
    {
        // Just one light, I don't care about its properties for now
        var lights = new List<Light> { new((3, 4, 0), (8, 24, 24)), new((-1, 5, -1), (24, 8, 8)) };

        Vector3 TileTexture(Vector2 uv)
        {
            // var c = (int)uv.X + (int)uv.Y & 1;
            var c = ((int)MathHelper.Floor(uv.X) + (int)MathHelper.Floor(uv.Y)) & 1;
            return new Vector3(c);
        }

        var plane = new Plane(Vector3.UnitY, -1.0f, (0.5f, 0.125f, 0.5f), Primitive.MaterialType.Matte, TileTexture);
        var diagonalWall = new Plane
        (
            new Vector3(1.0f, 0.0f, 1.0f).Normalized(),
            -10.0f, Vector3.One,
            Primitive.MaterialType.Plastic,
            TileTexture
        );
        Vector3 copperColor = (13.0f / 18.0f, 9.0f / 20.0f, 0.2f);
        var copperBall = new Sphere((-2, 0, -4), 1, copperColor, Primitive.MaterialType.Metal);
        var greenSphere = new Sphere((2, 1, -6), 4, (0, 0.5f, 0), Primitive.MaterialType.Plastic);
        var mirrorBall = new Sphere((-2, 2, -6), 2, Vector3.One, Primitive.MaterialType.Mirror);

        var shapes = new List<Primitive> { plane, copperBall, greenSphere, mirrorBall, diagonalWall };

        _rayTracer = new RayTracer(screen, lights, shapes, 90.0f);
    }

    public Surface Screen => _rayTracer.Display;

    private Camera Camera => _rayTracer.Camera;

    // initialize
    public void Init()
    {
    }

    // tick: renders one frame
    public void Tick()
    {
        const bool debug = false;

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

    // Updates the camera placement depending on keyboard movement
    public void Update(KeyboardState kbState)
    {
        const float moveSpeed = 0.125f;
        const float rotationSpeed = 0.125f;

        // Move the camera forward and backward for keys 'w' and 's'
        if (kbState[Keys.W])
            MoveCameraForward(moveSpeed);
        else if (kbState[Keys.S]) MoveCameraBackward(moveSpeed);

        // Move the camera left and right for keys 'a' and 'd'
        if (kbState[Keys.A])
            RotateCamera(-rotationSpeed);
        else if (kbState[Keys.D]) RotateCamera(rotationSpeed);
    }

    // Keyboard movement of camera
    private void MoveCameraForward(float moveSpeed)
    {
        Camera.Position += moveSpeed * Camera.LookAt;
    }

    private void MoveCameraBackward(float moveSpeed)
    {
        Camera.Position -= moveSpeed * Camera.LookAt;
    }

    private void RotateCamera(float angle)
    {
        var rotation = Matrix3.CreateFromAxisAngle(Camera.Up, angle);
        Camera.LookAt = rotation * Camera.LookAt;
    }
}