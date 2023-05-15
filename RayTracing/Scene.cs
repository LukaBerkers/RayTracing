namespace RayTracing;

public class Scene
{
    public List<Light> LightSources;
    public List<Primitive> Primitives;

    public Intersection ClosestIntersection(Ray ray)
    {
        var closestIntersection = new Intersection
        {
            Distance = float.PositiveInfinity
        };
        foreach (var primitive in Primitives)
        {
            var intersection = primitive.Intersect(ray);
            if (closestIntersection.Distance > intersection.Distance) closestIntersection = intersection;
        }

        return closestIntersection;
    }
}