using HPScreen.Admin;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPScreen.Entities
{
    public class SpaceObject
    {
        public const float BASE_SPEED = 1.15f;
        public SpaceObject()
        {
            Xpos = 0;
            Ypos = 0;
            Scale = 1;
            
            Acceleration = Ran.Current.Next(1.01f, 1.025f);
            ScaleIncrease = 0.015f * Acceleration;
        }
        public float Xpos { get; set; }
        public float Ypos { get; set; }
        public float Scale { get; set; }
        public float Xspeed { get; set; }
        public float Yspeed { get; set; }
        public float Acceleration { get; set; }
        public float AccelerationMax { get; set; }
        public float ScaleIncrease { get; set; }
        public Texture2D Sprite { get; set; }
        public void SetAbsolutePosition(float x, float y)
        {
            this.Xpos = x;
            this.Ypos = y;
        }
        public void Update()
        {
            Scale += ScaleIncrease;
            Xspeed *= Acceleration;
            Yspeed *= Acceleration;

            Xpos += Xspeed;
            Ypos += Yspeed;
        }
    }
}
