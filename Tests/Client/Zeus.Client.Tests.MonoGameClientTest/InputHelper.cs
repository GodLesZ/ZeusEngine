using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Zeus.Client.Tests.MonoGameClientTest {

	public class InputHelper : GameComponent {
		private KeyboardState mNewState;
		private KeyboardState mOldState;


		public InputHelper( Game game ) : base( game ) {
		}



		public override void Initialize() {
			base.Initialize();

			mNewState = mOldState = Keyboard.GetState();
		}

		public override void Update( GameTime gameTime ) {
			mOldState = mNewState;
			mNewState = Keyboard.GetState();

			base.Update( gameTime );
		}




		public bool WasPressed( Keys Key ) {
			return ( mOldState.IsKeyDown( Key ) && mNewState.IsKeyUp( Key ) );
		}

		public bool IsReleased( Keys Key ) {
			return WasPressed( Key );
		}

		public bool IsPressed( Keys Key ) {
			return mNewState.IsKeyDown( Key );
		}


	}

}
