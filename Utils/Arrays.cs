using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.Utils
{
   public static class Arrays
    {
       public static T[] Add<T>(T[] array, T one)
       {
           int n = array.Length;
           T[] newArray = new T[n + 1];
           System.Array.Copy(array, newArray, n);
           newArray[n] = one;
           return newArray;
       }

       public static T[] Remove<T>(T[] array, T one)
       {
           throw new NotImplementedException();
       }

       public static string[] TrimInPlace(this string[] source)
       {
           for (int i = 0; i < source.Length; i++)
           {
               source[i] = source[i].Trim();
           }
           return source;
       }

       public static string[] Trim(this string[] source)
       {
           string[] dest = new string[source.Length];
           for (int i = 0; i < source.Length; i++)
           {
               dest[i] = source[i].Trim();
           }
           return dest;
       }
    }
}
