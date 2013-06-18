using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolkit.Indicators
{
    public interface IIsValid
    {
        bool IsValid { get; }
    }

    public interface IResetable
    {
        void Reset();
    }

    public interface IPausable
    {
        bool Pause();
    }
}
