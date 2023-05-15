using OpenTK.Mathematics;
using SixLabors.ImageSharp;

namespace RayTracing;

// abstract class requires specification of plane/sphere when instantiating the primitive class
public abstract class Primitive
{
    public Vector3 Color;

    public abstract Intersection Intersect(Ray ray);
}

public class Plane : Primitive
{
    public Vector3 Normal;
    public float Distance;
    public override Intersection Intersect(Ray ray)
    {
        throw new NotImplementedException();
    }
}

public class Sphere : Primitive
{
    public Vector3 Position;
    public float Radius;
    public override Intersection Intersect(Ray ray)
    {
        throw new NotImplementedException();
    }
}