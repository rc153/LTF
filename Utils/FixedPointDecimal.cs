using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit
{
    //[Pure] // immutable
    public struct FixedPointDecimal : IEquatable<FixedPointDecimal>, IComparable<FixedPointDecimal>
    {
        private const string FORMAT = "{0:000000}";
        private const int DECIMALS = 6;
        private const int MULT = 1000000;
        private static readonly int[] POWS = new int[] { 1, 10, 100, 1000, 10000, 100000, 1000000 };

        private readonly long RawValue;

        public static readonly FixedPointDecimal MaxValue = FixedPointDecimal.FromRaw(Int64.MaxValue);
        public static readonly FixedPointDecimal Zero = FixedPointDecimal.FromRaw(0);
        public static readonly FixedPointDecimal MinValue = FixedPointDecimal.FromRaw(Int64.MinValue);

        public static bool TryParse(string str, out FixedPointDecimal fp)
        {
            try
            {
                fp = Parse(str);
                return true;
            }
            catch (Exception ex)
            {
                fp = FixedPointDecimal.FromRaw(0);
                if (ex is FormatException || ex is OverflowException || ex is ArgumentNullException)
                    return false;
                else
                    throw;
            }
        }

        public static FixedPointDecimal Parse(string str)
        {
            string[] bits = str.Split('.');
            if (bits.Length < 1 || bits.Length > 2) throw new FormatException(str);
            long newRawValue = Int64.Parse(bits[0].Replace(",","")) * MULT;
            if (bits.Length == 2)
                for (int i = 0; i < Math.Min(DECIMALS, bits[1].Length); i++)
                    newRawValue += (long)(Int32.Parse(bits[1].Substring(i, 1)) * POWS[DECIMALS - i - 1]);
            return FixedPointDecimal.FromRaw(newRawValue);
        }

        public static FixedPointDecimal FromRaw(long raw)
        {
            return new FixedPointDecimal(raw);
        }

        private FixedPointDecimal(long raw)
        {
            RawValue = raw;
        }

        public long ToRaw()
        {
            return RawValue;
        }

        public override int GetHashCode()
        {
            return RawValue.GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(2 * DECIMALS);
            long intValue = RawValue / MULT;
            sb.Append(intValue.ToString());
            sb.Append(".");
            long decValue = RawValue - (intValue * MULT);
            sb.Append(String.Format(FORMAT, decValue));
            return sb.ToString();
        }

        #region Math Functions

        // round toward 0
        public static FixedPointDecimal Mean(FixedPointDecimal fp1, FixedPointDecimal fp2)
        {
            return FixedPointDecimal.FromRaw((fp1.RawValue + fp2.RawValue) >> 1);
        }

        public FixedPointDecimal Round()
        {
            return FixedPointDecimal.FromRaw((long)Math.Round((double)RawValue / MULT) * MULT);
        }

        public FixedPointDecimal RoundUp()
        {
            return FixedPointDecimal.FromRaw((long)Math.Ceiling((double)RawValue / MULT) * MULT);
        }

        public FixedPointDecimal RoundDown()
        {
            return FixedPointDecimal.FromRaw((long)Math.Floor((double)RawValue / MULT) * MULT);
        }

        public FixedPointDecimal Round(FixedPointDecimal inc)
        {
            return FixedPointDecimal.FromRaw((long)Math.Round((double)RawValue / inc.RawValue) * inc.RawValue);
        }

        public FixedPointDecimal RoundUp(FixedPointDecimal inc)
        {
            return FixedPointDecimal.FromRaw((long)Math.Ceiling((double)RawValue / inc.RawValue) * inc.RawValue);
        }

        public FixedPointDecimal RoundDown(FixedPointDecimal inc)
        {
            return FixedPointDecimal.FromRaw((long)Math.Floor((double)RawValue / inc.RawValue) * inc.RawValue);
        }

        public FixedPointDecimal Abs()
        {
            return FixedPointDecimal.FromRaw(Math.Abs(RawValue));
        }

        #endregion

        #region Math Operators

        public static FixedPointDecimal operator +(FixedPointDecimal one)
        {
            return one;
        }

        public static FixedPointDecimal operator +(FixedPointDecimal one, FixedPointDecimal other)
        {
            return FixedPointDecimal.FromRaw(one.RawValue + other.RawValue);
        }

        public static FixedPointDecimal operator -(FixedPointDecimal one)
        {
            return FixedPointDecimal.FromRaw(-one.RawValue);
        }

        public static FixedPointDecimal operator -(FixedPointDecimal one, FixedPointDecimal other)
        {
            return FixedPointDecimal.FromRaw(one.RawValue - other.RawValue);
        }

        #endregion

        #region Comparison

        public bool Equals(FixedPointDecimal other)
        {
            return other.RawValue == this.RawValue;
        }

        public override bool Equals(object obj)
        {
            if (obj is FixedPointDecimal)
                return Equals((FixedPointDecimal)obj);
            else
                return false;
        }

        public int CompareTo(FixedPointDecimal other)
        {
            long otherRawValue = other.RawValue;
            if (RawValue < otherRawValue)
            {
                return -1;
            }
            if (RawValue > otherRawValue)
            {
                return 1;
            }
            return 0;
        }

        public static bool operator ==(FixedPointDecimal one, FixedPointDecimal other)
        {
            return one.RawValue == other.RawValue;
        }

        public static bool operator !=(FixedPointDecimal one, FixedPointDecimal other)
        {
            return one.RawValue != other.RawValue;
        }

        public static bool operator >=(FixedPointDecimal one, FixedPointDecimal other)
        {
            return one.RawValue >= other.RawValue;
        }

        public static bool operator <=(FixedPointDecimal one, FixedPointDecimal other)
        {
            return one.RawValue <= other.RawValue;
        }

        public static bool operator >(FixedPointDecimal one, FixedPointDecimal other)
        {
            return one.RawValue > other.RawValue;
        }

        public static bool operator <(FixedPointDecimal one, FixedPointDecimal other)
        {
            return one.RawValue < other.RawValue;
        }

        #endregion

        #region Conversion

        public double ToDouble()
        {
            return (double)this;
        }

        public int ToInt()
        {
            return (int)this;
        }

        public long ToLong()
        {
            return (long)this;
        }

        public static explicit operator long(FixedPointDecimal src)
        {
            return src.RawValue / MULT;
        }

        public static explicit operator int(FixedPointDecimal src)
        {
            return (int)(long)src;
        }

        public static explicit operator double(FixedPointDecimal src)
        {
            return (double)src.RawValue / MULT;
        }

        public static explicit operator FixedPointDecimal(long src)
        {
            return FixedPointDecimal.FromRaw(src * MULT);
        }

        public static explicit operator FixedPointDecimal(int src)
        {
            return (FixedPointDecimal)(long)src;
        }

        public static explicit operator FixedPointDecimal(double src)
        {
            return FixedPointDecimal.FromRaw((long)Math.Round(src * MULT));
        }

        #endregion

    }
}
