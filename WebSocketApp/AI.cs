using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSocketApp
{
    internal interface  IAI
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

        public   List<byte> GetBestResult(byte[,] field, byte[] next, byte current, byte hold);
    }

    class MisaMino : IAI
    {
        bool _finished;
        List<MisaMinoNET.Instruction> _result = null;

        public List<byte> GetBestResult(byte[,] field, byte[] next, byte current, byte hold)
        {

            MisaMinoNET.MisaMino.Finished += MisaMino_Finished;
            int? convertedhold;
            if (hold == (byte)IAI.MinoKind.None)
                convertedhold = null;
            else
                convertedhold = ConvertMino(hold);



            _finished = false;
            MisaMinoNET.MisaMino.FindMove(ConvertNext(next), current, hold, 20, ConvertField(field), 0, 0, 0);

            MisaMinoNET.MisaMino.Finished -= MisaMino_Finished;

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

        private void MisaMino_Finished(bool success)
        {
            _finished = true;
            _result = MisaMinoNET.MisaMino.LastSolution.Instructions;
        }

        int ConvertMino(byte mino)
        {
            throw new NotImplementedException();
        }

        int[] ConvertNext(byte[] next)
        {
            throw new NotImplementedException();
        }

        int[,] ConvertField(byte[,] field)
        {
            throw new NotImplementedException();
        }

        List<IAI.Action> GetBestResult()
        {
            throw new NotImplementedException();
        }
    }
}
