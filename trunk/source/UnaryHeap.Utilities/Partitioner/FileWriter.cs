using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnaryHeap.Utilities.D2;

namespace Partitioner
{
    class FileWriter : IDisposable
    {
        Stream output;
        BinaryWriter writer;

        public FileWriter(string filename)
        {
            output = File.Create(filename);
            writer = new BinaryWriter(output);
        }

        public void Dispose()
        {
            writer.Flush();
            output.Dispose();
        }


        public void WriteNodeCount(int count)
        {
            writer.Write(count);
        }

        public void WriteBranchNode(int splitterId, int frontChildId, int backChildId)
        {
            writer.Write((byte)0x00);
            writer.Write(splitterId);
            writer.Write(frontChildId);
            writer.Write(backChildId);
        }

        public void WriteLeafNode(int roomId, int surfCount, int firstSurfaceId)
        {
            writer.Write((byte)0xFF);
            writer.Write(roomId);
            writer.Write(surfCount);
            writer.Write(firstSurfaceId);
        }


        public void WriteSurfaceCount(int count)
        {
            writer.Write(count);
        }

        public void WriteSurface(int startVertexId, int endVertexId)
        {
            writer.Write(startVertexId);
            writer.Write(endVertexId);
        }


        public void WriteVertexCount(int count)
        {
            writer.Write(count);
        }

        public void WriteVertex(Point2D p)
        {
            writer.Write((double)p.X);
            writer.Write((double)p.Y);
        }


        public void WriteRoomCount(int count)
        {
            writer.Write(count);
        }

        public void WriteWroom(string name)
        {
            writer.Write(Encoding.ASCII.GetByteCount(name));
            writer.Write(Encoding.ASCII.GetBytes(name));
        }


        public void WriteSplitterCount(int count)
        {
            writer.Write(count);
        }

        public void WriteSplitter(Hyperplane2D splitter)
        {
            writer.Write((double)splitter.A);
            writer.Write((double)splitter.B);
            writer.Write((double)splitter.C);
        }
    }
}
