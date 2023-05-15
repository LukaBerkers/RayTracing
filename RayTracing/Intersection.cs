using OpenTK.Mathematics;

namespace RayTracing;

public class Intersection
{
    public float Distance;
    public Primitive NearestPrimitive;
    public Vector3 Normal;
}