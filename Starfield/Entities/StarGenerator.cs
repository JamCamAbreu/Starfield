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
        protected int timer;
        protected int timerMin;
        protected int timerMax;
        protected int numStarsGenerateMin;
        protected int numStarsGenerateMax;

        public List<SpaceObject> Stars { get; set; }
        public StarGenerator() 
        {
            timerMin = 1;
            timerMax = 4;
            numStarsGenerateMin = 1;
            numStarsGenerateMax = 18;
            ResetTimer();
            Stars = new List<SpaceObject>();
        }

        public void Update()
        {
            foreach (SpaceObject star in Stars)
            {
                star.Update();
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
            int num = Ran.Current.Next(numStarsGenerateMin, numStarsGenerateMax);
            for(int i = 0; i < num; i++)
            {
                SpaceObject star = new SpaceObject();
                star.Sprite = Graphics.Current.SpritesByName["Star"];
                star.SetAbsolutePosition(
                    Ran.Current.Next(100, Graphics.Current.ScreenWidth - 100),
                    Ran.Current.Next(100, Graphics.Current.ScreenHeight - 100));
                star.Scale = Ran.Current.Next(0.01f, 0.25f);

                float centerX = Graphics.Current.ScreenMidX;
                float centerY = Graphics.Current.ScreenMidY;

                // Calculate direction vector to edge
                float dx = star.Xpos - centerX;
                float dy = star.Ypos - centerY;
                float length = MathF.Sqrt(dx * dx + dy * dy);

                // Normalize and scale with speed
                star.Xspeed = (dx / length) * SpaceObject.BASE_SPEED;
                star.Yspeed = (dy / length) * SpaceObject.BASE_SPEED;

                Stars.Add(star);
            }
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
    }
}
