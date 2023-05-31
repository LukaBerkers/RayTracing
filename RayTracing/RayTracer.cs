using OpenTK.Mathematics;

namespace RayTracing;

public class RayTracer
{
    public readonly Surface Display;
    private Camera _camera;
    private Scene _scene;
    private readonly Vector3 _ambient = Vector3.One / 16.0f;
    private readonly Vector3 _specular = Vector3.One;
    private const float Shininess = 16.0f;

    // For easier access
    private ScreenPlane Screen => _camera.ScreenPlane;

    public RayTracer(Surface display, IEnumerable<Light> lightSources, IEnumerable<Primitive> primitives)
    {
        Display = display;
        var aspectRatio = (float)display.Width / display.Height;
        _camera = new Camera(Vector3.Zero, -Vector3.UnitZ, Vector3.UnitY, aspectRatio);
        _scene = new Scene
        {
            LightSources = new List<Light>(lightSources),
            Primitives = new List<Primitive>(primitives)
        };
    }

    public void Render()
    {
        Display.Clear(0);

        for (var y = 0; y < Display.Height; y++)
        {
            var heightScale = (float)y / Display.Height;

            for (var x = 0; x < Display.Width; x++)
            {
                var widthScale = (float)x / Display.Width;

                // Compute viewing ray
                var screenPosition = Screen.TopLeft + widthScale * Screen.TopLR + heightScale * Screen.TBLeft;
                var direction = screenPosition;
                direction.NormalizeFast();
                var ray = new Ray(_camera.Position, direction);

                // Intersect ray with scene
                var intersection = _scene.ClosestIntersection(ray);

                // Compute illumination at intersection
                // Black if there was no intersection
                var illumination = Vector3.Zero;
                if (intersection is not null)
                {
                    // Shadow rays
                    var intersectLocation = ray.Evaluate(intersection.Distance);
                    foreach (var light in _scene.LightSources)
                    {
                        var shadowRayDirection = light.Location - intersectLocation;
                        var distanceToLightSquared = shadowRayDirection.LengthSquared;
                        shadowRayDirection.NormalizeFast();
                        var shadowRay = new Ray(intersectLocation, shadowRayDirection);
                        var shadowIntersection = _scene.ClosestIntersection(shadowRay);
                        // If there is a something between the original intersection and the light source, continue to
                        // the next light source and do not add to the illumination
                        if (shadowIntersection is not null
                            && Helper.Compare
                            (
                                shadowIntersection.Distance * shadowIntersection.Distance,
                                distanceToLightSquared
                            ) <= 0)
                            continue;

                        // Distance fall-off with angle modulation
                        var lightDirection = shadowRayDirection;
                        var cosLightAngle = Vector3.Dot(intersection.Normal, lightDirection);
                        illumination += intersection.Color * light.Intensity * cosLightAngle / distanceToLightSquared;

                        if
                        (
                            intersection.NearestPrimitive.Material
                            is Primitive.MaterialType.Plastic
                            or Primitive.MaterialType.Metal
                        )
                        {
                            // Specular component
                            var reflectDirection = 2.0f * cosLightAngle * intersection.Normal - lightDirection;
                            reflectDirection.NormalizeFast();
                            var shine = float.Pow(Vector3.Dot(direction, reflectDirection), Shininess);
                            var shineColor = intersection.NearestPrimitive.Material switch
                            {
                                Primitive.MaterialType.Plastic => _specular,
                                Primitive.MaterialType.Metal => intersection.Color,
                                _ => throw new ArgumentOutOfRangeException()
                            };

                            illumination += shineColor * light.Intensity * shine / distanceToLightSquared;
                        }
                    }

                    illumination += intersection.Color * _ambient;
                }

                // Clamp illumination
                illumination = Vector3.Clamp(illumination, Vector3.Zero, Vector3.One);
                // Store resulting color at pixel
                Display.Plot(x, y, ConvertColor(illumination));
            }
        }
        
        // To test: shrink the green sphere
        switch (_scene.Primitives[2])
        {
            case Sphere sphere:
                sphere.Radius = float.Max(sphere.Radius - 1.0f / 32.0f, 0.0f);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private static int ConvertColor(Vector3 color)
    {
        var r = Convert.ToInt32(color.X * 255) << 16;
        var g = Convert.ToInt32(color.Y * 255) << 8;
        var b = Convert.ToInt32(color.Z * 255);

        return r | g | b;
    }

    #region Debug

    public void Debug(float height = 0.0f)
    {
        Display.Clear(0);

        // Draw camera in green
        var camera = DebugWorldToScreen(_camera.Position);
        Display.Bar(camera.X - 1, camera.Y - 1, camera.X + 1, camera.Y + 1, 0x00_ff_00);
        
        // Light sources
        foreach (var light in _scene.LightSources)
        {
            var lightPos = DebugWorldToScreen(light.Location);
            Display.Bar(lightPos.X - 1, lightPos.Y - 1, lightPos.X + 1, lightPos.Y + 1, 0xff_ff_ff);
        }

        // Draw screen plane in white
        // Get mid point of top and bottom corners
        var screen = _camera.ScreenPlane;
        var leftVec = (screen.TopLeft + screen.BottomLeft) / 2;
        var rightVec = (screen.TopRight + screen.BottomRight) / 2;
        var left = DebugWorldToScreen(leftVec);
        var right = DebugWorldToScreen(rightVec);
        // Draw a line between them
        Display.Line(left.X, left.Y, right.X, right.Y, 0xff_ff_ff);

        // Draw the spheres
        foreach (var sphere in _scene.Primitives.OfType<Sphere>())
        {
            var circleRadius = DebugSpherePlaneIntersectionRadius(sphere, height);
            if (!float.IsNaN(circleRadius))
                DebugDrawCircle(sphere.Position.Xz, circleRadius, sphere.Color);
        }
        
        // Draw eleven rays
        for (var i = 0; i < 11; i++)
        {
            // Primary rays
            var ratioAlongScreen = i / 10.0f;
            var posAlongScreen = leftVec + ratioAlongScreen * (rightVec - leftVec);
            var ray = new Ray(_camera.Position, posAlongScreen.Normalized());
            var intersection = _scene.ClosestIntersection(ray);
            var distance = intersection?.Distance ?? 100.0f;
            var intersectLocation = ray.Evaluate(distance);
            var pos = DebugWorldToScreen(intersectLocation);
            Display.Line(camera.X, camera.Y, pos.X, pos.Y, 0xff_ff_00);
            
            // Shadow rays
            if (intersection is null) continue;
            foreach (var light in _scene.LightSources)
            {
                var shadowRayDirection = light.Location - intersectLocation;
                shadowRayDirection.Normalize();
                var shadowRay = new Ray(intersectLocation, shadowRayDirection);
                var shadowIntersection = _scene.ClosestIntersection(shadowRay);
                var shadowDistance = shadowIntersection?.Distance ?? 100.0f;
                var shadowIntersectionLocation = shadowRay.Evaluate(shadowDistance);
                var shadowPos = DebugWorldToScreen(shadowIntersectionLocation);
                Display.Line(pos.X, pos.Y, shadowPos.X, shadowPos.Y, 0x80_80_00);
            }
        }

        // To test: move green sphere up
        switch (_scene.Primitives[2])
        {
            case Sphere sphere:
                sphere.Position.Y += 1.0f / 32.0f;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private Vector2i DebugWorldToScreen(Vector3 vec)
    {
        // Flatten; assume the vector is in the right y plane
        return DebugWorldToScreen(vec.Xz);
    }

    private Vector2i DebugWorldToScreen(Vector2 vec)
    {
        // Amount of pixels per unit lenght
        const int scale = 32;
        // Let the origin be at the middle-bottom
        Vector2i originInScreenSpace = (Display.Width / 2, Display.Height * 7 / 8);

        // Convert to integer representation
        var pixel = originInScreenSpace + (Vector2i)(vec * scale);

        return pixel;
    }

    /// <summary>
    ///     Find the radius of the circle that forms the intersection of a sphere with a certain y-plane.
    /// </summary>
    /// <param name="sphere">The sphere to calculate the intersection for.</param>
    /// <param name="height">The y-value of the intersection plane.</param>
    /// <returns>
    ///     The radius when there is an intersection.
    ///     NaN if there is no intersection.
    /// </returns>
    private static float DebugSpherePlaneIntersectionRadius(Sphere sphere, float height)
    {
        // The radius we are looking for is:
        //  sqrt( r^2 - (a - k)^2 )
        // where: r is the radius of the sphere
        //        a is the plane height value
        //        k is the y-value of the sphere center

        return float.Sqrt(sphere.Radius * sphere.Radius - (height - sphere.Position.Y) * (height - sphere.Position.Y));
    }

    private void DebugDrawCircle(Vector2 center, float radius, Vector3 color)
    {
        const int segments = 64;
        var displayColor = ConvertColor(color);

        for (var i = 0; i < segments; i++)
        {
            var angleStart = i * 2 * float.Pi / segments;
            var angleEnd = (i + 1) * 2 * float.Pi / segments;
            var segmentStart = center + Matrix2.CreateRotation(angleStart) * (radius * Vector2.UnitX);
            var segmentEnd = center + Matrix2.CreateRotation(angleEnd) * (radius * Vector2.UnitX);
            var lineStart = DebugWorldToScreen(segmentStart);
            var lineEnd = DebugWorldToScreen(segmentEnd);
            Display.Line(lineStart.X, lineStart.Y, lineEnd.X, lineEnd.Y, displayColor);
        }
    }

    #endregion
}

public struct Ray
{
    public Vector3 Base;

    /// <summary>
    ///     Assumed to be unit length.
    /// </summary>
    public Vector3 Direction;

    public Ray(Vector3 @base, Vector3 direction)
    {
        Base = @base;
        Direction = direction;
    }

    public Vector3 Evaluate(float t)
    {
        return Base + t * Direction;
    }
}