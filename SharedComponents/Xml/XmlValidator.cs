using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace SharedComponents.Xml
{
    public class XmlValidator
    {
        /// <summary>
        /// Errors  holder.
        /// </summary>
        private static List<string> v_Errors = new List<string>();

        /// <summary>
        /// Warnings holder.
        /// </summary>
        private static List<string> v_Warnings = new List<string>();

        /// <summary>
        /// Checkes whether input XML is valid based on given XSD.
        /// </summary>
        /// <param name="xmlPath">Path of XML document.</param>
        /// <param name="validatorPath">Path of XSD validator.</param>
        /// <param name="errors">Output list with errors.</param>
        /// <param name="warnings">Output list with warnings.</param>
        /// <returns>
        /// True if XML document is valid otherwise false.
        /// </returns>
        public static bool Validate(string xmlPath, string validatorPath, out List<string> errors, out List<string> warnings)
        {
            v_Errors.Clear();
            v_Warnings.Clear();

            ValidateXml(xmlPath, validatorPath);

            errors = v_Errors;
            warnings = v_Warnings;

            return IsXmlValid();

        }

        /// <summary>
        /// Checkes whether input XML is valid based on given XSD.
        /// </summary>
        /// <param name="xmlPath">Path of XML document.</param>
        /// <param name="validatorPath">Path of XSD validator.</param>
        /// <param name="errorsWarnings">Sumarized errors/warnings output from validation.</param>
        /// <returns></returns>
        public static bool Validate(string xmlPath, string validatorPath, out string errorsWarnings)
        {
            v_Errors.Clear();
            v_Warnings.Clear();

            ValidateXml(xmlPath, validatorPath);
            errorsWarnings = CreateStringFromErrorsWarnings(xmlPath);

            return IsXmlValid();
        }

        /// <summary>
        /// Checkes whether input XML is valid based on given XSD.
        /// </summary>
        /// <param name="xmlDocument">XML document.</param>
        /// <param name="validatorPath">Path of XSD validator.</param>
        /// <param name="errorsWarnings">Sumarized errors/warnings output from validation.</param>
        /// <returns></returns>
        public static bool Validate(XDocument xmlDocument, string validatorPath, out string errorsWarnings)
        {
            v_Errors.Clear();
            v_Warnings.Clear();

            ValidateXml(xmlDocument, validatorPath);
            errorsWarnings = CreateStringFromErrorsWarnings(string.Empty);

            return IsXmlValid();
        }

        /// <summary>
        /// Validate XML against given XSD.
        /// </summary>
        /// <param name="xmlPath">Path of XML document.</param>
        /// <param name="validatorPath">Path of XSD validator.</param>
        private static void ValidateXml(string xmlPath, string validatorPath)
        {
            XmlReader MSystemDescription = XmlReader.Create(xmlPath, CreateXmlReaderSettings(validatorPath));

            while (MSystemDescription.Read()) { }
        }

        /// <summary>
        /// Validate XML against given XSD.
        /// </summary>
        /// <param name="xmlDocument">XML document.</param>
        /// <param name="validatorPath">Path of XSD validator.</param>
        private static void ValidateXml(XDocument xmlDocument, string validatorPath)
        {
            XmlReader MSystemDescription = XmlReader.Create(xmlDocument.CreateReader(), CreateXmlReaderSettings(validatorPath));

            while (MSystemDescription.Read()) { }
        }


        /// <summary>
        /// Adds errors/warnings to output lists.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event parameter.</param>
        private static void MSystemDescriptionSettingsValidationEventHandler(object sender, ValidationEventArgs e)
        {
            if (e.Severity == XmlSeverityType.Warning)
            {
                v_Warnings.Add(e.Message);
            }
            else if (e.Severity == XmlSeverityType.Error)
            {
                v_Errors.Add(e.Message);
            }
        }

        /// <summary>
        /// Creates result of validation
        /// </summary>
        /// <returns>
        /// True if no errors/warnings occurs otherwise false.
        /// </returns>
        private static bool IsXmlValid()
        {
            if (!v_Errors.Any() && !v_Warnings.Any())
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Sumarize errors/warnings found during validation.
        /// </summary>
        /// <param name="xmlPath">Path of validated XML.</param>
        /// <returns>
        /// Formated string of warnings and errors.
        /// </returns>
        private static string CreateStringFromErrorsWarnings(string xmlPath)
        {
            string fileName = !string.IsNullOrEmpty(xmlPath) ? Path.GetFileName(xmlPath) : "In memory XML";

            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"Error occures during XSD validation. Given XML ({fileName}) is not valid!");
            builder.AppendLine("Errors:");
            foreach (string error in v_Errors)
            {
                builder.AppendLine(error);
            }

            builder.AppendLine("Warnings:");
            foreach (string warning in v_Warnings)
            {
                builder.AppendLine(warning);
            }

            return builder.ToString();
        }

        private static XmlReaderSettings CreateXmlReaderSettings( string validatorPath)
        {
            XmlReaderSettings MSystemDescriptionSetting = new XmlReaderSettings();
            MSystemDescriptionSetting.Schemas.Add("", validatorPath);
            MSystemDescriptionSetting.ValidationType = ValidationType.Schema;
            MSystemDescriptionSetting.ValidationEventHandler += MSystemDescriptionSettingsValidationEventHandler;

            return MSystemDescriptionSetting;
        }
    }
}
