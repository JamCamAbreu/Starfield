using HPScreen.Admin;
using HPScreen.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Starfield.Entities
{
    public class StarGenerator
    {
        protected int timer { get; set; }
        protected int timerMin { get; set; }
        protected int timerMax { get; set; }
        protected int numStarsGenerateMin { get; set; }
        protected int numStarsGenerateMax { get; set; }
        public float FocalPointX { get; set; }
        public float FocalPointY { get; set; }
        public float FocalPointTargetX { get; set; }
        public float FocalPointTargetY { get; set; }
        public int FocalPointTimer { get; set; }
        public int FocalPointTimerMin { get; set; }
        public int FocalPointTimerMax { get; set; }
        public int FocalPointMaxDistance { get; set; }
        public float SpeedMultiplier { get; set; } // Changed from static
        public float TargetSpeedMultiplier { get; set; }
        public float SpeedMultiplierMin { get; set; }
        public float SpeedMultiplierMax { get; set; }
        public List<SpaceObject> Stars { get; set; }

        public StarGenerator()
        {
            timerMin = 1;
            timerMax = 3;
            numStarsGenerateMin = 4;
            numStarsGenerateMax = 28;
            ResetTimer();
            Stars = new List<SpaceObject>();

            FocalPointX = Graphics.Current.ScreenMidX;
            FocalPointY = Graphics.Current.ScreenMidY;
            FocalPointTargetX = Graphics.Current.ScreenMidX;
            FocalPointTargetY = Graphics.Current.ScreenMidY;
            FocalPointTimerMin = 60 * 2;
            FocalPointTimerMax = 60 * 15;
            ResetFocalTimer();
            FocalPointMaxDistance = 1000;

            SpeedMultiplier = 1.0f;
            TargetSpeedMultiplier = 1.0f;
            SpeedMultiplierMin = 0.98f;
            SpeedMultiplierMax = 1.05f;
        }

        public void Update()
        {
            FocalPointTimer--;
            if (FocalPointTimer <= 0)
            {
                SetFocalPointRandom();
                SetSpeedMultiplierRandom();
                ResetFocalTimer();
            }
            FocalPointX = Global.Ease(FocalPointX, FocalPointTargetX, 0.05f);
            FocalPointY = Global.Ease(FocalPointY, FocalPointTargetY, 0.05f);
            SpeedMultiplier = Global.Ease(SpeedMultiplier, TargetSpeedMultiplier, 0.02f);

            foreach (SpaceObject star in Stars)
            {
                UpdateStarVelocity(star);
                star.Update(SpeedMultiplier); // Pass SpeedMultiplier to Update
            }

            timer--;
            if (timer <= 0)
            {
                GenerateStar();
                ResetTimer();
            }
            CleanUpStars();
        }

        public void Draw()
        {
            Rectangle rect = new Rectangle(0, 0, 8, 8);
            foreach (var star in Stars)
            {
                rect.X = (int)star.Xpos;
                rect.Y = (int)star.Ypos;
                rect.Width = (int)(8 * star.Scale);
                rect.Height = (int)(8 * star.Scale);
                Graphics.Current.SpriteB.Begin();
                Graphics.Current.SpriteB.Draw(
                    star.Sprite,
                    rect,
                    Color.White
                );
                Graphics.Current.SpriteB.End();
            }
        }

        protected void ResetTimer()
        {
            timer = Ran.Current.Next(timerMin, timerMax);
        }

        protected void GenerateStar()
        {
            int num = Ran.Current.Next(numStarsGenerateMin, numStarsGenerateMax + (int)(SpeedMultiplier * 12));
            for (int i = 0; i < num; i++)
            {
                SpaceObject star = new SpaceObject();
                star.Sprite = Graphics.Current.SpritesByName["Star"];

                // Use Gaussian distribution to spawn stars near focal point
                float stdDevX = Graphics.Current.ScreenWidth / 4.0f; // Controls spread in X
                float stdDevY = Graphics.Current.ScreenHeight / 4.0f; // Controls spread in Y
                float x = FocalPointX + (float)Ran.Current.NextGaussian() * stdDevX;
                float y = FocalPointY + (float)Ran.Current.NextGaussian() * stdDevY;

                // Clamp to ensure stars stay within screen bounds
                x = MathHelper.Clamp(x, 0, Graphics.Current.ScreenWidth);
                y = MathHelper.Clamp(y, 0, Graphics.Current.ScreenHeight);
                star.SetAbsolutePosition(x, y);
                star.Scale = Ran.Current.Next(0.01f, 0.25f);

                float dx = star.Xpos - FocalPointX;
                float dy = star.Ypos - FocalPointY;
                float length = MathF.Sqrt(dx * dx + dy * dy);
                if (length > 0)
                {
                    star.Xspeed = (dx / length) * SpaceObject.BASE_SPEED * SpeedMultiplier;
                    star.Yspeed = (dy / length) * SpaceObject.BASE_SPEED * SpeedMultiplier;
                }

                Stars.Add(star);
            }
        }

        protected void UpdateStarVelocity(SpaceObject star)
        {
            float dx = star.Xpos - FocalPointX;
            float dy = star.Ypos - FocalPointY;
            float length = MathF.Sqrt(dx * dx + dy * dy);
            float currentSpeed = MathF.Sqrt(star.Xspeed * star.Xspeed + star.Yspeed * star.Yspeed);
            if (length > 0)
            {
                star.Xspeed = (dx / length) * currentSpeed * SpeedMultiplier;
                star.Yspeed = (dy / length) * currentSpeed * SpeedMultiplier;
            }

            if (star.Xspeed > -1 && star.Xspeed < 1) { star.Xspeed *= 2; }
            if (star.Yspeed > -1 && star.Yspeed < 1) { star.Yspeed *= 2; }
        }

        protected void CleanUpStars()
        {
            List<SpaceObject> deleteStars = new List<SpaceObject>();
            foreach (var star in Stars)
            {
                if (
                    star.Xpos > Graphics.Current.ScreenWidth ||
                    star.Xpos < 0 ||
                    star.Ypos > Graphics.Current.ScreenHeight ||
                    star.Ypos < 0
                    )
                {
                    deleteStars.Add(star);
                }
            }
            foreach (var star in deleteStars)
            {
                Stars.Remove(star);
            }
        }

        protected void ResetFocalTimer()
        {
            FocalPointTimer = Ran.Current.Next(FocalPointTimerMin, FocalPointTimerMax);
        }

        protected void SetFocalPointRandom()
        {
            FocalPointTargetX = Ran.Current.Next(
                Graphics.Current.ScreenMidX - FocalPointMaxDistance / 2,
                Graphics.Current.ScreenMidX + FocalPointMaxDistance / 2);
            FocalPointTargetY = Ran.Current.Next(
                Graphics.Current.ScreenMidY - FocalPointMaxDistance / 2,
                Graphics.Current.ScreenMidY + FocalPointMaxDistance / 2);
        }

        protected void SetSpeedMultiplierRandom()
        {
            TargetSpeedMultiplier = Ran.Current.Next(SpeedMultiplierMin, SpeedMultiplierMax);
        }
    }
}
