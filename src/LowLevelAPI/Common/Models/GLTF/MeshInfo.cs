using glTFLoader.Schema;

namespace Common
{
    public class MeshInfo
    {
        public BufferView IndicesBufferView;
        public BufferView[] AttributeBufferView;

        public MeshInfo(BufferView indices, BufferView[] attributes)
        {
            this.IndicesBufferView = indices;
            this.AttributeBufferView = attributes;
        }
    }
}
