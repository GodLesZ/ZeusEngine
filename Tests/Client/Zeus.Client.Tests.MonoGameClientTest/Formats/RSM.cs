using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Zeus.Client.Library.Format.Ragnarok.Grf;

namespace Zeus.Client.Tests.MonoGameClientTest.Formats {

    public class RSMFile {

        public char[] MagicHead;	// 0: 4x Char
        public FileVersion Version;

        public byte Alpha;
        public int AnimationLen;
        public int ShadowType;

        /// <summary>
        /// Texture Names ( 40 * TextureNameCount )
        /// </summary>
        public int TextureNameCount;	// 4 bytes
        public string[] TextureNames;	// ( 40 * TextureNameCount ) bytes

        public string MainNode;

        /// <summary>
        /// Meshes ( 94 bytes )
        /// </summary>
        public List<RSMMesh> Meshes = new List<RSMMesh>();

        private GraphicsDevice mDevice;
        private VertexBuffer mVertexBuffer;
        private VertexDeclaration mVertexDeclaration;
        private List<VertexPositionColorTexture> mVerticesList = new List<VertexPositionColorTexture>();


        public RSMFile(GraphicsDevice Device, string Filename, bool Init) {
            mDevice = Device;
            Read(Filename);
        }

        public RSMFile(GraphicsDevice Device, string Filename, RoGrfFile grf, bool Init) {
            mDevice = Device;
            var grfItem = grf.SearchByLinq(Filename);
            var data = grf.GetFileData(grfItem, true);
            using (var stream = new MemoryStream(data)) {
                Read(stream);
            }
        }



        public void DrawAll(GraphicsDevice Device, BasicEffect Effect, Matrix view, Matrix projection, Matrix world) {
            if (mVerticesList.Count == 0) {
                // catch VerticeList from all Meshes
                for (int i = 0; i < Meshes.Count; i++)
                    mVerticesList.AddRange(Meshes[i].VerticesList);

                mVertexDeclaration = new VertexDeclaration(VertexPositionColorTexture.VertexDeclaration.GetVertexElements());
                mVertexBuffer = new VertexBuffer(Device, mVertexDeclaration, mVerticesList.Count, BufferUsage.WriteOnly);
                mVertexBuffer.SetData<VertexPositionColorTexture>(mVerticesList.ToArray());
            }

            Effect.Projection = projection;
            Effect.World = world;
            Effect.View = view;
            Effect.Techniques[0].Passes[0].Apply();

            Device.SetVertexBuffer(mVertexBuffer);
            Device.DrawPrimitives(PrimitiveType.TriangleList, 0, mVerticesList.Count);

        }

        public void DrawSingle(GraphicsDevice Device, BasicEffect Effect, Matrix view, Matrix projection, Matrix world) {
            for (int i = 0; i < Meshes.Count; i++) {
                Meshes[i].Draw(Device, Effect, view, projection, world);
            }
        }



        public void Read(string Filename) {

            using (FileStream s = System.IO.File.OpenRead(Filename)) {
                Read(s);
            }

        }

        public void Read(Stream s) {
            using (BinaryReader bin = new BinaryReader(s, Encoding.GetEncoding("ISO-8859-1"))) {
                MagicHead = bin.ReadChars(4);
                Version = new FileVersion(bin.ReadByte(), bin.ReadByte());

                AnimationLen = bin.ReadInt32();
                ShadowType = bin.ReadInt32();
                if (Version.IsCompatible(1, 4))
                    Alpha = bin.ReadByte();

                bin.BaseStream.Position += 16;

                TextureNameCount = bin.ReadInt32();
                TextureNames = new string[TextureNameCount];
                for (int i = 0; i < TextureNameCount; i++)
                    TextureNames[i] = Tools.ReadWord(bin, 40);

                MainNode = Tools.ReadWord(bin, 40);
                int nodeCount = bin.ReadInt32();
                RSMMesh m;
                for (int i = 0; i < nodeCount; i++) {
                    if (RSMMesh.ReadMesh(bin, this , out m) == true) {
                        m.SetUpVertices(mDevice);
                        Meshes.Add(m);
                    }
                }

                if (Meshes.Count == 1) {
                    Meshes[0].IsOnly = true;
                }

            }
        }



    }

}
