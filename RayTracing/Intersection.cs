using OpenTK.Mathematics;

namespace RayTracing;

public class Intersection
{
    public float Distance;
    public Primitive NearestPrimitive;

    /// <summary>Assumed to be unit length.</summary>
    public Vector3 Normal;
    public Vector3 Color;

    /// <summary></summary>
    /// <param name="distance"></param>
    /// <param name="nearestPrimitive"></param>
    /// <param name="normal">Should be unit length.</param>
    /// <param name="color"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public Intersection(float distance, Primitive nearestPrimitive, Vector3 normal, Vector3 color)
    {
        Distance = distance;
        NearestPrimitive = nearestPrimitive ?? throw new ArgumentNullException(nameof(nearestPrimitive));
        Normal = normal;
        Color = color;
    }
}