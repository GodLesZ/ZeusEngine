using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Graphics;
using Zeus.Client.Library.Format.Ragnarok.Grf;

namespace Zeus.Client.Tests.MonoGameClientTest.Formats {

    public class GNDFile {

        public struct TextureData {
            public string TexturePath;	// size: 40 [cap at \0]
            public char[] Unknown;		// size: 40

            public Bitmap TextureBmp;
            public Texture2D TextureTex;
        };

        public struct LitghmapData {
            public char[] brightness;	// size: 64
            public char[] colorrbg;		// size: 192
        };

        public struct TileData {
            /// <summary>
            /// Start: Unten links
            /// </summary>
            public float u1;
            /// <summary>
            /// Start: Unten rechts
            /// </summary>
            public float u2;
            /// <summary>
            /// Start: Oben links
            /// </summary>
            public float u3;
            /// <summary>
            /// Start: Oben rechts
            /// </summary>
            public float u4;
            /// <summary>
            /// End: Unten links
            /// </summary>
            public float v1;
            /// <summary>
            /// End: Unten rechts
            /// </summary>
            public float v2;
            /// <summary>
            /// End: Oben links
            /// </summary>
            public float v3;
            /// <summary>
            /// End: Oben rechts
            /// </summary>
            public float v4;
            public ushort TextureIndex;
            public ushort Lightmap;
            public char[] color; // BGRA => 'A' = ignored?
        };

        public struct CubeData {
            /// <summary>
            /// Höhe unten links
            /// </summary>
            public float y1;
            /// <summary>
            /// Höhe unten rechts
            /// </summary>
            public float y2;
            /// <summary>
            /// Höhe oben links
            /// </summary>
            public float y3;
            /// <summary>
            /// Höhe oben rechts
            /// </summary>
            public float y4;
            /// <summary>
            /// Textur Index: Oben
            /// </summary>
            public int TileTop;
            /// <summary>
            /// Textur Index: Front
            /// </summary>
            public int TileFront;
            /// <summary>
            /// Textur Index: Links
            /// </summary>
            public int TileLeft;
        };

        public struct GridData {
            public uint X;
            public uint Y;
            public uint Cells;
        };

        public uint Width;
        public uint Height;
        public uint Ratio;
        public uint TextureCount;
        public uint TextureSize;

        public int LightmapCount;
        public int TileCount;
        public uint CubeCount;

        public TextureData[] Textures;
        public GridData Grid;
        public LitghmapData[] Lightmaps;
        public TileData[] Tiles;
        public CubeData[] Cubes;


        public static GraphicsDevice GraphicsDevice;
        public ExportedTextureData ExportedTexture = new ExportedTextureData();
        public Texture2D TexturesAll;

        private string mTextureRoot;

        public GNDFile(string Filename, string TextureRoot) {
            mTextureRoot = TextureRoot;
            Read(Filename);
        }

        public GNDFile(string Filename, RoGrfFile grf) {
            var grfItem = grf.GetFileByName(Filename);
            var data = grf.GetFileData(grfItem, true);
            using (var stream = new MemoryStream(data)) {
                Read(stream, grf);
            }
        }

        public void Read(string Filename) {

            using (FileStream s = System.IO.File.OpenRead(Filename)) {
                Read(s);
            }
        }

