using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XnaContentSlaveGame {

	public class BaseCamera {
		protected Point mPosition = Point.Zero;
		protected readonly GraphicsDevice mGraphicsDevice;
		
		public Point Position {
			get { return mPosition; }
			set { mPosition = value; }
		}

		public int X {
			get { return mPosition.X; }
			set { mPosition.X = value; }
		}

		public int Y {
			get { return mPosition.Y; }
			set { mPosition.Y = value; }
		}

		public Matrix TransformMatrix {
			get { return Matrix.CreateTranslation(new Vector3(-X, -Y, 0f)); }
		}

		public Rectangle Viewport {
			get { return new Rectangle(X, Y, mGraphicsDevice.Viewport.Width, mGraphicsDevice.Viewport.Height); }
		}



		public BaseCamera(GraphicsDevice graphicsDevice) {
			mGraphicsDevice = graphicsDevice;
		}


		public virtual void Refresh() {

		}



		public bool IsInViewport(Point p) {
			return IsInViewport(p.X, p.Y);
		}

		public bool IsInViewport(Rectangle value) {
			var view = Viewport;
			return value.X < view.X + view.Width && view.X < value.X + value.Width && value.Y < view.Y + view.Height && view.Y < value.Y + value.Height;
		}

		public bool IsInViewport(int x, int y) {
			return (x >= X && y >= Y);
		}

	}

}
