using System;
using System.IO;
using Microsoft.Xna.Framework;

namespace Zeus.Client.Tests.MonoGameClientTest.Formats {

	public static class BinaryReaderExtensions {

		public static Vector2 ReadVector2( this BinaryReader bin ) {
			return new Vector2( bin.ReadSingle(), bin.ReadSingle() );
		}
		public static Vector3 ReadVector3( this BinaryReader bin ) {
			return new Vector3( bin.ReadSingle(), bin.ReadSingle(), bin.ReadSingle() );
		}
		public static Vector4 ReadVector4( this BinaryReader bin ) {
			return new Vector4( bin.ReadSingle(), bin.ReadSingle(), bin.ReadSingle(), bin.ReadSingle() );
		}

		public static void Write( this BinaryWriter bin, Vector2 vec ) {
			bin.Write( vec.X );
			bin.Write( vec.Y );
		}
		public static void Write( this BinaryWriter bin, Vector3 vec ) {
			bin.Write( vec.X );
			bin.Write( vec.Y );
			bin.Write( vec.Z );
		}
		public static void Write( this BinaryWriter bin, Vector4 vec ) {
			bin.Write( vec.X );
			bin.Write( vec.Y );
			bin.Write( vec.Z );
			bin.Write( vec.W );
		}

	}

	public static class RSMMeshExtensions {
		/// <summary>
		/// is the Parent from Mesh == MainMesh ?
		/// </summary>
		/// <param name="MainMesh"></param>
		/// <param name="Mesh"></param>
		/// <returns></returns>
		public static bool IsParent( this RSMMesh MainMesh, RSMMesh Mesh ) {
			return ( MainMesh.Head.Name == Mesh.Head.ParentName );
		}
	}
		
	public static class Vector3Extensions {

		public static Vector3 MaxFrom( this Vector3 v, Vector3 v2 ) {
			v.X = Math.Max( v.X, v2.X );
			v.Y = Math.Max( v.Y, v2.Y );
			v.Z = Math.Max( v.Z, v2.Z );

			return v;
		}

		public static Vector3 MinFrom( this Vector3 v, Vector3 v2 ) {
			v.X = Math.Min( v.X, v2.X );
			v.Y = Math.Min( v.Y, v2.Y );
			v.Z = Math.Min( v.Z, v2.Z );

			return v;
		}

		public static Vector3 MultipleMatrix( this Vector3 Vin, Vector4[] Matrix ) {
			Vector3 Vout = Vector3.Zero;

			Vout.X = ( Vin.X * Matrix[ 0 ].X ) + ( Vin.Y * Matrix[ 0 ].Y ) + ( Vin.Z * Matrix[ 0 ].Z ) + ( 1f * Matrix[ 0 ].W );
			Vout.Y = ( Vin.X * Matrix[ 1 ].X ) + ( Vin.Y * Matrix[ 1 ].Y ) + ( Vin.Z * Matrix[ 1 ].Z ) + ( 1f * Matrix[ 1 ].W );
			Vout.X = ( Vin.X * Matrix[ 2 ].X ) + ( Vin.Y * Matrix[ 2 ].Y ) + ( Vin.Z * Matrix[ 2 ].Z ) + ( 1f * Matrix[ 2 ].W );

			return Vout;
		}

	}

}
