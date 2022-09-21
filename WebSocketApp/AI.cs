using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSocketApp
{
    internal interface IAI
    {
        public enum Action
        {
            MoveRight,
            MoveLeft,
            RotateRight,
            RotateLeft,
            Rotate180,
            HardDrop,
            SoftDrop,
            Hold,
            None
        }

        public enum MinoKind
        {
            S,
            Z,
            L,
            J,
            T,
            O,
            I,
            None,
            Ojama,
            DeepOjama,
        }

        public List<byte> GetBestResult(byte[,] field, byte[] next, byte current, byte hold);
    }

    class AIMisaMino 
    {
     static   bool _finished;
     static   List<MisaMinoNET.Instruction> _result = null;

        public List<byte> GetBestResult(byte[,] field, byte[] next, byte current, byte hold)
        {

           
            Debug.WriteLine("aaa");
            int? convertedhold;
            if (hold == (byte)IAI.MinoKind.None)
                convertedhold = null;
            else
                convertedhold = ConvertMino(hold);



            _finished = false;
            MisaMinoNET.MisaMino.FindMove(ConvertNext(next),ConvertMino( current), convertedhold, 19, ConvertField(field), 0, 0, 0);
           // MisaMinoNET.MisaMino.FindMove()
           while(!_finished)
            { }

            MisaMinoNET.MisaMino.Finished -= MisaMinoA_Finished;

            return ConvertAction(_result);
        }

        List<byte> ConvertAction(List<MisaMinoNET.Instruction> actions)
        {
            var result = new List<byte>();
            //for zero index command
            result.Add(0);

            foreach (var action in actions)
            {
                switch (action)
                {
                    case MisaMinoNET.Instruction.L:
                        result.Add((byte)IAI.Action.MoveLeft);
                        break;

                    case MisaMinoNET.Instruction.R:
                        result.Add((byte)IAI.Action.MoveRight);
                        break;

                    case MisaMinoNET.Instruction.D:
                        result.Add((byte)IAI.Action.SoftDrop);
                        break;

                    case MisaMinoNET.Instruction.DROP:
                        result.Add((byte)IAI.Action.HardDrop);
                        break;

                    case MisaMinoNET.Instruction.RSPIN:
                        result.Add((byte)IAI.Action.RotateRight);
                        break;

                    case MisaMinoNET.Instruction.LSPIN:
                        result.Add((byte)IAI.Action.RotateLeft);
                        break;

                    case MisaMinoNET.Instruction.SPIN2:
                        result.Add((byte)IAI.Action.Rotate180);
                        break;

                    case MisaMinoNET.Instruction.HOLD:
                        result.Add((byte)IAI.Action.Hold);
                        break;

                    default:
                        throw new Exception();
                }

            }
            return result;
        }

      static  public void MisaMinoA_Finished(bool success)
        {
            _result = MisaMinoNET.MisaMino.LastSolution.Instructions;
            _finished = true;
        }

        int ConvertMino(byte mino)
        {
            if (mino == (byte)IAI.MinoKind.L)
                return (byte)IAI.MinoKind.J;
            else if (mino == (byte)IAI.MinoKind.J)
                return (byte)IAI.MinoKind.L;
            else if (mino == (byte)IAI.MinoKind.None)
                return 255;

            return mino;
        }

        int[] ConvertNext(byte[] next)
        {
            var result = new int[next.Length];
            for (int i = 0; i < next.Length; i++)
                result[i] = ConvertMino(next[i]);

            return result;
        }

        int[,] ConvertField(byte[,] field)
        {
            var result = new int[10, 20];
            for (int y = 0; y < 20; y++)
            {
                for (int x = 0; x < 10; x++)
                {
                    result[x, y] = ConvertMino(field[x, y]);
                }

            }

            return result;
        }

    }
}
