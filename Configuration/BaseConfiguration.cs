using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.Core;

namespace Toolkit.Configuration
{
    public class BaseConfiguration : IConfiguration
    {
        private Dictionary<string, string> valuesByKey;
        private List<string> orderedKeys;

        protected BaseConfiguration(IEnumerable<string[]> data)
        {
            this.orderedKeys = data.Select(item => item[0]).ToList();
            this.valuesByKey = data.ToDictionary(item => item[0], item => item[1]);
        }

        public IConfiguration SubSet(string path)
        {
            return new SubSetConfiguration(this, path);
        }

        public IEnumerable<string> AllKeys
        {
            get { return orderedKeys; }
        }

        public IEnumerable<string> LocalKeys
        {
            get { return AllKeys.Select(key => key.Contains(".") ? key.Substring(0, key.IndexOf(".")) : key).Distinct(); }
        }

        public bool HasKey(string key)
        {
            return valuesByKey.ContainsKey(key);
        }

        public string getString(string key)
        {
            return valuesByKey[key];
        }

        public string getString(string key, string def)
        {
            string result = null;
            if (!valuesByKey.TryGetValue(key, out result))
                return def;
            return result;
        }

        public TEnum getEnum<TEnum>(string key)
        {
            return (TEnum)Enum.Parse(typeof(TEnum), valuesByKey[key]);
        }

        public TEnum getEnum<TEnum>(string key, TEnum def) where TEnum : struct
        {
            string resultStr = null;
            if (!valuesByKey.TryGetValue(key, out resultStr))
                return def;
            TEnum result;
            if (!Enum.TryParse(resultStr, out result))
                return def;
            return result;
        }

        public Time getTime(string key)
        {
            return Time.fromSeconds(Double.Parse(valuesByKey[key]));
        }

        public Time getTime(string key, Time def)
        {
            string resultStr = null;
            if (!valuesByKey.TryGetValue(key, out resultStr))
                return def;
            double result;
            if (!Double.TryParse(resultStr, out result))
                return def;
            return Time.fromSeconds(result);
        }

        public bool getBool(string key)
        {
            return Boolean.Parse(valuesByKey[key].ToLowerInvariant());
        }

        public bool getBool(string key, bool def)
        {
            string resultStr = null;
            if (!valuesByKey.TryGetValue(key, out resultStr))
                return def;
            bool result;
            if (!Boolean.TryParse(resultStr.ToLowerInvariant(), out result))
                return def;
            return result;
        }

        public string getStringForInstrument(string key, InstrumentsConfiguration confInstr, string symbol, string def)
        {
            try
            {
                return getStringForInstrument(key, confInstr, symbol);
            }
            catch (KeyNotFoundException)
            {
                return def;
            }
        }

        public string getStringForInstrument(string key, InstrumentsConfiguration confInstr, string symbol)
        {
            // blows if the key is not in the main cfg
            string mainConfValue = getString(key);

            // the value from main cfg is not indexing the InstrumentsConfiguration, so let's return it as is
            if (!confInstr.HasKey(mainConfValue))
                return mainConfValue;

            // blows if it cannot find the symbol
            InstrumentConfiguration instr = confInstr.getInstrument(symbol);
            return instr.getString(mainConfValue);
        }

        public double getDouble(string key)
        {
            return Double.Parse(getString(key));
        }

        public double getDouble(string key, double def)
        {
            string resultStr = null;
            if (!valuesByKey.TryGetValue(key, out resultStr))
                return def;
            double result;
            if (!Double.TryParse(resultStr, out result))
                return def;
            return result;
        }

        public double getDoubleForInstrument(string key, InstrumentsConfiguration confInstr, string symbol)
        {
            return Double.Parse(getStringForInstrument(key, confInstr, symbol));
        }

        public double getDoubleForInstrument(string key, InstrumentsConfiguration confInstr, string symbol, double def)
        {
            try
            {
                return getDoubleForInstrument(key, confInstr, symbol);
            }
            catch (Exception ex)
            {
                if (ex is FormatException || ex is OverflowException || ex is KeyNotFoundException)
                    return def;
                else
                    throw;
            }
        }

        public FixedPointDecimal getFixedPointDecimal(string key)
        {
            return FixedPointDecimal.Parse(getString(key));
        }

        public FixedPointDecimal getFixedPointDecimal(string key, FixedPointDecimal def)
        {
            string resultStr = null;
            if (!valuesByKey.TryGetValue(key, out resultStr))
                return def;
            FixedPointDecimal result;
            if (!FixedPointDecimal.TryParse(resultStr, out result))
                return def;
            return result;
        }

        public FixedPointDecimal getFixedPointDecimalForInstrument(string key, InstrumentsConfiguration confInstr, string symbol)
        {
            return FixedPointDecimal.Parse(getStringForInstrument(key, confInstr, symbol));
        }

        public FixedPointDecimal getFixedPointDecimalForInstrument(string key, InstrumentsConfiguration confInstr, string symbol, FixedPointDecimal def)
        {
            try
            {
                return getFixedPointDecimalForInstrument(key, confInstr, symbol);
            }
            catch (Exception ex)
            {
                if (ex is FormatException || ex is OverflowException || ex is KeyNotFoundException)
                    return def;
                else
                    throw;
            }
        }
    }
}
