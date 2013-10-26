using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework;
using Zeus.Client.Library.Format.Ragnarok.Grf;

namespace Zeus.Client.Tests.MonoGameClientTest.Formats {

	public class RSWFile  {
		public class DataWater {
			/// <summary>
			/// Versions 1.3 and above
			/// </summary>
			public float Height = 0;
			/// <summary>
			/// Versions 1.8 and above
			/// </summary>
			public uint Type = 0;
			/// <summary>
			/// Versions 1.8 and above
			/// </summary>
			public float Amplitude = 1;
			/// <summary>
			/// Versions 1.8 and above
			/// </summary>
			public float Phase = 2;
			/// <summary>
			/// Versions 1.8 and above
			/// </summary>
			public float SurfaceCurveLevel = 0.5f;
			/// <summary>
			/// Versions 1.9 and above
			/// </summary>
			public int TextureCycling = 3;
		};

		public class DataModel {
			public string name; // 40 [prefix with: "data\model\"]
			public int unk1;// (version >= 1.3)
			public float unk2;// (version >= 1.3)
			public float unk3;// (version >= 1.3)
			public string filename; // 40
			public string reserved; // 40
			public string type; // 20
			public string sound; // 20
			public string todo1; // 40
			public Vector3 pos;
			public Vector3 rot;
			public Vector3 scale;

			public DataModel( BinaryReader bin, FileVersion Version ) {
				name = Tools.ReadWord( bin, 40 );
				if( Version.IsCompatible( 1, 3 ) ) {
					unk1 = bin.ReadInt32();
					unk2 = bin.ReadSingle();
					unk3 = bin.ReadSingle();
				}
				filename = Tools.ReadWord( bin, 40 );
				reserved = Tools.ReadWord( bin, 40 );
				type = Tools.ReadWord( bin, 20 );
				sound = Tools.ReadWord( bin, 20 );
				todo1 = Tools.ReadWord( bin, 40 );
				pos = Tools.ReadVector3( bin );
				rot = Tools.ReadVector3( bin );
				scale = Tools.ReadVector3( bin );
			}
		};


		public class DataLight {
			public string name; // 40
			public Vector3 pos;
			public char[] unk1; // 40
			public Vector3 color;
			public float unk2;

			public DataLight( BinaryReader bin, FileVersion Version ) {
				name = Tools.ReadWord( bin, 40 );
				pos = Tools.ReadVector3( bin );
				unk1 = bin.ReadChars( 40 );
				color = Tools.ReadVector3( bin );
				unk2 = bin.ReadSingle();
			}
		};

		public class DataSound {
			public string name; // 80
			public string filename; // 80
			public float[] unk; // 8

			public DataSound( BinaryReader bin, FileVersion Version ) {
				name = Tools.ReadWord( bin, 80 );
				filename = Tools.ReadWord( bin, 80 );
				unk = new float[ 8 ]{
					bin.ReadSingle(),
					bin.ReadSingle(),
					bin.ReadSingle(),
					bin.ReadSingle(),
					bin.ReadSingle(),
					bin.ReadSingle(),
					bin.ReadSingle(),
					bin.ReadSingle()
				};
			}
		};

		public class DataEffect {
			public string name; // 40
			public float unk1; // 9
			public int category;
			public Vector3 pos;
			public int type;
			public float loop;
			public float[] unk2; // 2
			public int[] unk3; // 2

			public DataEffect( BinaryReader bin, FileVersion Version ) {
				name = Tools.ReadWord( bin, 40 );
				unk1 = bin.ReadSingle();
				category = bin.ReadInt32();
				pos = Tools.ReadVector3( bin );
				type = bin.ReadInt32();
				loop = bin.ReadSingle();
				unk2 = new float[ 2 ] {
					bin.ReadSingle(),
					bin.ReadSingle()
				};
				unk3 = new int[ 2 ] {
					bin.ReadInt32(),
					bin.ReadInt32()
				};
			}
		};


		public FileVersion Version;

		private string mIniFile;
		private string mGndFile;
		private string mGatFile;

		private DataWater mWaterData;
		private List<DataModel> mModelData = new List<DataModel>();
		private List<DataLight> mLightData = new List<DataLight>();
		private List<DataSound> mSoundData = new List<DataSound>();
		private List<DataEffect> mEffectData = new List<DataEffect>();


