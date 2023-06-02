using OpenTK.Mathematics;

namespace RayTracing;

// abstract class requires specification of plane/sphere when instantiating the primitive class
public abstract class Primitive
{
    public Vector3 Color;
    protected Func<Vector2, Vector3>? Texture;
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

    private readonly Vector3 _u;
    private readonly Vector3 _v;

    /// <param name="normal">Should be a unit vector.</param>
    /// <param name="distanceFromOrigin">
    ///     Should be the signed distance according to the direction of <paramref name="normal" />.
    /// </param>
    /// <param name="color">Color of the plane as RGB vector from 0 to 1.</param>
    /// <param name="material">Defaults to matte.</param>
    /// <param name="textureMapping">A function from uv coordinates to a color.</param>
    public Plane
    (
        Vector3 normal,
        float distanceFromOrigin,
        Vector3 color,
        MaterialType material = default,
        Func<Vector2, Vector3>? textureMapping = null
    )
    {
        Normal = normal;
        Distance = distanceFromOrigin;
        Color = color;
        Material = material;
        Texture = textureMapping;

        // Constructing two unit vectors in the plane. This is based on §2.4.6 of Marschner et al, 4th ed.
        // Find the smallest component of `normal`
        int smallestComponent;
        if (normal.Z < normal.X && normal.Z < normal.Y)
            smallestComponent = 2;
        else if (normal.Y < normal.X && normal.Y < normal.Z)
            smallestComponent = 1;
        else
            smallestComponent = 0;

        var t = normal;
        t[smallestComponent] = 1;
        _u = Vector3.Cross(t, normal).Normalized();
        _v = Vector3.Cross(normal, _u);
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

        Vector3 color;
        if (Texture is null)
        {
            color = Color;
        }
        else
        {
            // A point on the plane (specifically, the one closest to the origin)
            var p = Distance * Normal;
            var pToIntersection = ray.Evaluate(distance) - p;
            var u = Vector3.Dot(pToIntersection, _u);
            var v = Vector3.Dot(pToIntersection, _v);
            color = Texture((u, v));
        }

        return new Intersection(distance, this, Normal, color);
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
                var normal = ray.Evaluate(t) - Position;
                normal.NormalizeFast();
                return new Intersection(t, this, normal, Color);
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(distanceSolution));
        }
    }
}

public class Triangle : Primitive
{
    public Vector3 V0;
    public Vector3 V1;
    public Vector3 V2;

    public Triangle(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 color = default, MaterialType material = default)
    {
        V0 = v0;
        V1 = v1;
        V2 = v2;
        Color = color;
        Material = material;
    }

    public override Intersection? Intersect(Ray ray)
    {
        // Perform ray-triangle intersection using barycentric coordinates

        var e1 = V1 - V0; // B - A
        var e2 = V2 - V0; // C - A
        var p = Vector3.Cross(ray.Direction, e2);
        var det = Vector3.Dot(e1, p);

        // If the determinant is close to zero, the ray is parallel to the triangle
        if (Helper.IsZero(det)) return null;

        var invDet = 1.0f / det;
        var t = ray.Base - V0;
        var u = Vector3.Dot(t, p) * invDet;

        // Check if the intersection is outside the triangle
        if (u < 0.0f || u > 1.0f) return null;

        var q = Vector3.Cross(t, e1);
        var v = Vector3.Dot(ray.Direction, q) * invDet;

        // Check if the intersection is outside the triangle
        if (v < 0.0f || u + v > 1.0f) return null;

        var distance = Vector3.Dot(e2, q) * invDet;

        // If the distance is negative, the intersection is behind the ray origin
        if (distance < 0.0f) return null;

        // Calculate the interpolated normal using barycentric coordinates
        var n0 = Vector3.Cross(V1 - V0, V2 - V0).Normalized();
        var n1 = Vector3.Cross(V2 - V1, V0 - V1).Normalized();
        var n2 = Vector3.Cross(V0 - V2, V1 - V2).Normalized();
        var normal = (1.0f - u - v) * n0 + u * n1 + v * n2;

        return new Intersection(distance, this, normal, Color);
    }
}