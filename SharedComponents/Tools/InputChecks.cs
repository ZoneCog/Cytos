using System;
using System.Windows.Forms;

namespace SharedComponents.Tools
{
    public static class InputChecks
    {
        /// <summary>
        /// Checks whether input value is correct.
        /// </summary>
        /// <param name="value">Input value.</param>
        /// <param name="type">Check type.</param>
        /// <returns>True if valid, otherwise false.</returns>
        public static bool CheckValue(string value, Regexp.Check type)
        {
            if (!Regexp.CheckInputText(value, type))
            {
                MessageBox.Show(string.Format("{0} expected instead of '{1}'", ConvertCheckTypeToString(type), value));
                return false;
            }
            return true;
        }

        /// <summary>
        /// Converts regexp type to string.
        /// </summary>
        /// <param name="type">Type of regexp check.</param>
        /// <returns>Converted type</returns>
        /// <exception cref="InvalidOperationException">If unknown type appears.</exception>
        private static string ConvertCheckTypeToString(Regexp.Check type)
        {
            switch (type)
            {
                case Regexp.Check.StringWithApostroph:
                    return "String with apostroph";
                case Regexp.Check.String:
                    return "String";
                case Regexp.Check.StringOrEmptyString:
                    return "String or empty string";
                case Regexp.Check.Number:
                    return "Number";
                case Regexp.Check.NumberWithoutZero:
                    return "Number without zero";
                case Regexp.Check.NumberOrEmptyString:
                    return "Number or empty string";
                case Regexp.Check.FloatingNumber:
                    return "Floating number";
                case Regexp.Check.FloatingNumberOrEmptyString:
                    return "Floating number or empty string";
                default:
                    throw new InvalidOperationException("Unknown check type.");
            }
        }
    }
}
