using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Zeus.Client.Tests.MonoGameClientTest {

	public class MouseCam : Camera {
		private KeyboardState mNewKeyboardState;
		private MouseState mNewMouseState, mOldMouseState;

		private Point oldMousePosition = Point.Zero;
		public Vector3 lastPosition;

		public MouseCam( GraphicsDeviceManager Manager ) : base( Manager ) {
			mNewMouseState = Mouse.GetState();
			mOldMouseState = mNewMouseState;
		}

		private bool IsPressed( Keys Key ) {
			return mNewKeyboardState.IsKeyDown( Key );
		}

		public override void Update( GameTime gameTime ) {
			float speed = 1f;
			if( IsPressed( Keys.LeftShift ) == true || IsPressed( Keys.RightShift ) == true || IsPressed( Keys.CapsLock ) == true )
				speed *= 5;
			else if( IsPressed( Keys.LeftControl ) == true || IsPressed( Keys.RightControl ) == true )
				speed /= 5;
			PollInput( speed );

			base.Update( gameTime );
		}

		private void PollInput( float amountOfMovement ) {
			mNewKeyboardState = Keyboard.GetState();
			mNewMouseState = Mouse.GetState();
			Vector3 moveVector = new Vector3();

			if( mNewKeyboardState.IsKeyDown( Keys.Right ) || mNewKeyboardState.IsKeyDown( Keys.D ) )
				moveVector.X -= amountOfMovement;
			if( mNewKeyboardState.IsKeyDown( Keys.Left ) || mNewKeyboardState.IsKeyDown( Keys.A ) )
				moveVector.X += amountOfMovement;
			if( mNewKeyboardState.IsKeyDown( Keys.Down ) || mNewKeyboardState.IsKeyDown( Keys.S ) )
				moveVector.Z -= amountOfMovement;
			if( mNewKeyboardState.IsKeyDown( Keys.Up ) || mNewKeyboardState.IsKeyDown( Keys.W ) )
				moveVector.Z += amountOfMovement;

			if( mNewMouseState.ScrollWheelValue - mOldMouseState.ScrollWheelValue > 0 )
				moveVector.Z += amountOfMovement;
			if( mNewMouseState.ScrollWheelValue - mOldMouseState.ScrollWheelValue < 0 )
				moveVector.Z -= amountOfMovement;

			lastPosition = position;
			rotationMatrix = Matrix.CreateRotationX( rotation.X ) * Matrix.CreateRotationY( rotation.Y );
			position += Vector3.Transform( moveVector, rotationMatrix );

			speed = Vector3.Distance( position, lastPosition );


			if( state != State.Fixed ) {

				if( mNewMouseState.X != mOldMouseState.X )
					rotation.Y -= amountOfMovement * 2f / mGraphic.PreferredBackBufferWidth * ( mNewMouseState.X - mOldMouseState.X );
				if( mNewMouseState.Y != mOldMouseState.Y )
					rotation.X += amountOfMovement * 2f / mGraphic.PreferredBackBufferWidth * ( mNewMouseState.Y - mOldMouseState.Y );

				Mouse.SetPosition( mOldMouseState.X, mOldMouseState.Y );
			}



			if( mNewMouseState.RightButton == ButtonState.Pressed ) {
				state = State.FirstPerson;
				if( oldMousePosition == Point.Zero )
					oldMousePosition = new Point( mNewMouseState.X, mNewMouseState.Y );
			} else {
				state = State.Fixed;
				if( oldMousePosition != Point.Zero ) {
					Mouse.SetPosition( oldMousePosition.X, oldMousePosition.Y );
					mNewMouseState = mOldMouseState = Mouse.GetState();
				}
				oldMousePosition = Point.Zero;
			}


			mOldMouseState = Mouse.GetState();

		}
	}
}
