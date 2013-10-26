using System;
using System.Drawing;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Zeus.Client.Tests.MonoGameClientTest.Formats {

	public class Tools {

		public static string ReadWord( BinaryReader bin, int max ) {
			StringBuilder sb = new StringBuilder();

			while( sb.Length < max && bin.BaseStream.CanRead == true && bin.PeekChar() != '\0' )
				sb.Append( bin.ReadChar() );

			bin.BaseStream.Position += ( max - sb.Length );
			return sb.ToString();
		}

		#region base ReadVector
		public static Vector2 ReadVector2( BinaryReader bin ) {
			if( ( bin.BaseStream.Length - bin.BaseStream.Position ) < 6 )
				return Vector2.Zero;

			Vector2 vec = new Vector2(
				bin.ReadSingle(),
				bin.ReadSingle()
			);

			return vec;
		}

		public static Vector3 ReadVector3( BinaryReader bin ) {
			if( ( bin.BaseStream.Length - bin.BaseStream.Position ) < 9 )
				return Vector3.Zero;

			Vector3 vec = new Vector3(
				bin.ReadSingle(),
				bin.ReadSingle(),
				bin.ReadSingle()
			);

			return vec;
		}

		public static Vector4 ReadVector4( BinaryReader bin ) {
			if( ( bin.BaseStream.Length - bin.BaseStream.Position ) < 12 )
				return Vector4.Zero;

			Vector4 vec = new Vector4(
				bin.ReadSingle(),
				bin.ReadSingle(),
				bin.ReadSingle(),
				bin.ReadSingle()
			);

			return vec;
		} 
		#endregion

		#region safe ReadVector
		public static bool ReadVector2( BinaryReader bin, out Vector2 vec ) {
			vec = Vector2.Zero;
			if( ( bin.BaseStream.Length - bin.BaseStream.Position ) < 6 )
				return false;

			vec = new Vector2(
				bin.ReadSingle(),
				bin.ReadSingle()
			);

			return true;
		}

		public static bool ReadVector3( BinaryReader bin, out Vector3 vec ) {
			vec = Vector3.Zero;
			if( ( bin.BaseStream.Length - bin.BaseStream.Position ) < 9 )
				return false;

			vec = new Vector3(
				bin.ReadSingle(),
				bin.ReadSingle(),
				bin.ReadSingle()
			);

			return true;
		}

		public static bool ReadVector4( BinaryReader bin, out Vector4 vec ) {
			vec = Vector4.Zero;
			if( ( bin.BaseStream.Length - bin.BaseStream.Position ) < 12 )
				return false;

			vec = new Vector4(
				bin.ReadSingle(),
				bin.ReadSingle(),
				bin.ReadSingle(),
				bin.ReadSingle()
			);

			return true;
		} 
		#endregion

	}

	public class ConvertTools {

		public static T Parse<T>( string input ) {
			try {
				return (T)Enum.Parse( typeof( T ), input );
			} catch {
				return default( T );
			}
		}
		public static T Parse<T>( int input ) {
			try {
				return (T)Enum.Parse( typeof( T ), input.ToString() );
			} catch {
				return default( T );
			}
		}

		public static Texture2D Image2Texture( System.Drawing.Bitmap Image, GraphicsDevice g ) {
			Texture2D Texture = null;
			try {
				using( System.IO.MemoryStream ms = new System.IO.MemoryStream() ) {
					Image.Save( ms, System.Drawing.Imaging.ImageFormat.Bmp );
					ms.Seek( 0, 0 );
					Texture = Texture2D.FromStream( g, ms ) as Texture2D;
				}
			} catch { }

			return Texture;
		}

        public static Texture2D BitmapToTexture(System.Drawing.Bitmap Image, GraphicsDevice g) {
            if (Image.PixelFormat == System.Drawing.Imaging.PixelFormat.Format32bppArgb) {
                return Image2Texture(Image, g);
            }

            // Convert .bmp's
            var bitmap = new Bitmap(Image.Width, Image.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (var graphics = System.Drawing.Graphics.FromImage(bitmap)) {
                graphics.DrawImage(Image, 0, 0, Image.Width, Image.Height);
            }
            return Image2Texture(bitmap, g);
        }
	}

}
