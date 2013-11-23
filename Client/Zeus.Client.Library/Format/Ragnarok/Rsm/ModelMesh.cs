using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Zeus.Client.Library.Format.Ragnarok.Rsm {

    public class ModelMesh {

        private readonly Format _rsmRoot;

        public string Name {
            get;
            protected set;
        }

        public string ParentName {
            get;
            protected set;
        }

        public ModelMesh Parent {
            get;
            set;
        }

        public List<int> Textureids {
            get;
            protected set;
        }

        public Dictionary<int, Texture2D> TextureCache {
            get;
            protected set;
        }

        public Matrix OffsetMatrix {
            get;
            protected set;
        }

        public Vector3 Position {
            get;
            protected set;
        }

        public Vector3 Position2 {
            get;
            protected set;
        }


        public float Rotationangle {
            get;
            protected set;
        }

        public Vector3 Rotationaxis {
            get;
            protected set;
        }

        public Vector3 Scale {
            get;
            protected set;
        }


        public List<Vector3> VerticePositions {
            get;
            protected set;
        }

        public List<Vector2> TextureCoordinats {
            get;
            protected set;
        }

        public List<ModelMeshFace> Faces {
            get;
            protected set;
        }

        public List<ModelAnimationFrame> Frames {
            get;
            protected set;
        }


        public List<ModelMesh> Children {
            get;
            protected set;
        }

        public Vector3 Bbmin {
            get;
            protected set;
        }

        public Vector3 Bbmax {
            get;
            protected set;
        }

        public Vector3 Bbrange {
            get;
            protected set;
        }

        public Matrix Matrix1 {
            get;
            protected set;
        }

        public Matrix Matrix2 {
            get;
            protected set;
        }

        public bool IsRoot {
            get { return _rsmRoot.RootMesh == this; }
        }

        public VertexDeclaration VertexDeclaration {
            get;
            protected set;
        }

        public Dictionary<int, VertexBuffer> VertexBuffer {
            get;
            protected set;
        }

        public Dictionary<int, List<VertexPositionNormalTexture>> VerticeListByTextureIndex {
            get;
            protected set;
        }


        protected ModelMesh() {
            Textureids = new List<int>();
            TextureCoordinats = new List<Vector2>();
            Faces = new List<ModelMeshFace>();
            VerticePositions = new List<Vector3>();
            Frames = new List<ModelAnimationFrame>();

            Children = new List<ModelMesh>();
            OffsetMatrix = Matrix.Identity;
            Matrix1 = Matrix.Identity;
            Matrix2 = Matrix.Identity;

            Bbmin = new Vector3(9999999, 9999999, 9999999);
            Bbmax = new Vector3(-9999999, -9999999, -9999999);

            VerticeListByTextureIndex = new Dictionary<int, List<VertexPositionNormalTexture>>();
            TextureCache = new Dictionary<int, Texture2D>();
        }

        public ModelMesh(Format rsmRoot)
            : this() {
            _rsmRoot = rsmRoot;
            Read(rsmRoot.Reader);
        }


        public void UpdateChildren(List<ModelMesh> meshes) {
            foreach (var mesh in meshes.Where(mesh => mesh.ParentName == Name && mesh != this)) {
                mesh.Parent = this;
                Children.Add(mesh);
            }

            foreach (var mesh in Children) {
                mesh.UpdateChildren(meshes);
            }
        }

        public void SetBoundingBox(ref Vector3 bbmin, ref Vector3 bbmax) {

            if (Parent != null) {
                Bbmin = Bbmax = new Vector3(0, 0, 0);
            }

            var myMat = OffsetMatrix;
            foreach (var face in Faces) {
                for (var ii = 0; ii < 3; ii++) {
                    var v = new Vector4(VerticePositions[face.Vertices[ii]], 1);
                    // v = myMat * v;
                    v = Vector4.Transform(v, myMat);
                    if (Parent != null || Children.Count != 0) {
                        v += new Vector4(Position + Position2, 1);
                    }


                    Bbmin = new Vector3(Math.Min(Bbmin.X, v.X), Math.Min(Bbmin.Y, v.Y), Math.Min(Bbmin.Z, v.Z));
                    Bbmax = new Vector3(Math.Max(Bbmax.X, v.X), Math.Max(Bbmax.Y, v.Y), Math.Max(Bbmax.Z, v.Z));
                }
            }

            Bbrange = (Bbmin + Bbmax) / 2.0f;

            for (var c = 0; c < 3; c++) {
                for (var i = 0; i < 3; i++) {
                    bbmin = new Vector3(Math.Min(bbmin.X, Bbmin.X), Math.Min(bbmin.Y, Bbmin.Y), Math.Min(bbmin.Z, Bbmin.Z));
                    bbmax = new Vector3(Math.Max(bbmax.X, Bbmax.X), Math.Max(bbmax.Y, Bbmax.Y), Math.Max(bbmax.Z, Bbmax.Z));
                }
            }

            foreach (var mesh in Children) {
                mesh.SetBoundingBox(ref bbmin, ref bbmax);
            }

        }

        public void SetBoundingBox2(ref Matrix mat, ref Vector3 bbmin, ref Vector3 bbmax) {

            var mat1 = mat * Matrix1;
            foreach (var face in Faces) {
                for (var ii = 0; ii < 3; ii++) {
                    var v = Vector4.Transform(new Vector4(VerticePositions[face.Vertices[ii]], 1), mat1);
                    bbmin = new Vector3(Math.Min(bbmin.X, v.X), Math.Min(bbmin.Y, v.Y), Math.Max(bbmin.Z, v.Z));
                    bbmax = new Vector3(Math.Min(bbmax.X, v.X), Math.Min(bbmax.Y, v.Y), Math.Max(bbmax.Z, v.Z));
                }
            }

            foreach (var mesh in Children) {
                mesh.SetBoundingBox2(ref mat1, ref bbmin, ref bbmax);
            }
        }

        public void CalcMatrix1() {

            if (Parent == null) {
                if (Children.Count > 0)
                    Matrix1 = Matrix.CreateTranslation(new Vector3(-_rsmRoot.BbRange.X, -_rsmRoot.BbMax.Y, -_rsmRoot.BbRange.Z));
                else
                    Matrix1 = Matrix.CreateTranslation(new Vector3(0, -_rsmRoot.BbMax.Y + _rsmRoot.BbRange.Y, 0));
            } else {
                Matrix1 = Matrix.CreateTranslation(Position);
            }

            if(Frames.Count == 0)
	        {
                if (Math.Abs(Rotationangle) > 0.01) {
                    // Matrix1 = glm::rotate(Matrix1, Rotationangle*180.0f/3.14159f, Rotationaxis);
                    Matrix1 = RotateMatrix(Matrix1, Rotationaxis, Rotationangle*180.0f/3.14159f);
                }
            }
	        else
	        {
			        Matrix1 *= Matrix.CreateFromQuaternion(Quaternion.Normalize(Frames[0].Quaternion));
 	        }

	        Matrix1 = Matrix.Multiply(Matrix1, Matrix.CreateScale(Scale));

            foreach (var mesh in Children) {
                mesh.CalcMatrix1();
            }
        }

        public void CalcMatrix2() {

            if (Parent == null && Children.Count == 0) {
                Matrix2 = Matrix.CreateTranslation(-1.0f*_rsmRoot.BbRange);
            } else {
                Matrix2 = Matrix.CreateTranslation(Position2);
            }

            Matrix2 *= OffsetMatrix;

            foreach (var mesh in Children) {
                mesh.CalcMatrix2();
            }
        }

        public void CalculateVertexArray(GraphicsDevice graphicsDevice) {
            VerticeListByTextureIndex = new Dictionary<int, List<VertexPositionNormalTexture>>();

            foreach (var face in Faces) {
                for (var c = 0; c < 3; c++) {
                    var position = VerticePositions[face.Vertices[c]];
                    var textureCoordinate = TextureCoordinats[face.TextureVertices[c]];
                    var vertexPositionNormalTex = new VertexPositionNormalTexture(position, face.Normal, textureCoordinate);

                    if (VerticeListByTextureIndex.ContainsKey(face.TextureIndex) == false) {
                        VerticeListByTextureIndex.Add(face.TextureIndex, new List<VertexPositionNormalTexture>());
                    }
                    
                    VerticeListByTextureIndex[face.TextureIndex].Add(vertexPositionNormalTex);
                }
            }

            VertexBuffer = new Dictionary<int, VertexBuffer>();
            foreach (var kvp in VerticeListByTextureIndex) {
                var verticeList = kvp.Value;
                var textureIndex = kvp.Key;
                //VertexDeclaration = new VertexDeclaration(VertexPositionNormalTexture.VertexDeclaration.GetVertexElements());
                var vertexBuffer = new VertexBuffer(graphicsDevice, VertexPositionNormalTexture.VertexDeclaration, verticeList.Count, BufferUsage.WriteOnly);
                vertexBuffer.SetData(verticeList.ToArray());
                VertexBuffer.Add(textureIndex, vertexBuffer);
            }


            // Load textures
            foreach (var index in Textureids) {
                var texture = _rsmRoot.LoadTexture(index, graphicsDevice);
                TextureCache.Add(index, texture);
            }
        }
        public void Draw(GraphicsDevice device, BasicEffect effect, Matrix view, Matrix projection, Matrix world) {
            if (VerticeListByTextureIndex.Count == 0) {
                CalculateVertexArray(device);
            }

            // Draw each list 
            foreach (var kvp in VerticeListByTextureIndex) {
                var verticeList = kvp.Value;
                var textureIndex = kvp.Key;

                effect.LightingEnabled = false;
                effect.Texture = (TextureCache.ContainsKey(textureIndex) ? TextureCache[textureIndex] : null);
                effect.Projection = projection;
                effect.World = world;
                effect.View = view;
                effect.CurrentTechnique.Passes[0].Apply();

                device.SetVertexBuffer(VertexBuffer[textureIndex]);
                device.DrawPrimitives(PrimitiveType.TriangleList, 0, verticeList.Count);
            }

        }


        protected void Read(BinaryReader reader) {
            Name = reader.ReadStringIso(40);
            ParentName = reader.ReadStringIso(40);

            var textureCount = reader.ReadInt32();
            for (var i = 0; i < textureCount; i++) {
                var textureIndex = reader.ReadInt32();
                if (textureIndex < 0 || textureIndex >= _rsmRoot.TexturesPaths.Count) {
                    throw new Exception("Invalid texture index #" + textureIndex);
                }

                Textureids.Add(textureIndex);
            }

            OffsetMatrix = new Matrix(
                reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 0,
                reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 0,
                reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 0,
                0, 0, 0, 0
            );

            // @TODO: First was pos_
            Position2 = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            Position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            Rotationangle = reader.ReadSingle();
            Rotationaxis = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            Scale = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

            var vertexCount = reader.ReadInt32();
            for (var i = 0; i < vertexCount; i++) {
                var vertex = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                VerticePositions.Add(vertex);
            }

            var texCoordCount = reader.ReadInt32();
            for (var i = 0; i < texCoordCount; i++) {
                if (_rsmRoot.FileHeader.Version.Version >= 0x0102) {
                    reader.ReadSingle();
                }

                var texCoord = new Vector2(reader.ReadSingle(), reader.ReadSingle());
                TextureCoordinats.Add(texCoord);
            }

            var faceCount = reader.ReadInt32();
            for (var i = 0; i < faceCount; i++) {
                var face = new ModelMeshFace(reader);
                Faces.Add(face);
            }

            var frameCount = reader.ReadInt32();
            for (var i = 0; i < frameCount; i++) {
                var frame = new ModelAnimationFrame(reader);
                Frames.Add(frame);
            }

        }

        protected Matrix RotateMatrix(Matrix m, Vector3 v, float angle) {
            // 1:1 C# version of "matrix_transform.inl":
            // detail::tmat4x4<T> rotate(detail::tmat4x4<T> const & m,T const & angle, detail::tvec3<T> const & v)

            // @TODO: Maybe this is Matrix.CreateFromAxisAngle ?
            
            var a = MathHelper.ToRadians(angle);
		    var c = (float)Math.Cos(a);
		    var s = (float)Math.Sin(a);

		    var axis = Vector3.Normalize(v);

		    var temp = axis * (1 - c);

            var rotate = Matrix.Identity;
            rotate.M11 = c + (temp.X*axis.X);
            rotate.M12 = 0 + (temp.X*axis.Y) + (s*axis.Z);
            rotate.M13 = 0 + (temp.X*axis.Z) - (s*axis.Y);

            rotate.M21 = 0 + (temp.Y*axis.X) - (s*axis.Z);
            rotate.M22 = c + (temp.Y*axis.Y);
            rotate.M23 = 0 + (temp.Y*axis.Z) + (s*axis.X);

            rotate.M31 = 0 + (temp.Z*axis.X) + (s*axis.Y);
            rotate.M32 = 0 + (temp.Z*axis.Y) - (s*axis.X);
            rotate.M33 = c + (temp.Z*axis.Z);

            return Matrix.Multiply(rotate, m);
        }

    }

}