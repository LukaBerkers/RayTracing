namespace RayTracing;

public static class Helper
{
    public static bool IsZero(float value)
    {
        return value == 0.0f || float.IsSubnormal(value);
    }
    
    public static int Compare(float left, float right)
    {
        // This will probably need a bigger error margin in the future, for now this works.
        return IsZero(left - right) ? 0 : left.CompareTo(right);
    }
}