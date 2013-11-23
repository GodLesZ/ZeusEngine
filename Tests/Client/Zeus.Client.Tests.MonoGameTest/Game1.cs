using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using RsmFormat = Zeus.Client.Library.Format.Ragnarok.Rsm.Format;
using RswFormat = Zeus.Client.Library.Format.Ragnarok.Rsw.Format;

namespace Zeus.Client.Tests.MonoGameTest {

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game {
        protected GraphicsDeviceManager _graphics;
        protected SpriteBatch _spriteBatch;

        protected RsmFormat _testRsm;
        protected RswFormat _testRsw;

        protected BasicEffect _modelEffect;
        protected float _aspectRatio;
        protected Vector3 _modelPosition = Vector3.Zero;
        protected float _modelRotation = 0;
        protected Vector3 _cameraPosition = new Vector3(0, 0, 50.0f);

        protected bool _showWireframe = false;

        protected KeyboardState _oldKeyboardState;
        protected MouseState _oldMouseState;


        public Game1() {
            _graphics = new GraphicsDeviceManager(this);

            _oldKeyboardState = Keyboard.GetState();
            _oldMouseState = Mouse.GetState();

            Content.RootDirectory = "Content";
        }


        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _aspectRatio = _graphics.GraphicsDevice.Viewport.Width / (float)_graphics.GraphicsDevice.Viewport.Height;

            _testRsm = new RsmFormat("Content/random_test_model.rsm");
            _testRsm.CalculateMeshes(GraphicsDevice);

            //_testRsw = new RswFormat("Content/prontera.rsw");
            //_testRsw.Load(GraphicsDevice, "C:/Games/RO/data/");

            _modelEffect = new BasicEffect(GraphicsDevice);
            _modelEffect.TextureEnabled = true;
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime) {
            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape)) {
                Exit();
                return;
            }

            var mouseMovement = new Point(mouseState.X - _oldMouseState.X, mouseState.Y - _oldMouseState.Y);
            var mouseWheelDelta = (mouseState.ScrollWheelValue - _oldMouseState.ScrollWheelValue);

            if (mouseState.RightButton == ButtonState.Pressed) {
                if (mouseMovement.X != 0) {
                    _cameraPosition.X += mouseMovement.X * (keyboardState.IsKeyDown(Keys.LeftControl) ? 0.1f : 0.001f);
                }
                if (mouseMovement.Y != 0) {
                    _cameraPosition.Y += mouseMovement.Y * (keyboardState.IsKeyDown(Keys.LeftControl) ? 0.1f : 0.001f);
                }
            }

            if (mouseWheelDelta != 0) {
                _cameraPosition.Z += mouseWheelDelta / (keyboardState.IsKeyDown(Keys.LeftControl) ? -1f : -10f);
            }

            if (keyboardState.IsKeyDown(Keys.Left)) {
                _modelPosition.X--;
            }
            if (keyboardState.IsKeyDown(Keys.Right)) {
                _modelPosition.X++;
            }
            if (keyboardState.IsKeyDown(Keys.Down)) {
                _modelPosition.Y--;
            }
            if (keyboardState.IsKeyDown(Keys.Up)) {
                _modelPosition.Y++;
            }
            if (keyboardState.IsKeyDown(Keys.PageUp)) {
                _modelPosition.Z++;
            }
            if (keyboardState.IsKeyDown(Keys.PageDown)) {
                _modelPosition.Z--;
            }

            if (keyboardState.IsKeyUp(Keys.W) && _oldKeyboardState.IsKeyDown(Keys.W)) {
                _showWireframe = !_showWireframe;
            }

            _oldKeyboardState = keyboardState;
            _oldMouseState = mouseState;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            if (_showWireframe) {
                var rasterizerState = new RasterizerState {
                    FillMode = FillMode.WireFrame
                };
                GraphicsDevice.RasterizerState = rasterizerState;
            } else {
                var rasterizerState = new RasterizerState {
                    FillMode = FillMode.Solid
                };
                GraphicsDevice.RasterizerState = rasterizerState;
            }

            /*
            var world = Matrix.CreateRotationX(_modelRotation) * Matrix.CreateRotationY(_modelRotation) * Matrix.CreateTranslation(_modelPosition);
            var view = Matrix.CreateLookAt(_cameraPosition, Vector3.Zero, Vector3.Up);
            var projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), _aspectRatio, 1.0f, 10000.0f);
            */
            var world =
                Matrix.CreateRotationX(_cameraPosition.Y) *
                Matrix.CreateRotationY(_cameraPosition.X) *
                Matrix.CreateTranslation(0.0f, 0f, 5f);
            var view = Matrix.CreateLookAt(new Vector3(0, 0, _cameraPosition.Z), Vector3.Zero, Vector3.Up);
            var projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), _aspectRatio, 1.0f, 10000.0f);

            _testRsm.Draw(GraphicsDevice, _modelEffect, view, projection, world);
            //_testRsw.DrawAll(GraphicsDevice, _modelEffect, view, projection, world);

            base.Draw(gameTime);
        }

    }

}
