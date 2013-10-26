using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Zeus.Client.Tests.MonoGameClientTest.Formats {

    public class RSMMesh {
        public struct Header {
            public string Name;				// 40 bytes
            public int UnknownInt;			// 4 bytes
            public string ParentName;		// 40 bytes
            // skip 10 * 4 (float) bytes
        };

        public struct TransMatrix {
            public Matrix Matrix;   		// 36 bytes (9x float)
            public Vector3 OffsetTranslation;		// 12 bytes (3x float)
            public Vector3 Position;		// 12 bytes (3x float)
            public float RotationAngle;		// 4 bytes
            public Vector3 RotationAxis;	// 12 bytes (3x float)
            public Vector3 Scale;			// 12 bytes (3x float)
        };

        public struct Surface {
            public ushort[] SurfaceVector;	// 3 * 2 bytes
            public ushort[] TextureVector;	// 3 * 2 bytes
            public ushort TextureID;		// 2 bytes
            public ushort Padding;		// 2 bytes
            public uint TwoSide;		// 4 bytes
            public uint SmoothGroup;		// 4 bytes
        };

        public struct RotationFrame {
            public int Frame;				// 4 bytes
            public Vector3 Rotation;		// 12 bytes (3x float)
        };

        public struct PositionFrame {
            public int Frame;				// 4 bytes
            public Vector4 Position;		// 16 bytes (4x float)
        };

        public struct TextureVertex {
            public uint Color;
            public float U, V;
        }

        public bool IsOnly;


        public RSMFile Rsm;
        public Header Head;
        public TransMatrix Matrix;

        public List<int> TextureIndexs = new List<int>();
        public List<Vector3> MainVectors = new List<Vector3>();
        public List<TextureVertex> TextureVectors = new List<TextureVertex>();
        public List<Surface> Surfaces = new List<Surface>();
        public List<RotationFrame> RotationFrames = new List<RotationFrame>();
        public List<PositionFrame> PositionFrames = new List<PositionFrame>();

        private VertexBuffer mVertexBuffer;
        private VertexDeclaration mVertexDeclaration;
        private List<VertexPositionColorTexture> mVerticesList = new List<VertexPositionColorTexture>();

        public List<VertexPositionColorTexture> VerticesList {
            get { return mVerticesList; }
        }


        public RSMMesh(RSMFile rsm) {
            Rsm = rsm;
        }

        public void SetUpVertices(GraphicsDevice Device) {
            if (MainVectors == null)
                return; // invalid Model?

            for (int i = 0; i < MainVectors.Count; i++) {
                Vector3 v = MainVectors[i];
                mVerticesList.Add(new VertexPositionColorTexture(new Vector3(v.X, v.Z, v.Y), Color.White, new Vector2(0, 0)));
                mVerticesList.Add(new VertexPositionColorTexture(new Vector3(v.X, v.Z, v.Y), Color.White, new Vector2(0, 0)));
                mVerticesList.Add(new VertexPositionColorTexture(new Vector3(v.X, v.Z, v.Y), Color.White, new Vector2(0, 0)));
            }

            mVertexDeclaration = new VertexDeclaration(VertexPositionColorTexture.VertexDeclaration.GetVertexElements());
            mVertexBuffer = new VertexBuffer(Device, mVertexDeclaration, mVerticesList.Count, BufferUsage.WriteOnly);
            mVertexBuffer.SetData<VertexPositionColorTexture>(mVerticesList.ToArray());
        }

        public void Draw(GraphicsDevice Device, BasicEffect Effect, Matrix view, Matrix projection, Matrix world) {
            Effect.Projection = projection;
            Effect.World = world;
            Effect.View = view;
            Effect.CurrentTechnique.Passes[0].Apply();

            Device.SetVertexBuffer(mVertexBuffer);
            Device.DrawPrimitives(PrimitiveType.TriangleList, 0, mVerticesList.Count);
        }

        // static Loader
        public static bool ReadMesh(BinaryReader bin, RSMFile rsm, out RSMMesh m) {
            m = new RSMMesh(rsm);
            int count;

            m.Head = new RSMMesh.Header();
            m.Head.Name = Tools.ReadWord(bin, 40);
            m.Head.ParentName = Tools.ReadWord(bin, 40);

            count = bin.ReadInt32();
            if ((bin.BaseStream.Length - bin.BaseStream.Position) < (4 * count))
                return false;
            for (int i = 0; i < count; i++)
                m.TextureIndexs.Add(bin.ReadInt32());

            m.Matrix = ReadMatrix(bin);

            count = bin.ReadInt32();
            if ((bin.BaseStream.Length - bin.BaseStream.Position) < (9 * count))
                return false;
            for (int i = 0; i < count; i++)
                m.MainVectors.Add(Tools.ReadVector3(bin));

            count = bin.ReadInt32();
            TextureVertex v;
            bool ignoreColor = !rsm.Version.IsCompatible(1, 2);
            for (int i = 0; i < count; i++) {
                v = new TextureVertex();
                v.Color = (ignoreColor ? 0xFFFFFFFF : bin.ReadUInt32());
                v.U = bin.ReadSingle();
                v.V = bin.ReadSingle();
                m.TextureVectors.Add(v);
            }


            count = bin.ReadInt32();
            for (int i = 0; i < count; i++)
                m.Surfaces.Add(ReadSurface(bin, rsm.Version));

            if (rsm.Version.IsCompatible(1, 5)) {
                count = bin.ReadInt32();
                for (int i = 0; i < count; i++)
                    m.PositionFrames.Add(ReadPositionFrame(bin));
            }

            count = bin.ReadInt32();
            for (int i = 0; i < count; i++)
                m.RotationFrames.Add(ReadRotationFrame(bin));

            return true;
        }

        private static RSMMesh.TransMatrix ReadMatrix(BinaryReader bin) {
            RSMMesh.TransMatrix m = new RSMMesh.TransMatrix();

            // FIX: The Ragnarok-way/order ...
            var m11 = bin.ReadSingle();
            var m21 = bin.ReadSingle();
            var m31 = bin.ReadSingle();
            var m12 = bin.ReadSingle();
            var m22 = bin.ReadSingle();
            var m32 = bin.ReadSingle();
            var m13 = bin.ReadSingle();
            var m23 = bin.ReadSingle();
            var m33 = bin.ReadSingle();
            m.Matrix = new Matrix(
                m11, m12, m13, 0,
                m21, m22, m23, 0,
                m31, m32, m33, 0,
                  0,   0,   0, 1
            );

            m.OffsetTranslation = Tools.ReadVector3(bin);
            m.Position = Tools.ReadVector3(bin);
            m.RotationAngle = bin.ReadSingle();
            m.RotationAxis = Tools.ReadVector3(bin);
            m.Scale = Tools.ReadVector3(bin);

            return m;
        }
        private static RSMMesh.Surface ReadSurface(BinaryReader bin, FileVersion Version) {
            RSMMesh.Surface s = new RSMMesh.Surface();

            s.SurfaceVector = new ushort[3];
            s.TextureVector = new ushort[3];
            for (int i = 0; i < s.SurfaceVector.Length; i++)
                s.SurfaceVector[i] = bin.ReadUInt16();
            for (int i = 0; i < s.TextureVector.Length; i++)
                s.TextureVector[i] = bin.ReadUInt16();

            s.TextureID = bin.ReadUInt16();
            s.Padding = bin.ReadUInt16();
            s.TwoSide = bin.ReadUInt32();
            if (Version.IsCompatible(1, 2))
                s.SmoothGroup = bin.ReadUInt32();
            else
                s.SmoothGroup = 0;

            return s;
        }
        private static RSMMesh.RotationFrame ReadRotationFrame(BinaryReader bin) {
            RSMMesh.RotationFrame f = new RSMMesh.RotationFrame();

            f.Frame = bin.ReadInt32();
            f.Rotation = Tools.ReadVector3(bin);

            return f;
        }
        private static RSMMesh.PositionFrame ReadPositionFrame(BinaryReader bin) {
            RSMMesh.PositionFrame f = new RSMMesh.PositionFrame();

            f.Frame = bin.ReadInt32();
            f.Position = Tools.ReadVector4(bin);

            return f;
        }

    }

}
