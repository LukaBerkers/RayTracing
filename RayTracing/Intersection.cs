using OpenTK.Mathematics;

namespace RayTracing;

public class Intersection
{
    public float Distance;
    public Primitive? NearestPrimitive;
    public Vector3 Normal;

    public Intersection(float distance, Primitive? nearestPrimitive, Vector3 normal)
    {
        Distance = distance;
        NearestPrimitive = nearestPrimitive;
        Normal = normal;
    }
}