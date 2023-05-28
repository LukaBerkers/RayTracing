using OpenTK.Mathematics;

namespace RayTracing;

// abstract class requires specification of plane/sphere when instantiating the primitive class
public abstract class Primitive
{
    public Vector3 Color;

    public abstract Intersection? Intersect(Ray ray);
}

public class Plane : Primitive
{
    /// <summary>
    ///     Assumed to be of unit length.
    /// </summary>
    public Vector3 Normal;

    /// <summary>
    ///     Signed distance in the direction of `Normal`.
    /// </summary>
    public float Distance;

    /// <param name="normal">Should be a unit vector.</param>
    /// <param name="distanceFromOrigin">
    ///     Should be the signed distance according to the direction of <paramref name="normal" />.
    /// </param>
    public Plane(Vector3 normal, float distanceFromOrigin)
    {
        Normal = normal;
        Distance = distanceFromOrigin;
    }

    public override Intersection? Intersect(Ray ray)
    {
        // Since `Normal` is unit length, the plane has equation: Ax + By + Cz - D = 0.
        // Where:   [A, B, C] = `Normal`
        //          D = `Distance`

        // At intersection: ray-parameter t = (D - (n . b)) / (n . d)
        // Where:   D = `Distance`
        //          n = `Normal` (unit)
        //          b = `ray.Base`
        //          d = `ray.Direction` (unit)
        // And since d is unit: t is the distance to the intersection

        // If this is zero the ray is parallel to the plane
        var cosAngleRayPlaneNormal = Vector3.Dot(Normal, ray.Direction);
        if (Helper.IsZero(cosAngleRayPlaneNormal)) return null;

        var distance = (Distance - Vector3.Dot(Normal, ray.Base)) / cosAngleRayPlaneNormal;

        return new Intersection(distance, this, Normal);
    }
}

public class Sphere : Primitive
{
    public Vector3 Position;
    public float Radius;

    public Sphere(Vector3 position, float radius, Vector3 color = default)
    {
        Position = position;
        Radius = radius;
        Color = color;
    }

    public override Intersection? Intersect(Ray ray)
    {
        throw new NotImplementedException();
    }
}