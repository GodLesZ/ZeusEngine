#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
#endregion

namespace Zeus.Client.Tests.MonoGameTest {
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game {
        protected GraphicsDeviceManager _graphics;
        protected SpriteBatch _spriteBatch;

        protected Model _pronteraModel;
        protected float _aspectRatio;
        protected Vector3 _modelPosition = Vector3.Zero;
        protected float _modelRotation = 0.0f;
        protected Vector3 _cameraPosition = new Vector3(0.0f, 50.0f, 5000.0f);

        protected bool _showWireframe = true;

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

            _pronteraModel = Content.Load<Model>("prontera");
            _aspectRatio = _graphics.GraphicsDevice.Viewport.Width / (float)_graphics.GraphicsDevice.Viewport.Height;
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
                    _cameraPosition.X += mouseMovement.X * (keyboardState.IsKeyDown(Keys.LeftControl) ? 3 : 1);
                }
                if (mouseMovement.Y != 0) {
                    _cameraPosition.Y += mouseMovement.Y;
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

            // Copy any parent transforms
            var transforms = new Matrix[_pronteraModel.Bones.Count];
            _pronteraModel.CopyAbsoluteBoneTransformsTo(transforms);

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;

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

            foreach (var mesh in _pronteraModel.Meshes) {
                foreach (BasicEffect effect in mesh.Effects) {
                    //effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index] * Matrix.CreateRotationY(_modelRotation) * Matrix.CreateTranslation(_modelPosition);
                    effect.View = Matrix.CreateLookAt(_cameraPosition, Vector3.Zero, Vector3.Up);
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), _aspectRatio, 1.0f, 10000.0f);
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }

            base.Draw(gameTime);
        }

    }

}
