using OpenTK.Mathematics;

namespace RayTracing;

// abstract class requires specification of plane/sphere when instantiating the primitive class
public abstract class Primitive
{
    public Vector3 Color;

    public abstract Intersection Intersect(Ray ray);
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

    public override Intersection Intersect(Ray ray)
    {
        // Since `Normal` is unit length, the plane has equation: Ax + By + Cz - D = 0.
        // Where:   [A, B, C] = `Normal`
        //          D = `Distance`
        
        // At intersection ray-parameter t = (D - (n . b)) / (n . d)
        // Where:   D = `Distance`
        //          n = `Normal` (unit)
        //          b = `ray.Base`
        //          d = `ray.Direction` (unit)
        
        // If this is zero the 

        throw new NotImplementedException();
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

    public override Intersection Intersect(Ray ray)
    {
        // Ray intersection with a 3D model: sphere
        
        // ray.Base & ray.Direction
        // Base E = (ex, ey, ez) 
        // Direction D = (dx, dy, dz)

        // base
        float ex = ray.Base[0];
        float ey = ray.Base[1];
        float ez = ray.Base[2];

        // direction
        float dx = ray.Direction[0];
        float dy = ray.Direction[1];
        float dz = ray.Direction[2];

        // sphere centre
        float x0 = Position[0];
        float y0 = Position[1];
        float z0 = Position[2];

        float a = Math.pow(dx, 2) + Math.pow(dy, 2) + Math.pow(dz, 2);
        float b = 2 * (ex - x0) * dx + 2 * (ey - y0) * dy + 2 * (ez - z0) * dz;
        float c = Math.pow(ex - x0, 2) + Math.pow(ey - y0, 2)  + Math.pow(ez - z0, 2) ;

        float t1 = (-b + sqrt(b * b + 4 * a * c)) / (2 * a);
        float t2 = (-b - sqrt(b * b + 4 * a * c)) / (2 * a);


        throw new NotImplementedException();
    }
}