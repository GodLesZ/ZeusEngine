using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using OpenTK.Graphics.OpenGL;
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
            _testRsm.LoadTextures(GraphicsDevice, "C:/Games/ragnarok-full-data/data/");

            //_testRsw = new RswFormat("Content/prontera.rsw");
            //_testRsw.Load(GraphicsDevice, "C:/Games/RO/data/");

            _modelEffect = new BasicEffect(GraphicsDevice);
            _modelEffect.TextureEnabled = true;


            var _vertexShader = @"
                attribute vec3 aPosition;
                attribute vec3 aVertexNormal;
                attribute vec2 aTextureCoord;
                attribute float aAlpha;
                
                varying vec2 vTextureCoord;
                varying float vLightWeighting;
                varying float vAlpha;
                
                uniform mat4 uModelViewMat;
                uniform mat4 uProjectionMat;
                
                uniform vec3 uLightDirection;
                uniform mat3 uNormalMat;
                
                void main(void) {
                    gl_Position     = uProjectionMat * uModelViewMat * vec4( aPosition, 1.0);
                    
                    vTextureCoord   = aTextureCoord;
                    vAlpha          = aAlpha;
                    
                    vec4 lDirection  = uModelViewMat * vec4( uLightDirection, 0.0);
                    vec3 dirVector   = normalize(lDirection.xyz);
                    float dotProduct = dot( uNormalMat * aVertexNormal, dirVector );
                    vLightWeighting  = max( dotProduct, 0.5 );
                }";


            var _fragmentShader = @"
                varying vec2 vTextureCoord;
                varying float vLightWeighting;
                varying float vAlpha;
                
                uniform sampler2D uDiffuse;
                
                uniform bool  uFogUse;
                uniform float uFogNear;
                uniform float uFogFar;
                uniform vec3  uFogColor;
                
                uniform vec3  uLightAmbient;
                uniform vec3  uLightDiffuse;
                uniform float uLightOpacity;
                
                void main(void) {
                    vec4 texture  = texture2D( uDiffuse,  vTextureCoord.st );
                    
                    if ( texture.a == 0.0 )
                        discard;
                    
                    vec3 Ambient    = uLightAmbient * uLightOpacity;
                    vec3 Diffuse    = uLightDiffuse * vLightWeighting;
                    vec4 LightColor = vec4( Ambient + Diffuse, 1.0);
                    
                    gl_FragColor    = texture * clamp(LightColor, 0.0, 1.0);
                    gl_FragColor.a *= vAlpha;
                    
                    if ( uFogUse ) {
                        float depth     = gl_FragCoord.z / gl_FragCoord.w;
                        float fogFactor = smoothstep( uFogNear, uFogFar, depth );
                        gl_FragColor    = mix( gl_FragColor, vec4( uFogColor, gl_FragColor.w ), fogFactor );
                    }
                }";

        }

        protected int CreateShaderProgram(string vertexShader, string fragmentShader) {

            // Compile shader and attach them
            var shaderProgram = GL.CreateProgram();
            var vs = CompileShader(vertexShader, ShaderType.VertexShader);
            var fs = CompileShader(fragmentShader, ShaderType.FragmentShader);

            GL.AttachShader(shaderProgram, vs);
            GL.AttachShader(shaderProgram, fs);
            GL.LinkProgram(shaderProgram);

            // Is there an error
            var link_status = 0;
            GL.GetProgram(shaderProgram, ProgramParameter.LinkStatus, out link_status);
            if (link_status < 1) {
                var error = GL.GetProgramInfoLog(shaderProgram);
                GL.DeleteProgram(shaderProgram);
                GL.DeleteShader(vs);
                GL.DeleteShader(fs);

                throw new Exception("CreateShaderProgram() - Fail to link shaders : " + error);
            }

            // Get back attributes
            var count = 0;
            GL.GetProgram(shaderProgram, ProgramParameter.ActiveAttributes, out count);
            var shaderAttributes = new Dictionary<ActiveAttribType, string>();
            for (var i = 0; i < count; i++) {
                var size = 0;
                ActiveAttribType type;
                var attrib = GL.GetActiveAttrib(shaderProgram, i, out size, out type);
                shaderAttributes.Add(type, attrib);
            }

            // Get back uniforms
            GL.GetProgram(shaderProgram, ProgramParameter.ActiveUniforms, out count);
            var shaderUniforms = new Dictionary<ActiveUniformType, string>();
            for (var i = 0; i < count; i++) {
                var size = 0;
                ActiveUniformType type;
                var attrib = GL.GetActiveUniform(shaderProgram, i, out size, out type);
                shaderUniforms.Add(type, attrib);
            }

            //gl.uniformMatrix4fv(uniform.uModelViewMat, false, modelView);
            //GL.UniformMatrix4(uniform.uModelViewMat, false, modelView);

            return shaderProgram;
        }

        private int CompileShader(string vertexShader, ShaderType type) {
            // Compile shader
            var shader = GL.CreateShader(type);
            GL.ShaderSource(shader, "precision mediump float;" + vertexShader);
            GL.CompileShader(shader);

            // Is there an error ?
            var compile_status = 0;
            GL.GetShader(shader, ShaderParameter.CompileStatus, out compile_status);
            if (compile_status < 1) {
                var error = GL.GetShaderInfoLog(shader);
                GL.DeleteShader(shader);

                throw new Exception("WebGL::CompileShader() - Fail to compile shader : " + error);
            }

            return shader;
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
