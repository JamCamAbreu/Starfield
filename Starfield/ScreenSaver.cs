﻿using HPScreen.Admin;
using HPScreen.Entities;
using Microsoft.VisualBasic.ApplicationServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct2D1;
using SharpDX.Direct3D9;
using Starfield.Entities;
using System;
using System.Collections.Generic;

namespace HPScreen
{
    public class ScreenSaver : Game
    {
        private GraphicsDeviceManager _graphics;
        private Microsoft.Xna.Framework.Graphics.SpriteBatch _spriteBatch;
        private KeyboardState _currentKeyboardState;
        private KeyboardState _previousKeyboardState;
        private MouseState _currentMouseState;
        private MouseState _previousMouseState;
        private int loadFrames = 0;
        private const int LOAD_FRAMES_THRESH = 10;
        StarGenerator StarGenerator;

        protected bool RunSetup { get; set; }
        public ScreenSaver()
        {
            Graphics.Current.GraphicsDM = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;

            // Set the following property to false to ensure alternating Step() and Draw() functions
            // Set the property to true to (hopefully) improve game smoothness by ignoring some draw calls if needed.
            IsFixedTimeStep = false;

            RunSetup = true;
        }
        protected override void Initialize()
        {
            // Note: This takes places BEFORE LoadContent()
            Graphics.Current.Init(this.GraphicsDevice, this.Window, true);
            base.Initialize();
        }
        protected override void LoadContent()
        {
            Graphics.Current.SpriteB = new Microsoft.Xna.Framework.Graphics.SpriteBatch(Graphics.Current.GraphicsDM.GraphicsDevice);

            // Add your Sprites here like the following:
            Graphics.Current.SpritesByName.Add("Star", Content.Load<Texture2D>("Sprites/Star"));
            Graphics.Current.SpritesByName.Add("rocket", Content.Load<Texture2D>("Sprites/rocket"));
            Graphics.Current.SpritesByName.Add("ryanhead", Content.Load<Texture2D>("Sprites/RyanHead"));
            Graphics.Current.SpritesByName.Add("alehead", Content.Load<Texture2D>("Sprites/AlejandroHead"));
            Graphics.Current.SpritesByName.Add("camhead", Content.Load<Texture2D>("Sprites/CamHead"));
            Graphics.Current.SpritesByName.Add("dakotahead", Content.Load<Texture2D>("Sprites/DakotaHead"));
            Graphics.Current.SpritesByName.Add("emilyhead", Content.Load<Texture2D>("Sprites/EmilyHead"));
            Graphics.Current.SpritesByName.Add("juanhead", Content.Load<Texture2D>("Sprites/JuanHead"));
            Graphics.Current.SpritesByName.Add("juvalhead", Content.Load<Texture2D>("Sprites/juval"));
            Graphics.Current.SpritesByName.Add("bug1", Content.Load<Texture2D>("Sprites/bug1"));
            Graphics.Current.SpritesByName.Add("bug2", Content.Load<Texture2D>("Sprites/bug2"));
            Graphics.Current.SpritesByName.Add("bug3", Content.Load<Texture2D>("Sprites/bug3"));
            Graphics.Current.SpritesByName.Add("laser", Content.Load<Texture2D>("Sprites/laser"));
            Graphics.Current.SpritesByName.Add("perr", Content.Load<Texture2D>("Sprites/Perr"));
            Graphics.Current.SpritesByName.Add("asteroid", Content.Load<Texture2D>("Sprites/asteroid"));
            Graphics.Current.SpritesByName.Add("spacecockpit", Content.Load<Texture2D>("Sprites/spacecockpit"));

            Graphics.Current.Fonts = new Dictionary<string, SpriteFont>();
            Graphics.Current.Fonts.Add("arial-48", Content.Load<SpriteFont>($"Fonts/arial_48"));
            Graphics.Current.Fonts.Add("arial-72", Content.Load<SpriteFont>($"Fonts/arial_72"));
            Graphics.Current.Fonts.Add("arial-96", Content.Load<SpriteFont>($"Fonts/arial_96"));
            Graphics.Current.Fonts.Add("arial-144", Content.Load<SpriteFont>($"Fonts/arial_144"));
        }
        protected override void Update(GameTime gameTime)
        {
            CheckInput(); // Used to exit game when input detected (aka screensaver logic)
            if (RunSetup) { Setup(); }

            StarGenerator.Update();
            //otherGenerator.Update();

            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            // Color that the screen is wiped with each frame before drawing anything else:
            GraphicsDevice.Clear(Color.Black);
            StarGenerator.Draw();

            // Cockpit
            Graphics.Current.SpriteB.Begin();
            Rectangle rect = new Rectangle(0, 0, Graphics.Current.ScreenWidth, Graphics.Current.ScreenHeight);
            Graphics.Current.SpriteB.Draw(
                Graphics.Current.SpritesByName["spacecockpit"],
                rect,
                Color.Gray
            );
            Graphics.Current.SpriteB.End();

            base.Draw(gameTime);
        }

        protected void Setup()
        {
            // Any logic that needs to run at the beginning of the game only:
            StarGenerator = new StarGenerator();
            RunSetup = false;
        }

        protected void DrawBackground()
        {
            // Draw your background image here if you want one:

            //Rectangle destinationRectangle = new Rectangle(0, 0, Graphics.Current.ScreenWidth, Graphics.Current.ScreenHeight);
            //Graphics.Current.SpriteB.Begin();
            //Graphics.Current.SpriteB.Draw(
            //    Graphics.Current.SpritesByName["sprite_name"],  // Sprite: (texture2d)
            //    destinationRectangle,
            //    Color.White
            //);
            //Graphics.Current.SpriteB.End();
        }
        protected void CheckInput()
        {
            loadFrames++;
            _previousKeyboardState = _currentKeyboardState;
            _currentKeyboardState = Keyboard.GetState();
            _previousMouseState = _currentMouseState;
            _currentMouseState = Mouse.GetState();

            if (loadFrames >= LOAD_FRAMES_THRESH)
            {
                // Check if any key was pressed
                if (_currentKeyboardState.GetPressedKeys().Length > 0 && _previousKeyboardState.GetPressedKeys().Length == 0)
                {
                    Exit();
                }

                // Check if the mouse has moved
                Vector2 currentPos = new Vector2(_currentMouseState.Position.X, _currentMouseState.Position.Y);
                Vector2 previousPos = new Vector2(_previousMouseState.Position.X, _previousMouseState.Position.Y);
                if (Global.ApproxDist(currentPos, previousPos) >= 1)
                {
                    Exit();
                }
            }
        }
    }
}