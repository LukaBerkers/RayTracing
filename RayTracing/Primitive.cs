using OpenTK.Mathematics;

namespace RayTracing;

// abstract class requires specification of plane/sphere when instantiating the primitive class
public abstract class Primitive
{
    public Vector3 Color;
    public MaterialType Material;

    public enum MaterialType
    {
        Matte,
        Plastic,
        Metal,
        Mirror
    }
    
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
    /// <param name="material">Defaults to matte.</param>
    public Plane(Vector3 normal, float distanceFromOrigin, Vector3 color, MaterialType material = default)
    {
        Normal = normal;
        Distance = distanceFromOrigin;
        Color = color;
        Material = material;
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

    public Sphere(Vector3 position, float radius, Vector3 color = default, MaterialType material = default)
    {
        Position = position;
        Radius = radius;
        Color = color;
        Material = material;
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

        var positionToBase = ray.Base - Position;
        var distanceSolution = Helper.SolveQuadratic
        (
            ray.Direction.LengthSquared,
            2.0f * Vector3.Dot(ray.Direction, positionToBase),
            positionToBase.LengthSquared - Radius * Radius
        );

        switch (distanceSolution)
        {
            // If the ray is tangent we also return no intersection
            case QuadraticSolution.None or QuadraticSolution.One:
                return null;
            case QuadraticSolution.Two solution:
            {
                // t1 < t2
                var (t1, t2) = (solution.Val1, solution.Val2);
                // We need the closest intersection > 0
                var t = Helper.Compare(t1, 0.0f) > 0 ? t1 : t2;
                // If t is still negative return no intersection
                if (Helper.Compare(t, 0.0f) <= 0) return null;
                var normal = ray.Evaluate(t1) - Position;
                normal.NormalizeFast();
                return new Intersection(t1, this, normal, Color);
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(distanceSolution));
        }
    }
}