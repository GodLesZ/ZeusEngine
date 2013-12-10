using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Zeus.Client.Library.Format.Ragnarok.Rsm {

    public class Format : FileFormat {

        public override char[] ExpectedMagicBytes {
            get { return new[] { 'G', 'R', 'S', 'M' }; }
        }

        public int AnimationLength {
            get;
            protected set;
        }

        public ModelShadeType ShadeType {
            get;
            protected set;
        }

        public byte Alpha {
            get;
            protected set;
        }

        public List<string> TexturesPaths {
            get;
            protected set;
        }

        public Dictionary<int, Texture2D> TexturesCache {
            get;
            protected set;
        }

        public ModelMesh RootMesh {
            get;
            protected set;
        }

        public List<ModelMesh> Meshes {
            get;
            protected set;
        }

        public bool IsAnimated {
            get;
            protected set;
        }

        public float MaxRange {
            get;
            protected set;
        }
        public Vector3 BbRange {
            get;
            protected set;
        }
        public Vector3 BbMax {
            get;
            protected set;
        }
        public Vector3 BbMin {
            get;
            protected set;
        }

        protected Vector3 _realBbMin;
        protected Vector3 _realBbMax;
        protected Vector3 _realBbRange;

        protected Format() {
        }

        public Format(string filepath)
            : base(filepath) {
        }

        public Format(byte[] data)
            : base(data) {
        }

        public Format(Stream stream)
            : base(stream) {
        }


        public void CalculateMeshes(GraphicsDevice graphicsDevice) {
            // @TODO: Cache in a global buffer? If so, how to handle multiple textures?
            Meshes.ForEach(m => m.CalculateVertexArray(graphicsDevice));
        }

        public Texture2D LoadTexture(int index, GraphicsDevice device) {
            if (TexturesCache.ContainsKey(index) == false) {
                // @TODO: Dynamic base path
                var dataFilepath = TexturesPaths[index];
                var filepath = string.Format("C:/Games/RO/{0}", dataFilepath);
                TexturesCache[index] = ImageExtensions.FromFile(filepath, device);
            }

            return TexturesCache[index];
        }

        public void Draw(GraphicsDevice device, BasicEffect effect, Matrix view, Matrix projection, Matrix world) {
            Meshes.ForEach(m => m.Draw(device, effect, view, projection, world));
        }


        protected override bool ReadInternal() {
            RootMesh = null;
            TexturesPaths = new List<string>();
            TexturesCache = new Dictionary<int, Texture2D>();
            Meshes = new List<ModelMesh>();

            if (base.ReadInternal() == false) {
                return false;
            }

            AnimationLength = Reader.ReadInt32();
            ShadeType = (ModelShadeType)Reader.ReadInt32();
            if (FileHeader.Version.Version >= 0x0104) {
                Alpha = Reader.ReadByte();
            } else {
                Alpha = 0;
            }

            var unknown = Reader.ReadBytes(16);
            var textureCount = Reader.ReadInt32();
            for (var i = 0; i < textureCount; i++) {
                var textureName = Reader.ReadStringIso(40);
                TexturesPaths.Add(string.Format("data/texture/{0}", textureName));
            }

            var mainMeshName = Reader.ReadStringIso(40);
            var meshCount = Reader.ReadInt32();
            for (var i = 0; i < meshCount; i++) {
                var mesh = new ModelMesh(this);
                Meshes.Add(mesh);

                if (mesh.Name == mainMeshName) {
                    RootMesh = mesh;
                }
            }

            if (RootMesh == null) {
                throw new Exception("Failed to find main-mesh node");
            }

            RootMesh.Parent = null;
            RootMesh.UpdateChildren(Meshes);
            
	        var bbmin = new Vector3(999999, 999999, 999999);
            var bbmax = new Vector3(-999999, -999999, -9999999);
            RootMesh.SetBoundingBox(ref bbmin, ref bbmax);
            BbMin = bbmin;
            BbMax = bbmax;
	        BbRange = (BbMin + BbMax) / 2.0f;

	        RootMesh.CalcMatrix1();
	        RootMesh.CalcMatrix2();
            
	        _realBbMin = new Vector3(999999, 999999, 999999);
	        _realBbMax = new Vector3(-999999, -999999, -999999);
	        //glm::mat4 mat = glm::scale(glm::vec3(1,-1,1));
            var mat = Matrix.CreateScale(1, -1, 1);
	        RootMesh.SetBoundingBox2(ref mat, ref _realBbMin, ref _realBbMax);
	        _realBbRange = (_realBbMax + _realBbMin) / 2.0f;
	        MaxRange = 
                Math.Max(
                    Math.Max(_realBbMax.X, -_realBbMin.X), 
                    Math.Max(
                        Math.Max(_realBbMax.Y, -_realBbMin.Y), 
                        Math.Max(_realBbMax.Z, -_realBbMin.Z)
                    )
                );
            

            return true;
        }

    }

}