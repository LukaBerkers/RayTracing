namespace RayTracing;

public class Scene
{
    public List<Light> LightSources = new();
    public List<Primitive> Primitives = new();

    public Intersection? ClosestIntersection(Ray ray)
    {
        Intersection? closestIntersection = null;
        
        foreach (var intersection in Primitives.Select(primitive => primitive.Intersect(ray)))
        {
            if (closestIntersection is null && intersection is not null) closestIntersection = intersection;
            // expression will always be false if one of them is null
            if (closestIntersection?.Distance > intersection?.Distance) closestIntersection = intersection;
        }

        return closestIntersection;
    }
}