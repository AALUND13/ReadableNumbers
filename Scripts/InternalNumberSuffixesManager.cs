using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ReadableNumbers.Scripts {
    public enum DisplayType {
        None,
        Name,
        Suffix
    }

    internal static class InternalNumberSuffixesManager {
        public static List<NumberSuffix> suffixes = new List<NumberSuffix>() {
            new NumberSuffix("K", "Thousand"),
            new NumberSuffix("M", "Million"),
            new NumberSuffix("B", "Billion"),
            new NumberSuffix("T", "Trillion"),
            new NumberSuffix("Qa", "Quadrillion"),
            new NumberSuffix("Qi", "Quintillion"),
            new NumberSuffix("Sx", "Sextillion"),
            new NumberSuffix("Sp", "Septillion"),
            new NumberSuffix("Oc", "Octillion"),
            new NumberSuffix("No", "Nonillion"),
            new NumberSuffix("Dc", "Decillion"),
            new NumberSuffix("Ud", "Undecillion")
        };

        public static string DisplayNumber(float number, DisplayType displayType, string format = null) {
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
                float value = isNegative ? -absNumber : absNumber;
                return (isNegative ? "-" : "") + (value.ToString($"0.{new string('#', Mathf.Max(0, 4 - decimals))}",CultureInfo.InvariantCulture));
            }

            switch(displayType) {
                case DisplayType.None:
                    return numberString;
                case DisplayType.Name:
                    return $"{scaledString} {suffixes[powerOf1000 - 1].Name}";
                default:
                    return $"{scaledString}{suffixes[powerOf1000 - 1].Suffix}";
            }
        }


        public static string DisplayNumber(int number, DisplayType displayType, string format = null) { 
            return DisplayNumber((float)number, displayType);
        }
    }
}
