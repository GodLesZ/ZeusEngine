using System;
using Microsoft.Xna.Framework;

namespace Zeus.Client.Tests.MonoGameClientTest {

	public class Camera {
		public Matrix view;
		public Matrix projection;
		public Matrix rotationMatrix;
		public Matrix reflectionView;
		public Matrix reflectionProjection;

		public Vector3 position = Vector3.Zero;
		public Vector3 rotation = Vector3.Zero;
		public Vector3 targetPos = Vector3.Zero;
		public Vector3 upVector = Vector3.Zero;
		public Vector3 direction = Vector3.Forward;

		private Vector3 VectorFirstPerson = new Vector3( 0, 0, 1 );
		private Vector3 VectorThirdPerson = new Vector3( 0, 200, -200 );

		protected GraphicsDeviceManager mGraphic;

		public float aspectRatio = 0f;
		public float fov = (float)Math.PI / 4.0f;
		public float NearPlane = .1f;
		public float FarPlane = 100000f;

		public State state = State.FirstPerson;
		public State lastState = State.FirstPerson;
		public enum State {
			FirstPerson = 0,
			ThirdPerson,
			Fixed
		}

		public float speed = 0f;

		public Camera( GraphicsDeviceManager Graphic ) {
			mGraphic = Graphic;
			aspectRatio = (float)mGraphic.GraphicsDevice.Viewport.Width / (float)mGraphic.GraphicsDevice.Viewport.Height;
		}

		public virtual void Update( GameTime gameTime ) {
			rotationMatrix = Matrix.CreateRotationX( rotation.X ) * Matrix.CreateRotationY( rotation.Y );
			targetPos = position + Vector3.Transform( VectorFirstPerson, rotationMatrix );
			upVector = Vector3.Transform( new Vector3( 0, 1, 0 ), rotationMatrix );

			direction = Vector3.Normalize( targetPos - position );

			view = Matrix.CreateLookAt( position, targetPos, upVector );
			projection = Matrix.CreatePerspectiveFieldOfView( fov, aspectRatio, NearPlane, FarPlane );

		}

		public void SetPosition( Vector3 newPosition ) {
			position = newPosition;
		}


	}
}
