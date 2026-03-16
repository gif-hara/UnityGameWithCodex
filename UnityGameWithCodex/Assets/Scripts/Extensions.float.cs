namespace UnityGameWithCodex
{
    public static class FloatExtensions
    {
        public static float NormalizeAngle(this float angle)
        {
            if (angle > 180f)
            {
                angle -= 360f;
            }

            return angle;
        }
    }
}
