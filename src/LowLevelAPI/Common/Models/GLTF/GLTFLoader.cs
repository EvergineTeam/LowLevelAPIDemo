using glTFLoader;
using glTFLoader.Schema;
using System;
using System.IO;
using Evergine.Platform;

namespace Common
{
    public class GLTFLoader : IDisposable
    {
        public Gltf model;
        private AssetsDirectory assetDirectory;

        public BufferInfo[] Buffers;
        public MeshInfo[] Meshes;

        public GLTFLoader(AssetsDirectory assetDirectory, string filePath)
        {
            this.assetDirectory = assetDirectory;
            using (var stream = this.assetDirectory.Open(filePath))
            {
                if (stream == null || !stream.CanRead)
                {
                    throw new ArgumentException("Invalid parameter. Stream must be readable", "imageStream");
                }

                Stream seekedStream = stream;
                MemoryStream memstream = null;
                if (!stream.CanSeek)
                {
                    memstream = new MemoryStream();
                    stream.CopyTo(memstream);

                    memstream.Seek(0, SeekOrigin.Begin);
                    seekedStream = memstream;
                }

                this.model = Interface.LoadModel(seekedStream);
                string folder = Path.GetDirectoryName(filePath);
                this.ReadModel(folder);
            }
        }

        private void ReadModel(string folder)
        {
            // read all buffers
            int numBuffers = this.model.Buffers.Length;
            this.Buffers = new BufferInfo[numBuffers];

            for (int i = 0; i < numBuffers; ++i)
            {
                var uri = Path.Combine(folder, this.model.Buffers[i].Uri);
                var bufferBytes = this.model.LoadBinaryBuffer(i, this.GetExternalFileSolver(uri));
                this.Buffers[i] = new BufferInfo(bufferBytes);
            }

            // Read meshes
            int meshCount = this.model.Meshes.Length;
            this.Meshes = new MeshInfo[meshCount];
            for (int m = 0; m < meshCount; m++)
            {
                var mesh = this.model.Meshes[m];

                BufferView indices = null;
                BufferView[] attributes = null;
                for (int p = 0; p < mesh.Primitives.Length; p++)
                {
                    var primitive = mesh.Primitives[p];

                    if (primitive.Indices.HasValue)
                    {
                        indices = this.ReadAccessor(primitive.Indices.Value);
                    }

                    int attributeCount = primitive.Attributes.Values.Count;
                    attributes = new BufferView[attributeCount];
                    int insertIndex = 0;
                    foreach (var attribute in primitive.Attributes)
                    {
                        attributes[insertIndex++] = this.ReadAccessor(attribute.Value);
                    }
                }

                this.Meshes[m] = new MeshInfo(indices, attributes);
            }
        }

        private Func<string, Byte[]> GetExternalFileSolver(string gltfFilePath)
        {            
            return assets =>
            {
                using (var stream = this.assetDirectory.Open(gltfFilePath))
                {

                    using (MemoryStream reader = new MemoryStream())
                    {
                        stream.CopyTo(reader);
                        return reader.ToArray();
                    }
                }
            };
        }

        private BufferView ReadAccessor(int index)
        {
            var accessor = this.model.Accessors[index];

            if (accessor.BufferView.HasValue)
            {
                return this.model.BufferViews[accessor.BufferView.Value];
            }
            else
            {
                return null;
            }
        }

        public void Dispose()
        {
            if (this.Buffers == null)
            {
                return;
            }

            for (int i = 0; i < this.Buffers.Length; i++)
            {
                this.Buffers[i].Dispose();
            }

            this.Buffers = null;
        }
    }
}