		public DataWater WaterData {
			get { return mWaterData; }
		}
		public List<DataModel> ModelData {
			get { return mModelData; }
		}
		public List<DataLight> LightData {
			get { return mLightData; }
		}
		public List<DataSound> SoundData {
			get { return mSoundData; }
		}
		public List<DataEffect> EffectData {
			get { return mEffectData; }
		}



        public RSWFile(string Filename) {
            Read(Filename);
        }

        public RSWFile(string Filename, RoGrfFile grf) {
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
                // skip Magic Header
                bin.BaseStream.Position += 4; // GRSW

                Version = new FileVersion(bin.ReadByte(), bin.ReadByte());

                mIniFile = Tools.ReadWord(bin, 40);
                mGndFile = Tools.ReadWord(bin, 40);
                if (Version.IsCompatible(1, 4))
                    mGatFile = Tools.ReadWord(bin, 40);

                mWaterData = new DataWater();
                if (Version.IsCompatible(1, 3)) {
                    mWaterData.Height = bin.ReadSingle();
                    if (Version.IsCompatible(1, 8)) {
                        mWaterData.Type = bin.ReadUInt32();
                        mWaterData.Amplitude = bin.ReadSingle();
                        mWaterData.Phase = bin.ReadSingle();
                        mWaterData.SurfaceCurveLevel = bin.ReadSingle();
                        if (Version.IsCompatible(1, 9))
                            mWaterData.TextureCycling = bin.ReadInt32();
                    }
                }

                // Unknown Byte Skipping
                if (Version.IsCompatible(1, 4)) {
                    int i1 = bin.ReadInt32(); // angle(?) in degrees
                    int i2 = bin.ReadInt32(); // angle(?) in degrees
                    Vector3 v1 = Tools.ReadVector3(bin); // some sort of Vector3D
                    Vector3 v2 = Tools.ReadVector3(bin); // some sort of Vector3D
                }
                if (Version.IsCompatible(1, 7)) {
                    float f1 = bin.ReadSingle();
                }
                if (Version.IsCompatible(1, 6)) {
                    int i1 = bin.ReadInt32();
                    int i2 = bin.ReadInt32();
                    int i3 = bin.ReadInt32();
                    int i4 = bin.ReadInt32();
                }

                // reading Objects
                uint objCount = bin.ReadUInt32();
                DataModel m;
                DataLight l;
                DataSound s;
                DataEffect e;
                //System.Diagnostics.Debug.WriteLine( "reading " + objCount + " Objects from RWS File" );
                for (int i = 0; i < objCount; i++) {
                    if (bin.BaseStream.Position + 4 >= bin.BaseStream.Length)
                        break;

                    int objType = bin.ReadInt32();
                    if (objType < 1 || objType > 4) {
                        //System.Diagnostics.Debug.WriteLine( "- UNKNOWN Object (" + objType + ") @ " + i );
                        continue;
                    }

                    // 1 = Model
                    // 2 = Light
                    // 3 = Sound
                    // 4 = Effect
                    if (objType == 1) {
                        try {
                            //System.Diagnostics.Debug.WriteLine( "- found Model @ Obj " + i );
                            m = new DataModel(bin, Version);
                            mModelData.Add(m);
                        } catch {
                            continue;
                        }
                    } else if (objType == 2) {
                        try {
                            //System.Diagnostics.Debug.WriteLine( "- found Light @ Obj " + i );
                            l = new DataLight(bin, Version);
                            mLightData.Add(l);
                        } catch {
                            continue;
                        }
                    } else if (objType == 3) {
                        try {
                            //System.Diagnostics.Debug.WriteLine( "- found Sound @ Obj " + i );
                            s = new DataSound(bin, Version);
                            mSoundData.Add(s);
                        } catch {
                            continue;
                        }
                    } else if (objType == 4) {
                        try {
                            //System.Diagnostics.Debug.WriteLine( "- found Effect @ Obj " + i );
                            e = new DataEffect(bin, Version);
                            mEffectData.Add(e);
                        } catch {
                            continue;
                        }
                    }

                }


            }
        }


	}

}
