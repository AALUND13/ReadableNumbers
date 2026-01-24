using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using UnityEngine;

namespace ReadableNumbers.Formatters {
    public struct NormalNumberSuffix {
        public string Suffix { get; private set; }
        public string Name { get; private set; }

        public NormalNumberSuffix(string suffix, string name) {
            Suffix = suffix;
            Name = name;
        }
    }

    public class NormalNumberFormatter : INumberFormatter {
        public readonly NormalNumberSuffix[] suffixes = new NormalNumberSuffix[] {
            new NormalNumberSuffix("K", "Thousand"),
            new NormalNumberSuffix("M", "Million"),
            new NormalNumberSuffix("B", "Billion"),
            new NormalNumberSuffix("T", "Trillion"),
            new NormalNumberSuffix("Qa", "Quadrillion"),
            new NormalNumberSuffix("Qi", "Quintillion"),
            new NormalNumberSuffix("Sx", "Sextillion"),
            new NormalNumberSuffix("Sp", "Septillion"),
            new NormalNumberSuffix("Oc", "Octillion"),
            new NormalNumberSuffix("No", "Nonillion"),
            new NormalNumberSuffix("Dc", "Decillion"),
            new NormalNumberSuffix("Ud", "Undecillion")
        };

        public string DisplayNumber(float number, DisplayType displayType, string format = null) {
            bool isNegative = number < 0f;
            float absNumber = Mathf.Abs(number);

            if(absNumber == 0f)
                return format != null ? number.ToString(format) : number.ToString();

            int powerOf1000 = Mathf.FloorToInt(Mathf.Log10(absNumber) / 3);
            float scaled = absNumber / Mathf.Pow(10, powerOf1000 * 3);

            int decimals = 1 + Mathf.FloorToInt(Mathf.Log10(scaled));
            int roundingFactor = (int)Mathf.Pow(10, 4 - decimals);
            scaled = Mathf.Floor(scaled * roundingFactor) / roundingFactor;

            string scaledString = format != null
                ? scaled.ToString(format)
                : scaled.ToString();

            string numberString = format != null
                ? absNumber.ToString(format)
                : absNumber.ToString();

            if(isNegative) {
                scaledString = "-" + scaledString;
                numberString = "-" + numberString;
            }

            if(powerOf1000 <= 0) {
                return number.ToString($"0.{new string('#', Mathf.Max(0, 4 - decimals))}",
                    CultureInfo.InvariantCulture);
            }

            switch(displayType) {
                case DisplayType.None:
                    return number.ToString($"0.##", CultureInfo.InvariantCulture);
                case DisplayType.Name:
                    return $"{scaledString} {suffixes[powerOf1000 - 1].Name}";
                default:
                    return $"{scaledString}{suffixes[powerOf1000 - 1].Suffix}";
            }
        }
        public string DisplayNumber(int number, DisplayType displayType, string format = null) {
            return DisplayNumber((float)number, displayType);
        }
    }
}
