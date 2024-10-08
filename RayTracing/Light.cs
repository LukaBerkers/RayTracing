using OpenTK.Mathematics;

namespace RayTracing;

public class Light
{
    public Vector3 Location;
    public Vector3 Intensity;

    public Light(Vector3 location, Vector3 intensity)
    {
        Location = location;
        Intensity = intensity;
    }
}