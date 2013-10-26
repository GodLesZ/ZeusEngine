using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using Zeus.Server.Library.Configuration;
using Zeus.Server.Library.Configuration.Yaml;

namespace Zeus.Client.Tests.MonoGameClientTest {
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private Camera camera;
        private MaptestRoGrf romap;
        private InputHelper mInputHelper;


        public Game1() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            IsMouseVisible = true;
            IsFixedTimeStep = false;

            mInputHelper = new InputHelper(this);
            Components.Add(mInputHelper);
            Components.Add(new FpsCounter(this));

            romap = new MaptestRoGrf();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize() {
            var settings = ConfigurationFactory.Create<YamlConfiguration>("config.yaml").FirstAsExpando();

            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();

            camera = new MouseCam(graphics);
            camera.SetPosition(new Vector3(-40.19f, 8.7f, -167.8f));
            camera.rotation = new Vector3(0f, 0f, 100f);

            romap.FromGrf(this, (string)settings.test_mapname, (string)settings.test_grfpath);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            romap.LoadContent();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent() {
            Content.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime) {
            if (mInputHelper.WasPressed(Keys.Escape))
                this.Exit();

            if (camera != null) {
                camera.Update(gameTime);

                if (camera.state == Camera.State.Fixed && !IsMouseVisible)
                    IsMouseVisible = true;
                else if (camera.state != Camera.State.Fixed && IsMouseVisible)
                    IsMouseVisible = false;
            }

            romap.Update(mInputHelper);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            romap.Draw(camera.view, camera.projection);

            base.Draw(gameTime);
        }

    }

}
