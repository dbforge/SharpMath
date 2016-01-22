using System;

namespace SharpMath.Trigonometry
{
    public class Converter
    {
        public static double DegreesToRadians(double degrees)
        {
            return degrees * (Math.PI / 180);
        }

        public static double RadiansToDegrees(double degrees)
        {
            return degrees * (180 / Math.PI);
        }
    }
}