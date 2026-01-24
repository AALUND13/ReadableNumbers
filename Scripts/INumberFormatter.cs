using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadableNumbers {
    public enum DisplayType {
        None,
        Name,
        Suffix
    }

    public interface INumberFormatter {
        string DisplayNumber(float number, DisplayType displayType, string format = null);
        string DisplayNumber(int number, DisplayType displayType, string format = null);
    }
}
