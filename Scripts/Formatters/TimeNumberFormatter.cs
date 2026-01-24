using System;

namespace ReadableNumbers.Formatters {
    public struct TimeNumberSuffix {
        public float Unit { get; private set; }
        public string Suffix { get; private set; }
        public string Name { get; private set; }

        public TimeNumberSuffix(float unit, string suffix, string name) {
            Unit = unit;
            Suffix = suffix;
            Name = name;
        }
    }

    public class TimeNumberFormatter : INumberFormatter {
        public readonly TimeNumberSuffix[] Suffixes =
        {
            new TimeNumberSuffix(0.001f,  "ms", "Millisecond"),
            new TimeNumberSuffix(1f,      "s",  "Second"),
            new TimeNumberSuffix(60f,     "min","Minute"),
            new TimeNumberSuffix(3600f,   "h",  "Hour"),
            new TimeNumberSuffix(86400f,  "d",  "Day"),
            new TimeNumberSuffix(604800f, "wk", "Week")
        };

        public string DisplayNumber(float number, DisplayType displayType, string format = null) {
            if(number <= 0f)
                return displayType == DisplayType.Name ? "0 Milliseconds" :
                       displayType == DisplayType.Suffix ? "0ms" :
                       "0";

            int index = 0;

            for(int i = Suffixes.Length - 1; i >= 0; i--) {
                if(number >= Suffixes[i].Unit) {
                    index = i;
                    break;
                }
            }

            float value = number / Suffixes[index].Unit;
            TimeNumberSuffix unit = Suffixes[index];

            string numberFormat = string.IsNullOrEmpty(format) ? "0.##" : format;
            string numberText = value.ToString(numberFormat);

            switch(displayType) {
                case DisplayType.None:
                    return numberText;

                case DisplayType.Suffix:
                    return numberText + unit.Suffix;

                case DisplayType.Name:
                    string name = value == 1f ? unit.Name : unit.Name + "s";
                    return numberText + " " + name;

                default:
                    return numberText;
            }
        }

        public string DisplayNumber(int number, DisplayType displayType, string format = null) {
            return DisplayNumber((float)number, displayType, format);
        }
    }
}