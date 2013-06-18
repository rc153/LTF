using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Toolkit.Ids
{
    // International Securities Identification Number
    public class ISIN
    {
        private string value;

        public string Value
        {
            get { return value + CheckDigit; }
        }

        public int CheckDigit
        {
            get;
            private set;
        }

        public string CountryCode
        {
            get { return value.Substring(0, 2); }
        }

        public string CoreCode
        {
            get { return value.Substring(2, 9); }
        }

        public ISIN(string isinWithOrWithoutCheckDigit)
        {
            if (!IsValidFormatted(isinWithOrWithoutCheckDigit))
                throw new ArgumentException("The ISIN value is not valid formatted.", "isinWithOrWithoutCheckDigit");
            if (isinWithOrWithoutCheckDigit.Length == 12 && !IsValidCheckDigit(isinWithOrWithoutCheckDigit))
                throw new ArgumentException("The ISIN check digit is not valid.", "isinWithOrWithoutCheckDigit");

            value = isinWithOrWithoutCheckDigit.ToUpperInvariant().Substring(0, 11);
            CheckDigit = computeCheckDigit(value);
        }

        public override string ToString()
        {
            return Value;
        }

        private static bool IsValidCheckDigit(string isinWithCheckDigit)
        {
            return computeCheckDigit(isinWithCheckDigit.Substring(0, 11)) == Int32.Parse(isinWithCheckDigit.Substring(11));
        }

        private static bool IsValidFormatted(string isin)
        {
            if (isin.Length != 11 && isin.Length != 12) return false;
            if (!Char.IsLetter(isin[0]) || !Char.IsLetter(isin[1])) return false;
            for (int i = 2; i <= 10; i++) if (!Char.IsLetterOrDigit(isin[i])) return false;
            return isin.Length == 11 || Char.IsDigit(isin[11]);
        }

        private static int computeCheckDigit(string isin)
        {
            List<int> checkDigits = new List<int>();
            foreach (char c in isin)
            {
                int d = CharToISINDigit(c);
                checkDigits.AddRange(ToSingleDigits(d));
            }
            if (checkDigits.Count > 0)
            {
                int quersumme = 0;
                int count = 1;
                for (int i = checkDigits.Count - 1; i >= 0; i--)
                {
                    count += 1;
                    if (count % 2 == 0)
                    {
                        checkDigits[i] *= 2;
                    }
                    int[] digits = ToSingleDigits(checkDigits[i]);
                    foreach (int d in digits)
                    {
                        quersumme += d;
                    }
                }
                return (10 - (quersumme % 10)) % 10;
            }
            return -1;
        }

        private static int[] ToSingleDigits(int i)
        {
            return i.ToString().ToCharArray().Select(c => Int32.Parse(c.ToString())).ToArray();
        }

        private static int CharToISINDigit(char c)
        {
            return Char.IsDigit(c) ? Int32.Parse(c.ToString()) : c - 'A' + 10;
        }
    }
}
