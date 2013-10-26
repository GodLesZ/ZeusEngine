using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Zeus.Client.Tests.MonoGameClientTest.Formats;

namespace Zeus.Client.Tests.MonoGameClientTest {

    public class MapTestRO {
        protected Vector3 mPosition = Vector3.Zero;
        protected Vector3 mRotation = Vector3.Zero;
        protected Vector3 mCenter = Vector3.Zero;

        protected Matrix mWorldMatrix;
        protected Matrix mRotationMatrix;

        protected BasicEffect mBasicEffect;

        #region Ground Properties
        protected List<VertexPositionColorTexture>[] mVerticesList;
        protected VertexBuffer[] mVertexBuffer;
        protected VertexDeclaration[] mVertexDeclaration;
        protected IndexBuffer[] mIndexBuffer;
        protected List<List<int>> mIndices = new List<List<int>>();
        protected int[] mIndiceCount;
        #endregion

        protected GNDFile mGnd;
        protected RSWFile mRsw;
        protected GATFile mGat;
        protected List<RSMFile> mRsm = new List<RSMFile>();
        protected Game mClient;


        public MapTestRO() {
        }

        public virtual void Initialize(Game game, string Filename, string TextureRoot) {
            mClient = game;
            GNDFile.GraphicsDevice = mClient.GraphicsDevice;

            //ROMap convMap = ROMapConverter.Convert( Filename, TextureRoot );
            //convMap.Save( Filename + ".gdmap" );

            System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
            mGnd = new GNDFile(Filename + ".gnd", TextureRoot);
            mRsw = new RSWFile(Filename + ".rsw");
            mGat = new GATFile(Filename + ".gat");
            watch.Stop();
            System.Diagnostics.Debug.WriteLine("ROMap: " + watch.ElapsedMilliseconds + "ms");

            if (mRsw.ModelData.Count > 0) {
                for (int i = 0; i < mRsw.ModelData.Count; i++) {
                    mRsm.Add(new RSMFile(mClient.GraphicsDevice, TextureRoot + @"..\model\" + mRsw.ModelData[i].filename, true));
                }
            }

            mCenter = new Vector3(mGnd.Width * .5f, 0f, mGnd.Height * .5f);

            mBasicEffect = new BasicEffect(mClient.GraphicsDevice);
            mBasicEffect.TextureEnabled = true;
        }

        public void LoadContent() {
            System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
            SetUpGroundVertices();
            System.Diagnostics.Debug.WriteLine(watch.ElapsedMilliseconds + "ms needed");
            watch.Stop();
            watch = null;
        }

        public void Update(InputHelper InputHelper) {
            mRotationMatrix = Matrix.CreateRotationX(mRotation.X) * Matrix.CreateRotationY(mRotation.Y) * Matrix.CreateRotationZ(mRotation.Z);
            mWorldMatrix = Matrix.CreateTranslation(mPosition - mCenter) * mRotationMatrix;
        }

        public void Draw(Matrix view, Matrix projection) {

            DrawTerrain(view, projection);
            DrawModels( view, projection );

        }


        protected virtual void DrawTerrain(Matrix view, Matrix projection) {
            for (int i = 0; i < mVerticesList.Length; i++) {
                mBasicEffect.Projection = projection;
                mBasicEffect.World = mWorldMatrix;
                mBasicEffect.View = view;
                mBasicEffect.Texture = mGnd.Textures[i].TextureTex;
                mBasicEffect.CurrentTechnique.Passes[0].Apply();

                mClient.GraphicsDevice.SetVertexBuffer(mVertexBuffer[i]);
                mClient.GraphicsDevice.Indices = mIndexBuffer[i];
                mClient.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, mVertexBuffer[i].VertexCount / 3, 0, mIndices[i].Count);
            }
        }

        protected virtual void DrawModels(Matrix view, Matrix projection) {
            for (int i = 0; i < mRsm.Count; i++) {
                mRsm[i].DrawSingle(mClient.GraphicsDevice, mBasicEffect, view, projection, mWorldMatrix);
            }
        }


        protected void SetUpGroundVertices() {
            mVerticesList = new List<VertexPositionColorTexture>[mGnd.Textures.Length];
            mIndiceCount = new int[mGnd.Textures.Length];
            for (int i = 0; i < mGnd.Textures.Length; i++) {
                mIndices.Add(new List<int>());
                mVerticesList[i] = new List<VertexPositionColorTexture>();
            }

            GNDFile.TileData tile;
            GNDFile.CubeData cube;

            for (int x = 0; x < mGnd.Width; x++) {
                for (int y = 0; y < mGnd.Height; y++) {
                    int cubeNum = (x + (y * (int)mGnd.Width));
                    if (cubeNum < 0 || cubeNum >= mGnd.Cubes.Length)
                        continue;

                    cube = mGnd.Cubes[cubeNum];
                    if (cube.TileFront != -1) {
                        tile = mGnd.Tiles[cube.TileFront];

                        mVerticesList[tile.TextureIndex].Add(new VertexPositionColorTexture(new Vector3(x + 1, cube.y1 / -13f, -y - 1), Color.White, new Vector2(tile.u1, tile.v1))); // UL
                        mVerticesList[tile.TextureIndex].Add(new VertexPositionColorTexture(new Vector3(x + 1, cube.y3 / -13f, -y - 1), Color.White, new Vector2(tile.u3, tile.v3))); // OL
                        mVerticesList[tile.TextureIndex].Add(new VertexPositionColorTexture(new Vector3(x + 1, cube.y2 / -13f, -y), Color.White, new Vector2(tile.u2, tile.v2))); // UR
                        mVerticesList[tile.TextureIndex].Add(new VertexPositionColorTexture(new Vector3(x + 1, cube.y4 / -13f, -y), Color.White, new Vector2(tile.u4, tile.v4))); // OR

                        AddGroundIndices(tile.TextureIndex);
                    }

                    if (cube.TileLeft != -1) {
                        tile = mGnd.Tiles[cube.TileLeft];

                        mVerticesList[tile.TextureIndex].Add(new VertexPositionColorTexture(new Vector3(x, cube.y1 / -13f, -y - 1), Color.White, new Vector2(tile.u1, tile.v1))); // UL
                        mVerticesList[tile.TextureIndex].Add(new VertexPositionColorTexture(new Vector3(x, cube.y3 / -13f, -y - 1), Color.White, new Vector2(tile.u3, tile.v3))); // OL
                        mVerticesList[tile.TextureIndex].Add(new VertexPositionColorTexture(new Vector3(x, cube.y2 / -13f, -y), Color.White, new Vector2(tile.u2, tile.v2))); // UR
                        mVerticesList[tile.TextureIndex].Add(new VertexPositionColorTexture(new Vector3(x, cube.y4 / -13f, -y), Color.White, new Vector2(tile.u4, tile.v4))); // OR

                        AddGroundIndices(tile.TextureIndex);
                    }

                    if (cube.TileTop != -1) {
                        tile = mGnd.Tiles[cube.TileTop];

                        mVerticesList[tile.TextureIndex].Add(new VertexPositionColorTexture(new Vector3(x, cube.y1 / -13f, -y), Color.White, new Vector2(tile.u1, tile.v1))); // UL
                        mVerticesList[tile.TextureIndex].Add(new VertexPositionColorTexture(new Vector3(x, cube.y3 / -13f, -y - 1), Color.White, new Vector2(tile.u3, tile.v3))); // OL
                        mVerticesList[tile.TextureIndex].Add(new VertexPositionColorTexture(new Vector3(x + 1, cube.y2 / -13f, -y), Color.White, new Vector2(tile.u2, tile.v2))); // UR
                        mVerticesList[tile.TextureIndex].Add(new VertexPositionColorTexture(new Vector3(x + 1, cube.y4 / -13f, -y - 1), Color.White, new Vector2(tile.u4, tile.v4))); // OR

                        AddGroundIndices(tile.TextureIndex);
                    }

                }

            }

            mVertexBuffer = new VertexBuffer[mVerticesList.Length];
            mVertexDeclaration = new VertexDeclaration[mVerticesList.Length];
            mIndexBuffer = new IndexBuffer[mVerticesList.Length];
            for (int i = 0; i < mVerticesList.Length; i++) {
                mVertexDeclaration[i] = new VertexDeclaration(VertexPositionColorTexture.VertexDeclaration.GetVertexElements());
                mVertexBuffer[i] = new VertexBuffer(mClient.GraphicsDevice, mVertexDeclaration[i], mVerticesList[i].Count, BufferUsage.WriteOnly);
                mVertexBuffer[i].SetData<VertexPositionColorTexture>(mVerticesList[i].ToArray());

                mIndexBuffer[i] = new IndexBuffer(mClient.GraphicsDevice, typeof(int), mIndices[i].Count, BufferUsage.WriteOnly);
                mIndexBuffer[i].SetData(mIndices[i].ToArray());
            }

        }

        protected void AddGroundIndices(int index) {
            mIndices[index].Add(mIndiceCount[index]++);     // 0 => UL
            mIndices[index].Add(mIndiceCount[index]++);     // 1 => OL
            mIndices[index].Add(mIndiceCount[index]);       // 2 => UR
            mIndices[index].Add(mIndiceCount[index] - 1);   // 1 => OL
            mIndices[index].Add(++mIndiceCount[index]);     // 3 => OR
            mIndices[index].Add(mIndiceCount[index] - 1);   // 2 => UR
            mIndiceCount[index]++; // start next Loop with 4
        }


    }

}
