using Microsoft.Xna.Framework;

namespace Zeus.Client.Tests.MonoGameClientTest {

	public class FpsCounter : DrawableGameComponent {
		int fps = 0;
		float total = 0;


		public FpsCounter( Game game )
			: base( game ) {
		}


		public override void Draw( GameTime gameTime ) {
			float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
			total += elapsed;

			if( total >= 1 ) {
				this.Game.Window.Title = "FPS: " + fps;
				fps = 0;
				total = 0;
			}

			fps += 1;
		}

	}

}
