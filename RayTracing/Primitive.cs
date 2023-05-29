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
    /// <param name="color">Color of the plane as RGB vector from 0 to 1.</param>
    public Plane(Vector3 normal, float distanceFromOrigin, Vector3 color)
    {
        Normal = normal;
        Distance = distanceFromOrigin;
        Color = color;
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
        if (distance < 0.0f || Helper.IsZero(distance)) return null;
        
        return new Intersection(distance, this, Normal, Color);
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
        // Equation of the sphere is (x - h)^2 + (y - k)^2 + (z - l)^2 = r^2
        // Where:   [h, k, l] = `Position`
        //          r = `Radius`
        
        // At intersection: ray-parameter t = (-b ± sqrt(b^2 - 4ac)) / 2a   (quadratic formula)
        // Where:   a = d . d
        //          b = 2 * (d . (b - p))
        //          c = (b - p) . (b - p) - r^2     (note that: v . v = |v|^2)
        //      Where:  d = `ray.Direction` (unit)
        //              b = `ray.Base`
        //              p = `Position`
        //              r = `Radius`
        // And since d is unit: t is the distance to the intersection

        throw new NotImplementedException();
    }
}