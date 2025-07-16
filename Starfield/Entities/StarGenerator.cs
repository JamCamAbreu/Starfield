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
        public const int ASTEROID_SPRITE_SIZE = 128;
        public const int ASTEROID_SPRITE_ROWS = 7;
        public const int ASTEROID_SPRITE_COLS = 7;
        public const int ASTEROID_ANIMATION_SPEED_MIN = 12;
        public const int ASTEROID_ANIMATION_SPEED_MAX = 30;
        public const int ASTEROID_WARNING_TIMER = 30;

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
        public int RecenterChanceMin { get; set; }
        public int RecenterChanceMax { get; set; }
        public int AsteroidFieldTimer { get; set; }
        public int AsteroidFieldTimerMin { get; set; }
        public int AsteroidFieldTimerMax { get; set; }
        public bool EnteredAsteroidField { get; set; }
        public int WarningTimer { get; set; }

        public List<SpaceObject> Stars { get; set; }
        public List<SpaceObject> Asteroids { get; set; }

        public StarGenerator()
        {
            timerMin = 1;
            timerMax = 3;
            numStarsGenerateMin = 4;
            numStarsGenerateMax = 28;
            ResetGeneratorTimer();
            Stars = new List<SpaceObject>();
            Asteroids = new List<SpaceObject>();

            FocalPointX = Graphics.Current.ScreenMidX;
            FocalPointY = Graphics.Current.ScreenMidY;
            FocalPointTargetX = Graphics.Current.ScreenMidX;
            FocalPointTargetY = Graphics.Current.ScreenMidY;
            FocalPointTimerMin = 60 * 1;
            FocalPointTimerMax = 60 * 4;
            ResetFocalTimer();
            FocalPointMaxDistance = 600;

            SpeedMultiplier = 1.0f;
            TargetSpeedMultiplier = 1.0f;
            SpeedMultiplierMin = 0.98f;
            SpeedMultiplierMax = 1.05f;

            RecenterChanceMax = 50;
            RecenterChanceMin = 0;

            AsteroidFieldTimerMin = 60 * 5;
            AsteroidFieldTimerMax = 60 * 14;
            ResetAsteroidFieldTimer();
            EnteredAsteroidField = false;
        }
        protected void GenerateStars()
        {
            int num = Ran.Current.Next(numStarsGenerateMin, numStarsGenerateMax + (int)(SpeedMultiplier * 12));
            for (int i = 0; i < num; i++)
            {
                SpaceObject star = new SpaceObject();

                // Use Gaussian distribution to spawn stars near focal point
                float stdDevX = Graphics.Current.ScreenWidth / 4.0f; // Controls spread in X
                float stdDevY = Graphics.Current.ScreenHeight / 4.0f; // Controls spread in Y
                float x = FocalPointX + (float)Ran.Current.NextGaussian() * stdDevX;
                float y = FocalPointY + (float)Ran.Current.NextGaussian() * stdDevY;

                // Clamp to ensure stars stay within screen bounds
                x = MathHelper.Clamp(x, 0, Graphics.Current.ScreenWidth);
                y = MathHelper.Clamp(y, 0, Graphics.Current.ScreenHeight);
                star.SetAbsolutePosition(x, y);

                float dx = star.Xpos - FocalPointX;
                float dy = star.Ypos - FocalPointY;
                float length = MathF.Sqrt(dx * dx + dy * dy);
                if (length > 0)
                {
                    star.Xspeed = (dx / length) * SpaceObject.BASE_SPEED * SpeedMultiplier;
                    star.Yspeed = (dy / length) * SpaceObject.BASE_SPEED * SpeedMultiplier;
                }

                int asteroidChance = EnteredAsteroidField ? 992 : 0;
                if (Ran.Current.Next(asteroidChance, 1000) < 998) { SetStar(star); }
                else { SetAsteroid(star); }
            }
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

            AsteroidFieldTimer--;
            if (AsteroidFieldTimer <= 0)
            {
                EnteredAsteroidField = false;
                ResetAsteroidFieldTimer();
                if (Ran.Current.Next(0, 3) == 0)
                {
                    EnteredAsteroidField = true;
                    TargetSpeedMultiplier = SpeedMultiplierMin;
                }
            }
            WarningTimer--;
            if (WarningTimer < -ASTEROID_WARNING_TIMER)
            {
                WarningTimer = ASTEROID_WARNING_TIMER;
            }

            foreach (SpaceObject star in Stars.Union(Asteroids))
            {
                UpdateStarVelocity(star);
                star.Update(SpeedMultiplier); // Pass SpeedMultiplier to Update
            }

            timer--;
            if (timer <= 0)
            {
                GenerateStars();
                ResetGeneratorTimer();
            }
            CleanUpStars();
        }
        public void Draw()
        {
            Graphics.Current.SpriteB.Begin();

            Rectangle rect = new Rectangle(0, 0, 8, 8);
            foreach (var star in Stars)
            {
                UpdateAnimation(star);

                rect.X = (int)star.Xpos;
                rect.Y = (int)star.Ypos;
                rect.Width = (int)(8 * star.Scale);
                rect.Height = (int)(8 * star.Scale);
                Graphics.Current.SpriteB.Draw(
                    star.Sprite,
                    rect,
                    star.SourceSpriteRect,
                    star.DrawColor
                );
            }

            foreach (var asteroid in Asteroids)
            {
                UpdateAnimation(asteroid);

                rect.X = (int)asteroid.Xpos;
                rect.Y = (int)asteroid.Ypos;
                rect.Width = (int)(8 * asteroid.Scale);
                rect.Height = (int)(8 * asteroid.Scale);
                Graphics.Current.SpriteB.Draw(
                    asteroid.Sprite,
                    rect,
                    asteroid.SourceSpriteRect,
                    asteroid.DrawColor
                );
            }

            if (EnteredAsteroidField && WarningTimer > 0)
            {
                Graphics.Current.DrawString("Asteroid Field!", new Vector2(Graphics.Current.ScreenMidX, 200), new Font(Color.DarkRed, Font.Type.arial, Font.Size.SIZE_S), true, true, false);
            }

            //Graphics.Current.DrawString($"Stars: {Stars.Count()}", new Vector2(0, 0), new Font(Color.White, Font.Type.arial, Font.Size.SIZE_S), false, false, false);
            //Graphics.Current.DrawString($"Asteroids: {Asteroids.Count()}", new Vector2(0, 60), new Font(Color.White, Font.Type.arial, Font.Size.SIZE_S), false, false, false);

            Graphics.Current.SpriteB.End();
        }

        #region Misc
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
            List<SpaceObject> deleteEntities = new List<SpaceObject>();
            foreach (var entity in Stars.Union(Asteroids))
            {
                if (
                    entity.Xpos > Graphics.Current.ScreenWidth ||
                    entity.Xpos < 0 ||
                    entity.Ypos > Graphics.Current.ScreenHeight ||
                    entity.Ypos < 0
                    )
                {
                    deleteEntities.Add(entity);
                }
            }
            foreach (var entity in deleteEntities)
            {
                Stars.Remove(entity);
                Asteroids.Remove(entity);
            }
        }
        #endregion

        #region Focal Point and Speed Changes
        protected void SetFocalPointRandom()
        {
            bool recenter = Ran.Current.Next(RecenterChanceMin, RecenterChanceMax) > (RecenterChanceMax / 2);

            if (recenter)
            {
                RecenterChanceMin = 0;
                FocalPointTargetX = Graphics.Current.ScreenMidX;
                FocalPointTargetY = Graphics.Current.ScreenMidY;
            }
            else
            {
                RecenterChanceMin += 7;
                FocalPointTargetX = Ran.Current.Next(
                    Graphics.Current.ScreenMidX - FocalPointMaxDistance / 2,
                    Graphics.Current.ScreenMidX + FocalPointMaxDistance / 2);
                FocalPointTargetY = Ran.Current.Next(
                    Graphics.Current.ScreenMidY - FocalPointMaxDistance / 2,
                    Graphics.Current.ScreenMidY + FocalPointMaxDistance / 2);
            }

            RecenterChanceMin = Math.Clamp(RecenterChanceMin, 0, RecenterChanceMax);
        }
        protected void SetSpeedMultiplierRandom()
        {
            TargetSpeedMultiplier = Ran.Current.Next(SpeedMultiplierMin, SpeedMultiplierMax);
        }
        #endregion

        #region Timers 
        protected void ResetGeneratorTimer()
        {
            timer = Ran.Current.Next(timerMin, timerMax);
        }
        protected void ResetFocalTimer()
        {
            FocalPointTimer = Ran.Current.Next(FocalPointTimerMin, FocalPointTimerMax);
        }
        protected void ResetAsteroidFieldTimer()
        {
            AsteroidFieldTimer = Ran.Current.Next(AsteroidFieldTimerMin, AsteroidFieldTimerMax);
        }
        #endregion

        #region Stars and Asteroids
        protected void SetStar(SpaceObject star)
        {
            star.Sprite = Graphics.Current.SpritesByName["Star"];
            star.Scale = Ran.Current.Next(0.05f, 0.25f);
            star.Acceleration = Ran.Current.Next(1.01f, 1.04f);

            // Set Colors
            if (Ran.Current.Next(0, 10) >= 8)
            {
                Color rancolor = new Color(
                    Ran.Current.Next(0, 255),
                    Ran.Current.Next(0, 255),
                    Ran.Current.Next(0, 255));
                rancolor = Color.Lerp(rancolor, Color.White, Ran.Current.Next(0.6f, 0.85f));
                star.DrawColor = rancolor;
            }
            else
            {
                star.DrawColor = Color.White;
            }

            Stars.Add(star);
        }
        protected void SetAsteroid(SpaceObject asteroid)
        {
            asteroid.Sprite = Graphics.Current.SpritesByName["asteroid"];
            asteroid.Scale = Ran.Current.Next(0.1f, 1.5f);
            asteroid.Acceleration = Ran.Current.Next(1.001f, 1.0025f);

            asteroid.AnimationSpeed = (int)(ASTEROID_ANIMATION_SPEED_MAX/2 * asteroid.Scale);
            Math.Clamp(asteroid.AnimationSpeed, ASTEROID_ANIMATION_SPEED_MIN, ASTEROID_ANIMATION_SPEED_MAX);
            Color[] colors = new Color[]
                {
                    Color.Brown,
                    Color.Blue, 
                    Color.White,
                    Color.Black 
                };
            Color rancolor = colors[Ran.Current.Next(0, 3)];
            asteroid.DrawColor = Color.Lerp(rancolor, Color.White, Ran.Current.Next(0.5f, 1f));
            asteroid.SourceSpriteRect = new Rectangle(0, 0, ASTEROID_SPRITE_SIZE, ASTEROID_SPRITE_SIZE);
            asteroid.ScaleIncrease = 0.05f * asteroid.Acceleration;

            Asteroids.Add(asteroid);
        }
        protected void IncrementAnimation(SpaceObject star)
        {
            if (star.SourceSpriteRect == null) return;
            
            star.SourceSpriteCol++;
            if (star.SourceSpriteCol >= ASTEROID_SPRITE_COLS)
            {
                star.SourceSpriteCol = 0;
                star.SourceSpriteRow++;
            }
            if (star.SourceSpriteRow >= ASTEROID_SPRITE_ROWS)
            {
                star.SourceSpriteRow = 0;
            }

            star.SourceSpriteRect = new Rectangle(
                star.SourceSpriteRow * ASTEROID_SPRITE_SIZE,
                star.SourceSpriteCol * ASTEROID_SPRITE_SIZE,
                ASTEROID_SPRITE_SIZE, ASTEROID_SPRITE_SIZE);
        }
        protected void UpdateAnimation(SpaceObject star)
        {
            if (star.SourceSpriteRect == null) return;
            star.AnimationTimer++;
            if (star.AnimationTimer > star.AnimationSpeed)
            {
                star.AnimationTimer = 0;
                IncrementAnimation(star);
            }
        }
        #endregion
    }
}
