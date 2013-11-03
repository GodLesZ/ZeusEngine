using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XnaContentSlaveGame {

	public class MouseCamera : BaseCamera {
		public static Vector3 VectorFirstPerson = new Vector3(0, 0, 1);
		public static Vector3 VectorThirdPerson = new Vector3(0, 200, -200);
        
		public Matrix MatrixView {
			get;
			protected set;
		}

		public Matrix MatrixProjection {
			get;
			protected set;
		}

		public Matrix MatrixRotation {
			get;
			protected set;
		}

		public Matrix MatrixReflectionView {
			get;
			protected set;
		}

		public Matrix MatrixReflectionProjection {
			get;
			protected set;
		}

	    public float Zoom {
	        get;
	        set;
	    }

		public Vector3 PositionAsVector3 {
			get { return new Vector3(Position.X, Position.Y, Zoom); }
		}

		public Vector3 Rotation {
			get;
			set;
		}

		public Vector3 TargetPosition {
			get;
			set;
		}

		public Vector3 UpVector {
			get;
			set;
		}

		public Vector3 Direction {
			get;
			set;
		}


		public float AspectRatio {
			get;
			set;
		}

		public float FoV {
			get;
			set;
		}

		public float NearPlane {
			get;
			set;
		}

		public float FarPlane {
			get;
			set;
		}


		public MouseCamera(GraphicsDevice device)
			: base(device) {
			Rotation = Vector3.Zero;
			TargetPosition = Vector3.Zero;
			UpVector = Vector3.Zero;
			Direction = Vector3.Forward;

			AspectRatio = device.Viewport.Width / (float)device.Viewport.Height;
			FoV = (float)Math.PI / 4.0f;
			NearPlane = .1f;
			FarPlane = 100000f;
		}


		public virtual void Update() {
			MatrixRotation = Matrix.CreateRotationX(Rotation.X) * Matrix.CreateRotationY(Rotation.Y);
			TargetPosition = PositionAsVector3 + Vector3.Transform(VectorFirstPerson, MatrixRotation);
			UpVector = Vector3.Transform(new Vector3(0, 1, 0), MatrixRotation);

			Direction = Vector3.Normalize(TargetPosition - PositionAsVector3);

			MatrixView = Matrix.CreateLookAt(PositionAsVector3, TargetPosition, UpVector);
			MatrixProjection = Matrix.CreatePerspectiveFieldOfView(FoV, AspectRatio, NearPlane, FarPlane);
		}

	}

}
