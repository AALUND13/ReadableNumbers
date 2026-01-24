using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ReadableNumbers {
    public class NumberDisplayController : MonoBehaviour {
        private static DisplayType currentDisplayType = DisplayType.Suffix;

        public static bool isDisable { get; private set; } = false;
        public static DisplayType CurrentDisplayType { get { return !isDisable ? currentDisplayType : DisplayType.None; } }

        public void Update() {
            if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.I)) {
                CycleDisplayType();
            }

            if(Input.GetKeyDown(KeyCode.RightControl)) {
                isDisable = true;
            } else if(Input.GetKeyUp(KeyCode.RightControl)) {
                isDisable = false;
            }
        }

        void CycleDisplayType() {
            switch(currentDisplayType) {
                case DisplayType.Suffix:
                    currentDisplayType = DisplayType.Name; 
                    break;
                case DisplayType.Name:
                    currentDisplayType = DisplayType.Suffix;
                    break;
                default:
                    currentDisplayType = DisplayType.Suffix; 
                    break;
            }
        }

        // Normal
        public static string DisplayNumber(float number, string format = null) {
            return NumberFormatters.NormalNumberFormatter.DisplayNumber(number, CurrentDisplayType, format);
        }

        public static string DisplayNumber(int number, string format = null) {
            return NumberFormatters.NormalNumberFormatter.DisplayNumber(number, CurrentDisplayType, format);
        }

        // Time
        public static string DisplayTime(float time, string format = null) {
            return NumberFormatters.TimeNumberFormatter.DisplayNumber(time, CurrentDisplayType, format);
        }

        public static string DisplayTime(int seconds, string format = null) {
            return NumberFormatters.TimeNumberFormatter.DisplayNumber(seconds, CurrentDisplayType, format);
        }
    }
}
