namespace RayTracing;

public static class Helper
{
    public static bool IsZero(float value)
    {
        return value == 0.0f || float.IsSubnormal(value);
    }

    public static int Compare(float left, float right)
    {
        throw new NotImplementedException();
    }
}