using OpenTK.Mathematics;

namespace RayTracing;

public class Scene
{
    public List<Light> LightSources = new();
    public List<Primitive> Primitives = new();

    public Intersection ClosestIntersection(Ray ray)
    {
        var closestIntersection = new Intersection(float.PositiveInfinity, null, Vector3.Zero);
        
        foreach (var primitive in Primitives)
        {
            var intersection = primitive.Intersect(ray);
            if (closestIntersection.Distance > intersection.Distance) closestIntersection = intersection;
        }

        return closestIntersection;
    }
}