using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.Core;

namespace Toolkit.Configuration
{
    public interface IConfiguration
    {
        string getString(string key);
        string getString(string key, string def);
        double getDouble(string key);
        double getDouble(string key, double def);
        FixedPointDecimal getFixedPointDecimal(string key);
        FixedPointDecimal getFixedPointDecimal(string key, FixedPointDecimal def);
        TEnum getEnum<TEnum>(string key);
        TEnum getEnum<TEnum>(string key, TEnum def) where TEnum : struct;
        bool getBool(string key);
        bool getBool(string key, bool def);
        Time getTime(string key);
        Time getTime(string key, Time def);

        string getStringForInstrument(string key, InstrumentsConfiguration confInstr, string symbol, string def);
        string getStringForInstrument(string key, InstrumentsConfiguration confInstr, string symbol);
        double getDoubleForInstrument(string key, InstrumentsConfiguration confInstr, string symbol);
        double getDoubleForInstrument(string key, InstrumentsConfiguration confInstr, string symbol, double def);
        FixedPointDecimal getFixedPointDecimalForInstrument(string key, InstrumentsConfiguration confInstr, string symbol);
        FixedPointDecimal getFixedPointDecimalForInstrument(string key, InstrumentsConfiguration confInstr, string symbol, FixedPointDecimal def);

        IConfiguration SubSet(string path);

        IEnumerable<string> AllKeys { get; }
        IEnumerable<string> LocalKeys { get; }
    }
}
