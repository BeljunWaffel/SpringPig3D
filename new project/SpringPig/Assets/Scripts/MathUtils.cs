public static class MathUtils
{
    // Returns true if the numbers are both positive or both negative
    public static bool IsSameSign(float x, float y)
    {
        return (x < 0 && y < 0) || (x >= 0 && y >= 0);
    }

    // Returns 1 if positive, -1 if negative
    public static int IsPositive(float x)
    {
        return (x >= 0 ? 1 : -1);
    }
}
