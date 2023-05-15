using OpenTK.Mathematics;
namespace RayTracing;

public class Raytracer
{
    public Scene Scene;
    public Camera Camera;
    public Surface Surface;
}

public struct Ray
{
    public Vector3 Base;
    public Vector3 Direction;
}