        public void Read(Stream s, RoGrfFile grf = null) {
            using (BinaryReader bin = new BinaryReader(s, Encoding.GetEncoding("ISO-8859-1"))) {
                // skip Magic Header
                bin.BaseStream.Position += 6;

                Width = bin.ReadUInt32();
                Height = bin.ReadUInt32();
                Ratio = bin.ReadUInt32();
                TextureCount = bin.ReadUInt32();
                TextureSize = bin.ReadUInt32();

                Textures = new TextureData[TextureCount];
                for (int i = 0; i < TextureCount; i++) {
                    Textures[i] = new TextureData();
                    Textures[i].TexturePath = Tools.ReadWord(bin, 40).ToLower(); //.Replace( ".bmp", ".png" );
                    Textures[i].Unknown = Tools.ReadWord(bin, 40).ToCharArray();

                    if (grf != null) {
                        var grfItem = grf.SearchByLinq("data/texture/" + Textures[i].TexturePath);
                        if (grfItem != null) {
                            var data = grf.GetFileData(grfItem, true);
                            using (var stream = new MemoryStream(data)) {
                                Textures[i].TextureBmp = Bitmap.FromStream(stream) as Bitmap;
                            }
                            if (GraphicsDevice != null) {
                                Textures[i].TextureTex = ConvertTools.BitmapToTexture(Textures[i].TextureBmp, GraphicsDevice);
                            }
                        } else {
                            Debug.WriteLine("[GND] Texture not found in grf \"{0}\": {1}", Path.GetFileName(grf.Filepath), Textures[i].TexturePath);
                        }
                    } else {
                        Textures[i].TextureBmp = Bitmap.FromFile(mTextureRoot + Textures[i].TexturePath) as Bitmap;
                        if (GraphicsDevice != null) {
                            Textures[i].TextureTex = ConvertTools.BitmapToTexture(Textures[i].TextureBmp, GraphicsDevice);
                        }
                    }
                }

                LightmapCount = bin.ReadInt32();

                Grid = new GridData();
                Grid.X = bin.ReadUInt32();
                Grid.Y = bin.ReadUInt32();
                Grid.Cells = bin.ReadUInt32();

                Lightmaps = new LitghmapData[LightmapCount];
                for (int i = 0; i < LightmapCount; i++) {
                    Lightmaps[i] = new LitghmapData();
                    Lightmaps[i].brightness = bin.ReadChars(64);
                    Lightmaps[i].colorrbg = bin.ReadChars(192);
                }


                TileCount = bin.ReadInt32();
                Tiles = new TileData[TileCount];
                for (int i = 0; i < TileCount; i++) {
                    Tiles[i] = new TileData();
                    Tiles[i].u1 = bin.ReadSingle();
                    Tiles[i].u2 = bin.ReadSingle();
                    Tiles[i].u3 = bin.ReadSingle();
                    Tiles[i].u4 = bin.ReadSingle();
                    Tiles[i].v1 = bin.ReadSingle();
                    Tiles[i].v2 = bin.ReadSingle();
                    Tiles[i].v3 = bin.ReadSingle();
                    Tiles[i].v4 = bin.ReadSingle();
                    Tiles[i].TextureIndex = bin.ReadUInt16();
                    Tiles[i].Lightmap = bin.ReadUInt16();
                    Tiles[i].color = bin.ReadChars(4);
                }

                CubeCount = Width * Height;
                Cubes = new CubeData[CubeCount];
                for (int i = 0; i < CubeCount; i++) {
                    Cubes[i] = new CubeData();
                    Cubes[i].y1 = bin.ReadSingle();
                    Cubes[i].y2 = bin.ReadSingle();
                    Cubes[i].y3 = bin.ReadSingle();
                    Cubes[i].y4 = bin.ReadSingle();
                    Cubes[i].TileTop = bin.ReadInt32();
                    Cubes[i].TileFront = bin.ReadInt32();
                    Cubes[i].TileLeft = bin.ReadInt32();
                }

            }
        }


        [Serializable]
        public struct ExportedTextureDataValue {
            [XmlAttribute()]
            public int Width;
            [XmlAttribute()]
            public int Height;

            public ExportedTextureDataValue(int w, int h) {
                Width = w;
                Height = h;
            }
        }

        [Serializable]
        public struct ExportedTextureData {
            [XmlArrayItem("Data")]
            public List<ExportedTextureDataValue> Positions;

            public ExportedTextureDataValue this[int index] {
                get { return Positions[index]; }
            }

            public int Count {
                get { return Positions.Count; }
            }

            public void Add(int w, int h) {
                Add(new ExportedTextureDataValue(w, h));
            }

            public void Add(ExportedTextureDataValue pos) {
                if (Positions == null)
                    Positions = new List<ExportedTextureDataValue>();

                Positions.Add(pos);
            }
        }

        public void ExportTextureMap(string Filename) {
            ExportedTexture = new ExportedTextureData();
            TexturesAll = null;

            Microsoft.Xna.Framework.Vector2 maxSize = Microsoft.Xna.Framework.Vector2.Zero;
            for (int i = 0; i < TextureCount; i++) {
                ExportedTexture.Add(Textures[i].TextureBmp.Width, Textures[i].TextureBmp.Height);
                maxSize.X += Textures[i].TextureBmp.Width;
                maxSize.Y = Math.Max(Textures[i].TextureBmp.Height, maxSize.Y);
            }

            Bitmap newImg = new Bitmap((int)maxSize.X, (int)maxSize.Y);
            int drawnWidth = 0;
            using (Graphics g = Graphics.FromImage(newImg)) {
                for (int i = 0; i < TextureCount; i++) {
                    g.DrawImage(Textures[i].TextureBmp, drawnWidth, 0);
                    drawnWidth += (int)ExportedTexture[i].Width;
                }
                g.Save();
            }

            TexturesAll = ConvertTools.Image2Texture(newImg, GNDFile.GraphicsDevice);

            if (Filename != string.Empty) {
                System.IO.File.Delete(Filename);
                newImg.Save(Filename);

                string FilenameXml = Path.Combine(Path.GetDirectoryName(Filename), Path.GetFileNameWithoutExtension(Filename)) + @".xml";
                System.IO.File.Delete(FilenameXml);

                try {
                    XmlSerializer xml = new XmlSerializer(typeof(ExportedTextureData));
                    using (FileStream s = System.IO.File.OpenWrite(FilenameXml))
                        xml.Serialize(s, ExportedTexture);
                } catch (Exception e) {
                    Debug.WriteLine(e);
                }
            }
        }



    }

}
