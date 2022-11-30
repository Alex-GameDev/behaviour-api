namespace BehaviourAPI.Core
{
    public static class MathUtilities
    {
        /// <summary>
        /// Clamp the float value between 0 and 1.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The value clamped between 0 and 1.</returns>
        public static float Clamp01(float value)
        {
            if (0 <= value && value <= 1) return value;
            else if (value < 0) return 0;
            else return 1;
        }

        /// <summary>
        /// Clamp the float value between 0 and 1.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The value clamped between 0 and 1.</returns>
        public static double Clamp01(double value)
        {
            if (0 <= value && value <= 1) return value;
            else if (value < 0) return 0;
            else return 1;
        }
    }
}
