using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using SharedComponents.Tools;

namespace MSystemCreator.Classes
{
    /// <summary>
    /// Serialize new Evolution objects to Xml structure.
    /// </summary>
    [Obsolete("User SerializeMSystem instead", true)]
    class SerializeEvolutionObjects
    {
        #region Private Data

        /// <summary>
        /// Xml document containing Evolution file.
        /// </summary>
        private static XmlDocument v_EvolutionXmlDocument;

        /// <summary>
        /// Holds the type of the error message.
        /// </summary>
        private enum ErrorMessages
        { IsNullOrEmpty, IncorectCharacters, IncorectCharactersWithApostroph, EmptyList }

        #endregion

        #region Private methods

        /// <summary>
        /// Creates Exception Message.
        /// </summary>
        /// <param name="type">
        /// Types:
        /// - 'IsNullOrEmpty'
        /// - 'IncorectCharacters'
        /// - 'IncorectCharactersWithApostroph'
        /// - 'IncorectNumber'
        /// - 'IncorectFloatingNumber'
        /// - 'EmptyList'  
        /// </param>
        /// <param name="exceptionName">Name of exception.</param>
        /// <returns>
        /// Exception message.
        /// </returns>
        private string ExceptionsMessage(string exceptionName, int type)
        {
            switch (type)
            {
                case 0:
                    return string.Format("Parametr {0} can't be null or empty string.", exceptionName);
                case 1:
                    return string.Format("Incorect input character/s in {0}\nAllowed characters are a-z, A-Z, 0-9", exceptionName);
                case 2:
                    return string.Format("Incorect input character/s in {0}\nAllowed characters are a-z, A-Z, 0-9, '", exceptionName);
                case 3:
                    return string.Format("Parametr {0} can't be null or empty list.", exceptionName);
                default:
                    return "Incorect input.";
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Gets Xml document as a string.
        /// </summary>
        /// <returns></returns>
        public string GetXmlDocAsAString()
        {
            XDocument evolutionFileDocument = XDocument.Parse(v_EvolutionXmlDocument.OuterXml);
            return evolutionFileDocument.ToString();
        }
        /// <summary>
        /// Saves XML document as XML file.
        /// </summary>
        /// <param name="path">String location where you want to save Evolution file.</param>
        public void SaveXmlFile(string path)
        {
            v_EvolutionXmlDocument.Save(path);
        }

        /// <summary>
        /// Creates new Xml document.
        /// </summary>
        public void InitializeEvolutionXmlDocument()
        {
            v_EvolutionXmlDocument = new XmlDocument();
            // Xml default Structure.
            // Master evolution.
            XmlElement evolution = (XmlElement)v_EvolutionXmlDocument.AppendChild(v_EvolutionXmlDocument.CreateElement("evolution"));
            // Child glueRelations.
            evolution.AppendChild(v_EvolutionXmlDocument.CreateElement("glueRelations"));
            // Child evoRulesWithPriority.
            evolution.AppendChild(v_EvolutionXmlDocument.CreateElement("evoRulesWithPriority"));
        }

        /// <summary>
        /// Creates new glueTuple element.
        /// </summary>
        /// <param name="protein1">Name of the protein1.</param>
        /// <param name="protein2">Name of the protein2.</param>
        /// <param name="signalM">Signal M set string.</param>
        /// <param name="errorMessage">
        /// Used as error output if exception is caught.
        /// </param>
        /// <returns>
        /// True if serialization was succesfull otherwise returns false.
        /// </returns>
        public bool GlueRelation(string protein1, string protein2, string signalM, out string errorMessage)
        {
            try
            {
                if (string.IsNullOrEmpty(protein1))
                {
                    throw new ArgumentException(ExceptionsMessage("protein1", (int)ErrorMessages.IsNullOrEmpty));
                }
                if (!Regexp.CheckInputText(protein1, Regexp.Check.StringWithApostroph))
                {
                    throw new ArgumentException(ExceptionsMessage("protein1", (int)ErrorMessages.IncorectCharactersWithApostroph));
                }
                if (string.IsNullOrEmpty(protein2))
                {
                    throw new ArgumentException(ExceptionsMessage("protein2", (int)ErrorMessages.IsNullOrEmpty));
                }
                if (!Regexp.CheckInputText(protein2, Regexp.Check.StringWithApostroph))
                {
                    throw new ArgumentException(ExceptionsMessage("protein2", (int)ErrorMessages.IncorectCharactersWithApostroph));
                }
                if (!Regexp.CheckInputText(signalM, Regexp.Check.String))
                {
                    throw new ArgumentException(ExceptionsMessage("protein1", (int)ErrorMessages.IncorectCharacters));
                }

                // <glueTuple protein1="p0" protein2="p2" signalMset=""/>
                XmlElement glueTuple = v_EvolutionXmlDocument.CreateElement("glueTuple");
                glueTuple.SetAttribute("signalMset", signalM);
                glueTuple.SetAttribute("protein2", protein2);
                glueTuple.SetAttribute("protein1", protein1);

                XmlNode glueRelations = v_EvolutionXmlDocument.SelectSingleNode("evolution/glueRelations");
                glueRelations?.AppendChild(glueTuple);

                errorMessage = null;
                return true;
            }
            catch (Exception exception)
            {
                errorMessage = exception.Message;
                return false;
            }
        }

        /// <summary>
        /// Creates new evoRule element within evoRulesWithPriority block.
        /// </summary>
        /// <remarks>
        /// Catches all exceptions and returned as errorMessage. 
        /// </remarks>
        /// <param name="type">EvoRule type can be metabolic, divide, center or destroy.</param>
        /// <param name="priority">EvoRule priority.</param>
        /// <param name="leftSideList">Container leftSide object.</param>
        /// <param name="rightSideList">Container rightSide object.</param>
        /// <param name="errorMessage">
        /// Output parameter, which holds error message catched during serialization.
        /// </param>
        /// <returns>
        /// True if serialization was succesfull otherwise returns false.
        /// </returns>
        public bool EvoRule(string type, int priority, List<string> leftSideList, List<string> rightSideList, out string errorMessage)
        {
            try
            {
                if (string.IsNullOrEmpty(type))
                {
                    throw new ArgumentException(ExceptionsMessage("type", (int)ErrorMessages.IsNullOrEmpty));
                }
                if (!Regexp.CheckInputText(type, Regexp.Check.String))
                {
                    throw new ArgumentException(ExceptionsMessage("type", (int)ErrorMessages.IncorectCharacters));
                }
                if (leftSideList == null || leftSideList.Count == 0)
                {
                    throw new ArgumentException(ExceptionsMessage("leftSideList", (int)ErrorMessages.EmptyList));
                }
                if (rightSideList == null || rightSideList.Count == 0)
                {
                    throw new ArgumentException(ExceptionsMessage("rightSideList", (int)ErrorMessages.EmptyList));
                }

                /*<evoRule type="2res" priority="0">
                    <leftside >
                        <object name="a"/>
                    </leftside>
                    <rightside>
                        <object name="p1"/>
                    </rightside>
                  </evoRule>*/
                XmlElement evoRule = v_EvolutionXmlDocument.CreateElement("evoRule");
                evoRule.SetAttribute("priority", priority.ToString());
                evoRule.SetAttribute("type", type);

                XmlElement leftsideElement = (XmlElement)evoRule.AppendChild(v_EvolutionXmlDocument.CreateElement("leftside"));
                foreach (string leftSide in leftSideList)
                {
                    XmlElement leftobject = (XmlElement)leftsideElement.AppendChild(v_EvolutionXmlDocument.CreateElement("object"));
                    leftobject.SetAttribute("name", leftSide);
                }

                XmlElement rightsideElement = (XmlElement)evoRule.AppendChild(v_EvolutionXmlDocument.CreateElement("rightside"));
                foreach (string rightSide in rightSideList)
                {
                    XmlElement rightobject = (XmlElement)rightsideElement.AppendChild(v_EvolutionXmlDocument.CreateElement("object"));
                    rightobject.SetAttribute("name", rightSide);
                }

                XmlNode evoRulesWithPriority = v_EvolutionXmlDocument.SelectSingleNode("evolution/evoRulesWithPriority");
                evoRulesWithPriority?.AppendChild(evoRule);

                errorMessage = null;
                return true;
            }
            catch (Exception exception)
            {
                errorMessage = exception.Message;
                return false;
            }
        }

        #endregion
    }
}
