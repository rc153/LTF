using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.Ids
{
   public class IdUtils
    {
        private static string[] monthNumberToLetter = new string[] { "F", "G", "H", "J", "K", "M", "N", "Q", "U", "V", "X", "Z" };

        // january = 1
        public static string GetMonthLetter(int monthNumber)
        {
            return monthNumberToLetter[monthNumber - 1];
        }

        // january = 1
        // returns <= 0 if not found
        public static int GetMonthNumber(string letter)
        {
            return Array.BinarySearch(monthNumberToLetter, letter) + 1;
        }
    }
}
