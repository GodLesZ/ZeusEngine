using System.IO;
using System.Text;
using Zeus.Client.Library.Format.Ragnarok.Grf;

namespace Zeus.Client.Tests.MonoGameClientTest.Formats {

	public class GATFile {
		public enum ECellType {
			Walkable = 0,
			NoWalkable = 1,
			NoWalkableNoSnipeableWater = 2, // not snipeable
			WalkableWater = 3,
			NoWalkableSnipeableWater = 4, // snipeable
			Snipeable = 5, // snipeable
			NoSnipeable = 6,
			Unknown = 10
		};

		public class GATCell {
			public float LeftBottom;
			public float RightBottom;
			public float LeftTop;
			public float RightTop;
			public ECellType Type;

			public GATCell( float f1, float f2, float f3, float f4, byte t ) {
				LeftBottom = f1;
				RightBottom = f2;
				LeftTop = f3;
				RightTop = f4;
				Type = ConvertTools.Parse<ECellType>( t );
			}
		};

		private int mWidth;
		private int mHeight;
		private GATCell[] mCells;

		public int Width {
			get { return mWidth; }
		}
		public int Height {
			get { return mHeight; }
		}
		public GATCell[] Cells {
			get { return mCells; }
		}

		public char[] MagicHead;



        public GATFile(string Filename) {
            Read(Filename);
        }

        public GATFile(string Filename, RoGrfFile grf) {
            var grfItem = grf.GetFileByName(Filename);
            var data = grf.GetFileData(grfItem, true);
            using (var stream = new MemoryStream(data)) {
                Read(stream);
            }
        }



		public void Read( string Filename ) {
			using( FileStream fs = System.IO.File.OpenRead( Filename ) ) {
			    Read(fs);
			}

		}

        public void Read(Stream fs) {
            using (BinaryReader bin = new BinaryReader(fs, Encoding.GetEncoding("ISO-8859-1"))) {
                MagicHead = bin.ReadChars(6); // GRAT..
                mWidth = bin.ReadInt32();
                mHeight = bin.ReadInt32();
                mCells = new GATCell[mWidth * mHeight];

                for (int i = 0; i < mCells.Length; i++)
                    mCells[i] = new GATCell(bin.ReadSingle(), bin.ReadSingle(), bin.ReadSingle(), bin.ReadSingle(), bin.ReadByte());

            }
        }

	}

}
