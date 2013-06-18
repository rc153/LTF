using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.Core;

namespace Toolkit.Configuration
{
    class SubSetConfiguration : IConfiguration
    {
        private IConfiguration parent;
        private string path;

        public SubSetConfiguration(IConfiguration parent, string path)
        {
            this.parent = parent;
            this.path = path;
            if (!this.path.EndsWith(".")) this.path += ".";
        }

        public IConfiguration SubSet(string path)
        {
            return new SubSetConfiguration(parent, this.path + path);
        }

        public string getString(string key)
        {
            return parent.getString(path + key);
        }

        public string getString(string key, string def)
        {
            return parent.getString(path + key, def);
        }

        public double getDouble(string key)
        {
            return parent.getDouble(path + key);
        }

        public double getDouble(string key, double def)
        {
            return parent.getDouble(path + key, def);
        }

        public Time getTime(string key)
        {
            return parent.getTime(path + key);
        }

        public Time getTime(string key, Time def)
        {
            return parent.getTime(path + key, def);
        }

        public FixedPointDecimal getFixedPointDecimal(string key)
        {
            return parent.getFixedPointDecimal(path + key);
        }

        public FixedPointDecimal getFixedPointDecimal(string key, FixedPointDecimal def)
        {
            return parent.getFixedPointDecimal(path + key, def);
        }

        public TEnum getEnum<TEnum>(string key)
        {
            return parent.getEnum<TEnum>(path + key);
        }

        public TEnum getEnum<TEnum>(string key, TEnum def) where TEnum : struct
        {
            return parent.getEnum(path + key, def);
        }

        public bool getBool(string key)
        {
            return parent.getBool(path + key);
        }

        public bool getBool(string key, bool def)
        {
            return parent.getBool(path + key, def);
        }

        public IEnumerable<string> AllKeys
        {
            get { return parent.AllKeys.Where(key => key.StartsWith(path)).Select(key => key.Substring(path.Length)); }
        }

        public IEnumerable<string> LocalKeys
        {
            get { return AllKeys.Select(key => key.Contains(".") ? key.Substring(0, key.IndexOf(".")) : key).Distinct(); }
        }

        public string getStringForInstrument(string key, InstrumentsConfiguration confInstr, string symbol, string def)
        {
            return parent.getStringForInstrument(path + key, confInstr, symbol, def);
        }

        public string getStringForInstrument(string key, InstrumentsConfiguration confInstr, string symbol)
        {
            return parent.getStringForInstrument(path + key, confInstr, symbol);
        }

        public double getDoubleForInstrument(string key, InstrumentsConfiguration confInstr, string symbol)
        {
            return parent.getDoubleForInstrument(path + key, confInstr, symbol);
        }

        public double getDoubleForInstrument(string key, InstrumentsConfiguration confInstr, string symbol, double def)
        {
            return parent.getDoubleForInstrument(path + key, confInstr, symbol, def);
        }

        public FixedPointDecimal getFixedPointDecimalForInstrument(string key, InstrumentsConfiguration confInstr, string symbol)
        {
            return parent.getFixedPointDecimalForInstrument(path + key, confInstr, symbol);
        }

        public FixedPointDecimal getFixedPointDecimalForInstrument(string key, InstrumentsConfiguration confInstr, string symbol, FixedPointDecimal def)
        {
            return parent.getFixedPointDecimalForInstrument(path + key, confInstr, symbol, def);
        }
    }
}
