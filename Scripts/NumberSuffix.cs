using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadableNumbers {
    public struct NumberSuffix {
        public string Suffix { get; private set; }
        public string Name { get; private set; }

        public NumberSuffix(string suffix, string name) {
            Suffix = suffix;
            Name = name;
        }
    }
}
