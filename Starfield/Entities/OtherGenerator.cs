using HPScreen.Admin;
using HPScreen.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Starfield.Entities
{
    public class OtherGenerator
    {
        public Dictionary<string, (int minValue, int maxValue)> ObjectTimerDefinitions = new ()
        {
            { "ryanhead", (200, 3500) },
            { "alehead", (200, 3500) },
            { "camhead", (200, 3500) },
            { "dakotahead", (200, 3500) },
            { "emilyhead", (200, 3500) },
            { "juanhead", (200, 3500) },
            { "juvalhead", (200, 3500) },
        };
        public Dictionary<string, int> ObjectTimers { get; set; }

        public List<SpaceObject> Things { get; set; }
        public OtherGenerator()
        {
            Things = new List<SpaceObject>();
            ObjectTimers = new Dictionary<string, int>();
            foreach (var (key, (min, max)) in ObjectTimerDefinitions)
            {
                int time = Ran.Current.Next(min, max);
                ObjectTimers.Add(key, time);
            }
        }

        public void Update()
        {
            foreach (SpaceObject thing in Things)
            {
                thing.Update();
            }

            foreach (var (key, time) in ObjectTimers)
            {
                ObjectTimers[key]--;
                if (ObjectTimers[key] <= 0)
                {
                    GenerateSpaceObject(key);
                    ObjectTimers[key] = Ran.Current.Next(ObjectTimerDefinitions[key].minValue, ObjectTimerDefinitions[key].maxValue);
                }
            }
            CleanUpThings();
        }
        public void Draw()
        {
            Rectangle rect = new Rectangle(0, 0, 128, 128);
            foreach (var thing in Things)
            {
                rect.X = (int)thing.Xpos;
                rect.Y = (int)thing.Ypos;
                rect.Width = (int)(thing.Scale * thing.Sprite.Width);
                rect.Height = (int)(thing.Scale * thing.Sprite.Height);
                Graphics.Current.SpriteB.Begin();
                Graphics.Current.SpriteB.Draw(
                    thing.Sprite,
                    rect,
                    Color.Gray
                );
                Graphics.Current.SpriteB.End();
            }
        }
        protected void GenerateSpaceObject(string spritename)
        {
            SpaceObject thing = new SpaceObject();
            thing.Sprite = Graphics.Current.SpritesByName[spritename];
            thing.SetAbsolutePosition(
                Ran.Current.Next(300, Graphics.Current.ScreenWidth - 300),
                Ran.Current.Next(300, Graphics.Current.ScreenHeight - 300));
            thing.Scale = 0.1f;

            float centerX = Graphics.Current.ScreenMidX;
            float centerY = Graphics.Current.ScreenMidY;

            // Calculate direction vector to edge
            float dx = thing.Xpos - centerX;
            float dy = thing.Ypos - centerY;
            float length = MathF.Sqrt(dx * dx + dy * dy);

            // Normalize and scale with speed
            thing.Xspeed = (dx / length) * SpaceObject.BASE_SPEED;
            thing.Yspeed = (dy / length) * SpaceObject.BASE_SPEED;

            Things.Add(thing);
        }
        protected void CleanUpThings()
        {
            List<SpaceObject> deleteThings = new List<SpaceObject>();
            int pad = 256;
            foreach (var thing in Things)
            {
                if (
                    thing.Xpos > Graphics.Current.ScreenWidth + pad ||
                    thing.Xpos < 0 - pad ||
                    thing.Ypos > Graphics.Current.ScreenHeight + pad ||
                    thing.Ypos < 0 - pad
                    )
                {
                    deleteThings.Add(thing);
                }
            }
            foreach (var thing in deleteThings)
            {
                Things.Remove(thing);
            }
        }
    }
}
