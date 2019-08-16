using System.Text.RegularExpressions;

namespace SharedComponents.Tools
{
    /// <summary>
    /// Regexp checks.
    /// </summary>
    public class Regexp
    {
        #region Public data

        /// <summary>
        /// Type of check.
        /// </summary>
        public enum Check
        { StringWithApostroph, String, StringOrEmptyString, Number, NumberOrEmptyString, NumberWithoutZero, FloatingNumber, FloatingNumberOrEmptyString, Skip }

        #endregion

        #region Public methods

        /// <summary>
        /// Controls input string with Regex.
        /// </summary>
        /// <param name="input">Input value.</param>
        /// <param name="type">
        /// Types:
        /// - 'StringWithApostroph'
        /// - 'String'
        /// - 'Numbers'
        /// - 'FloatingNumbers'
        /// - 'NumbersWithOutZero'
        /// </param>
        /// <returns>
        /// If valid returns true, otherwise false.
        /// </returns>
        public static bool CheckInputText(string input, Check type)
        {
            switch (type)
            {
                case Check.StringWithApostroph:
                    return Regex.IsMatch(input, @"^[a-zA-Z0-9\']*$");
                case Check.String:
                    return Regex.IsMatch(input, @"^[a-zA-Z0-9,]*$");
                case Check.StringOrEmptyString:
                    return Regex.IsMatch(input, @"(^$)|(^[a-zA-Z0-9,]*$)");
                case Check.Number:
                    return Regex.IsMatch(input, @"^-?[0-9]*$");
                case Check.NumberWithoutZero:
                    return Regex.IsMatch(input, @"^[1-9]*([0-9])*$");
                case Check.NumberOrEmptyString:
                    return Regex.IsMatch(input, @"(^$)|(^-?[0-9]*$)");
                case Check.FloatingNumber:
                    return Regex.IsMatch(input, @"^[-+]?[0-9]*(?:\.[0-9]+)?$");
                case Check.FloatingNumberOrEmptyString:
                    return Regex.IsMatch(input, @"(^$)|(^[-+]?[0-9]*(?:\.[0-9]+)?$)");
                case Check.Skip:
                    return true;
                default:
                    return false;
            }
        }

        #endregion
    }
}
