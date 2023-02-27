namespace ThrustDoll
{
    public static class Util
    {
        public static float BezierComponent(float p1y, float p2y, float t)
        {
            float a = 1 - 3 * p2y + 3 * p1y;
            float b = 3 * p2y - 6 * p1y;
            float c = 3 * p1y;

            return a*t*t*t + b*t*t + c*t;
        }
    }
}