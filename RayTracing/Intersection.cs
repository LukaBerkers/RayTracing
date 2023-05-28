using OpenTK.Mathematics;

namespace RayTracing;

public class Intersection
{
    public float Distance;
    public Primitive NearestPrimitive;
    public Vector3 Normal;
    public Vector3 Color;

    public Intersection(float distance, Primitive nearestPrimitive, Vector3 normal, Vector3 color)
    {
        Distance = distance;
        NearestPrimitive = nearestPrimitive ?? throw new ArgumentNullException(nameof(nearestPrimitive));
        Normal = normal;
        Color = color;
    }
}