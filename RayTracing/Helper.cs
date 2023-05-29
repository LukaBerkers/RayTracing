using System.Diagnostics;

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

    public static QuadraticSolution SolveQuadratic(float a, float b, float c)
    {
        // First part compares if b^2 == 4ac, that is, the discriminant is zero.
        // Second part handles the case where one of a or c is zero and the other is infinite.
        if (Compare(b * b, 4.0f * a * c) == 0 || float.IsNaN(4.0f * a * c))
            return new QuadraticSolution.One(-b / (2.0f * a));

        var discriminant = b * b - 4.0f * a * c;
        switch (discriminant)
        {
            case < 0.0f:
                return new QuadraticSolution.None();
            case > 0.0f:
            {
                var dRoot = float.Sqrt(discriminant);
                return a switch
                {
                    // This split makes sure that Val1 <= Val2
                    < 0.0f => new QuadraticSolution.Two((-b + dRoot) / (2.0f * a), (-b - dRoot) / (2.0f * a)),
                    // remaining cases: a >= 0.0f || float.IsNaN(a)
                    _ => new QuadraticSolution.Two((-b - dRoot) / (2.0f * a), (-b + dRoot) / (2.0f * a))
                };
            }
            // All of the ways these last two cases could occur are already handled by the if statement above
            case 0.0f:
                throw new UnreachableException();
            case float.NaN:
                throw new UnreachableException();
        }
    }
}

public abstract class QuadraticSolution
{
    public class None : QuadraticSolution
    {
    }

    public class One : QuadraticSolution
    {
        public One(float value)
        {
            Value = value;
        }

        public float Value { get; }
    }

    public class Two : QuadraticSolution
    {
        public Two(float val1, float val2)
        {
            Val1 = val1;
            Val2 = val2;
        }

        public float Val1 { get; }
        public float Val2 { get; }
    }
}