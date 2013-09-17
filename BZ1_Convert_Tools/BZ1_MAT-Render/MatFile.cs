using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BZ1_MAT_Render
{
    class MatFile
    {
        public List<MatZone> zoneList;
        public MatFile(System.IO.Stream fileStream)
        {
            zoneList = new List<MatZone>();

            byte[] buffer = new byte[64 * 64 * sizeof(UInt16)];
            while (fileStream.Read(buffer, 0, 64 * 64 * sizeof(UInt16)) > 0)
            {
                zoneList.Add(new MatZone(buffer));
                buffer = new byte[64 * 64 * sizeof(UInt16)];
            }
        }
    }

    class MatZone
    {
        UInt16[,] materials = new UInt16[64,64];

        public MatZone(byte[] buffer)
        {
            System.Buffer.BlockCopy(buffer,0,materials,0,buffer.Length);

            /*int x = 0;
            int y = 0;
            for (int i = 0; i < buffer.Length; i += 2)
            {
                x++;
                if(x >= 64)
                {
                    y++;
                    x = 0;
                }
                materials[x,y] = Buffer.BlockCopy
            }*/
            //Console.WriteLine("test");
        }

        public int GetVariant(int x, int y)
        {
            return (materials[y, x] & 0x3);
        }

        public int GetMix(int x, int y)
        {
            return (materials[y, x] & 0xF0) >> 4;
        }

        public int GetNext(int x, int y)
        {
            return (materials[y, x] & 0xF00) >> 8;
        }

        public int GetBase(int x, int y)
        {
            return (materials[y, x] & 0xF000) >> 12;
        }
    }
}
