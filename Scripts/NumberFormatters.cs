using ReadableNumbers.Formatters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ReadableNumbers {
    public static class NumberFormatters {
        public static readonly TimeNumberFormatter TimeNumberFormatter = new TimeNumberFormatter();
        public static readonly NormalNumberFormatter NormalNumberFormatter = new NormalNumberFormatter();
    }
}
