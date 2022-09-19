using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSocketApp
{
    class TetrisData
    {
        public byte PlayerIndex;
public byte[,] Field;
        public byte[] Next;
        public byte Hold;
        public byte CanHold;
        public byte NowMinoKind;
        public byte NowMinoRotation;
        public byte NowMinoPosition;
    }
}
