using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharedComponents.Tools;

namespace Cytos_v2_Tests.Classes.Tools
{
    [TestClass]
    public class RegexpTest
    {
        [TestMethod]
        public void RegexpTestsForDifferentInputs()
        {
            Assert.IsTrue(Regexp.CheckInputText(string.Empty, Regexp.Check.NumberOrEmptyString));
            Assert.IsTrue(Regexp.CheckInputText("123", Regexp.Check.NumberOrEmptyString));
            Assert.IsFalse(Regexp.CheckInputText("1 23", Regexp.Check.NumberOrEmptyString));

            Assert.IsTrue(Regexp.CheckInputText(string.Empty, Regexp.Check.FloatingNumberOrEmptyString));
            Assert.IsTrue(Regexp.CheckInputText("123.5", Regexp.Check.FloatingNumberOrEmptyString));
            Assert.IsFalse(Regexp.CheckInputText("1 23.5", Regexp.Check.FloatingNumberOrEmptyString));

            Assert.IsTrue(Regexp.CheckInputText(string.Empty, Regexp.Check.StringOrEmptyString));
            Assert.IsTrue(Regexp.CheckInputText("test", Regexp.Check.StringOrEmptyString));
            Assert.IsFalse(Regexp.CheckInputText("test test", Regexp.Check.StringOrEmptyString));
        }
    }
}
