using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WebSocketApp
{
    public static class ClassExtension
    {
        public static T Pop<T>(this IList<T> list)
        {
            var result = list[0];
            list.RemoveAt(0);
            return result;
        }

        public static byte[,] RevertY(this byte[,] list)
        {
            return null;
            var result=new byte[10,20];
            for(int x=0;x<10;x++)
            {
                for(int y=0;y<20;y++)
                {
                    list[x,y] = list[x,y];
                }
            }
        }
    }
}
