using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPScreen.Admin
{
    public class Ran
    {
        #region Singleton Implementation
        private static Ran instance;
        private static object _lock = new object();
        private Ran()
        {
            this._random = new Random();
        }
        public static Ran Current
        {
            get
            {
                if (instance == null)
                {
                    lock (_lock)
                    {
                        if (instance == null)
                        {
                            instance = new Ran();
                        }
                    }
                }
                return instance;
            }
        }
        #endregion

        private Random _random { get; set; }

        public float NextAngleRadians(float minAngleDegrees = 0f, float maxAngleDegrees = 360f)
        {
            // Convert min and max angles from degrees to radians
            float minAngleRadians = MathHelper.ToRadians(minAngleDegrees);
            float maxAngleRadians = MathHelper.ToRadians(maxAngleDegrees);

            // Get a random angle in radians between min and max angles
            float randomAngleRadians = (float)(_random.NextDouble() * (maxAngleRadians - minAngleRadians) + minAngleRadians);

            return randomAngleRadians;
        }
        public float NextAngleDegrees(float minAngle = 0f, float maxAngle = 360f)
        {
            // Generate a random value between 0 and 1
            float randomValue = (float)_random.NextDouble();

            // Calculate the angle within the range
            float angle = minAngle + (maxAngle - minAngle) * randomValue;

            return angle;
        }

        public double NextDouble()
        {
            return this._random.NextDouble();
        }
        public int Next(int min, int max)
        {
            return this._random.Next(min, max + 1);
        }
        public float Next(float min, float max)
        {
            return ((max - min) * (float)this._random.NextDouble()) + min;
        }
        public float NextGaussian()
        {
            // Box-Muller transform
            float u1 = (float)_random.NextDouble(); // Uniform(0,1]
            float u2 = (float)_random.NextDouble(); // Uniform(0,1]
                                                    // Avoid log(0) by ensuring u1 is not 0
            if (u1 == 0) u1 = float.Epsilon;
            float z = MathF.Sqrt(-2.0f * MathF.Log(u1)) * MathF.Cos(2.0f * MathF.PI * u2);
            return z;
        }
    }
}
