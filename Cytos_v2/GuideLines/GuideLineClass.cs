using System;

//Don't forget to remove unused references.

namespace Cytos_v2.GuideLines
{
    /// <summary>
    /// Each class has to have its summary, what the class do and what is its purpose.
    /// </summary>
    class GuideLineClass
    {
        // Regions are used to circumscribe variables/class with the same access (private, public, protected, etc.)
        #region Public variables 

        /// <summary>
        /// Also each variables has to have its summary with the same rules as class.
        /// </summary>
        public int SomeNumber;

        #endregion

        #region Private variables

        /// <summary>
        /// Describes what the variable is used for.
        /// </summary>
        private string v_SomeString;

        /// <summary>
        /// Same here
        /// </summary>
        private const char c_SomeChar = 'c';

        #endregion

        #region Public methods

        /// <summary>
        /// Example of public method with correct descriptio, what it does.
        /// </summary>
        /// <param name="someInput">Describe all input parameters, what it is used for.</param>
        /// <returns>And, of course, describe what it returns.</returns>
        /// <exception cref="InvalidOperationException">
        /// If method can throw some exception, describe WHEN it is thown. Exaple see below.
        /// If someInput is null or empty string.
        /// </exception>
        public string SomePublicMethod(string someInput)
        {
            if (string.IsNullOrEmpty(someInput))
            {
                throw new InvalidOperationException("It's necessary to check if the input is in correct form, otherwise throw exception.");
            }
            return v_SomeString;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Method is used for class variable initialization.
        /// </summary>
        /// <param name="someIntInput">String contains some value to be used for something.</param>
        /// <param name="someStringInput">Integere contains some value to be used for something.</param>
        /// <exception cref="InvalidOperationException">
        /// If someIntInput has default value, which is not allowed.
        /// </exception>
        private void SomePrivateMethodWithMoreInputs(int someIntInput, string someStringInput)
        {
            if (someIntInput == 0)
            {
                throw new InvalidOperationException("Input variable someIntInput can't be null.");
            }
            SomeNumber = someIntInput;
            v_SomeString = someStringInput;
        }

        #endregion

    }
}
