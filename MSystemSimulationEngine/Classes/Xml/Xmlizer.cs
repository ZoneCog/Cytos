using System;
using System.Xml;
using System.Xml.Linq;
using Cytos_v2.Exceptions;
using MSystemSimulationEngine.Interfaces;

namespace MSystemSimulationEngine.Classes.Xml
{
    /// <summary>
    /// Provides a centralized place for doing XML operations.
    /// </summary>
    public static class Xmlizer
    {
        #region Public Methods

        /// <summary>
        /// Deserialize input XML file to simulation objects.
        /// </summary>
        /// <remarks>
        /// MissingXmlElement and MissingXmlAttribute are catched and returned as errorMessage. 
        /// </remarks>
        /// <param name="filePath">Path to file, which is going to be deserialized.</param>
        /// <param name="errorMessage">If derialization fails, contains error message,
        /// otherwise it is null.
        /// </param>
        /// <returns>If deserialization is successfull, returns list of deserialized objects,
        /// otherwise return null.
        /// </returns>
        /// <exception cref="ArgumentException">If filePath is null or empty string.</exception>
        public static IDeserializedObjects Deserialize(string filePath, out string errorMessage)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    throw new ArgumentException("File path can't be null or empty string.");
                }
                XDocument doc = XDocument.Load(filePath);

                //Deserialize Inventory File
                if (doc.Element("root") != null)
                {
                    IDeserializedObjects deserializedObjects = new DeserializedObjects(doc, filePath);
                    errorMessage = null;
                    return deserializedObjects;
                }
                throw new InvalidOperationException("Invalid input XML file.");

            }
            catch (MissingXmlElement xmlException)
            {
                errorMessage = string.Format("Exception during deserialization occured.\n --> {0}", xmlException.Message);
                return null;
            }
            catch (MissingXmlAttribute xmlException)
            {
                errorMessage = string.Format("Exception during deserialization occured.\n --> {0}", xmlException.Message);
                return null;
            }
            catch (Exception ex)
            {
                errorMessage = string.Format("Exception during deserialization occured.\n --> {0}", ex.Message);
                return null;
            }
        }

        #region Snapshots file serialization

        /// <summary>
        /// Adds a new snapshot to the snapshots XML document
        /// </summary>
        /// <param name="snapshotXmlDoc">Target snapshots XML document</param>
        /// <param name="stepID">Snapshot number.</param>
        /// <param name="floatingObjects">Floating objects to be serialized.</param>
        /// <param name="tilesWorld">Tiles to be serialized.</param>
        public static void AddSnapshot(ulong stepID, FloatingObjectsSet floatingObjects, TilesWorld tilesWorld, SerializeSnapshot snapshotXmlDoc)
        {
            if (floatingObjects == null)
            {
                throw new ArgumentNullException("Parameter 'floatingObjects' may not be null.");
            }
            if (tilesWorld == null)
            {
                throw new ArgumentNullException("Parameter 'tilesWorld' may not be null.");
            }
            if (snapshotXmlDoc == null)
            {
                throw new ArgumentNullException("Parameter 'snapshotXmlDoc' may not be null.");
            }
            snapshotXmlDoc.Serialize(stepID, floatingObjects, tilesWorld);
        }

        /// <summary>
        /// Saves XML document as XML file.
        /// </summary>
        /// <returns>Path to the saved Snapshot</returns>
        public static string SaveSnapshotsDoc(SerializeSnapshot snapshotXmlDoc, bool toTempFile = true)
        {
            if (snapshotXmlDoc == null)
            {
                throw new ArgumentNullException("Parameter 'snapshotXmlDoc' may not be null.");
            }

            string snapShotFilePath;
            if (toTempFile)
            {
                snapShotFilePath = "./snapshots/TemporarySnapshot.xml";
            }
            else
            {
                snapShotFilePath = string.Format("./snapshots/SnapshotFile-{0}.xml", DateTime.Now.ToString("yyyyMMdd-HHmmssffff"));
            }
            snapshotXmlDoc.SaveXmlFile(snapShotFilePath);
            return snapShotFilePath;
        }

        /// <summary>
        /// Saves XML document as XML file to specified location.
        /// </summary>
        /// <param name="snapshotXmlDoc">XML snapshot document.</param>
        /// <param name="path">Save path.</param>
        public static void SaveSnapshotsDocWithSpecificLocation(SerializeSnapshot snapshotXmlDoc, string path)
        {
            if (snapshotXmlDoc == null)
            {
                throw new ArgumentNullException("Parameter 'snapshotXmlDoc' may not be null.");
            }
            snapshotXmlDoc.SaveXmlFile(path);
        }

        #endregion


        //TODO move to shared components.
        #region Getters for element/attribute values

        /// <summary>
        /// Gets selected element value.
        /// </summary>
        /// <param name="element">Parent element.</param>
        /// <param name="parentElementName">Parent element name.</param>
        /// <param name="elementName">Selected element name.</param>
        /// <returns>Elements value</returns>
        /// <exception cref="ArgumentNullException">If parrent element is null.</exception>
        /// <exception cref="MissingXmlElement">If name of the element does not exist.</exception>
        public static string GetElementValueWithException(XElement element, string parentElementName, string elementName)
        {
            if (element == null)
            {
                throw new ArgumentNullException(string.Format("Element '{0}' is missing.", parentElementName));
            }
            XElement selectedElement = element.Element(elementName);
            if (selectedElement == null)
            {
                throw new MissingXmlElement(string.Format("Element '{0}' is missing.", elementName));
            }
            return selectedElement.Value;
        }

        /// <summary>
        /// Gets selected element value.
        /// </summary>
        /// <param name="element">Element which contains selected attribute.</param>
        /// <param name="elementName">Name of the selected element.</param>
        /// <param name="attributeName">Selected attribute name.</param>
        /// <returns>Elements value</returns>
        /// <exception cref="ArgumentNullException">If element is null.</exception>
        /// <exception cref="MissingXmlElement">If name of the element does not exist.</exception>
        public static string GetAttributeValueWithException(XElement element, string elementName, string attributeName)
        {
            if (element == null)
            {
                throw new ArgumentNullException(string.Format("Element '{0}' is missing.", elementName));
            }
            XAttribute selectedAttribute = element.Attribute(attributeName);
            if (selectedAttribute == null)
            {
                throw new MissingXmlElement(string.Format("Attribute '{0}' is missing.", attributeName));
            }
            return selectedAttribute.Value;
        }

        #endregion


        #region Possible attributes/elements exceptions used during deserialization

        /// <summary>
        /// Throws exception with proper error message if attribute is null.
        /// </summary>
        /// <param name="attribute">Xml attribute.</param>
        /// <param name="attributeName">Attribure name.</param>
        /// <param name="objectType">Deserialized object type.</param>
        /// <exception cref="MissingXmlAttribute">If attribute is missing.</exception>
        public static void ThrowExceptionIfAttributeIsNull(XmlAttribute attribute, string attributeName, string objectType)
        {
            if (attribute == null)
            {
                throw new MissingXmlAttribute(string.Format("Attribute '{0}' of {1} is missing.", attributeName, objectType));
            }
        }

        /// <summary>
        /// Throws exception with proper error message if attribute is null.
        /// </summary>
        /// <param name="attribute">Xml attribute.</param>
        /// <param name="attributeName">Attribure name.</param>
        /// <param name="elementName">Element name.</param>
        /// <param name="objectType">Deserialized object type.</param>
        /// <exception cref="MissingXmlAttribute">If attribute is missing.</exception>
        public static void ThrowExceptionIfAttributeIsNull(XmlAttribute attribute, string attributeName, string elementName, string objectType)
        {
            if (attribute == null)
            {
                throw new MissingXmlAttribute(string.Format("Attribute '{0}' of {1} of {2} is missing.",
                    attributeName, elementName, objectType));
            }
        }

        /// <summary>
        /// Throws exception with proper error message if attribute is null.
        /// </summary>
        /// <param name="attribute">Xml attribute.</param>
        /// <param name="attributeName">Attribure name.</param>
        /// <param name="elementName">Element name.</param>
        /// <param name="objectType">Deserialized object type.</param>
        /// <param name="objectName">Object name.</param>
        /// <exception cref="MissingXmlAttribute">If attribute is missing.</exception>
        public static void ThrowExceptionIfAttributeIsNull(XmlAttribute attribute, string attributeName, string elementName,
            string objectType, string objectName)
        {
            if (attribute == null)
            {
                throw new MissingXmlAttribute(string.Format("Attribute '{0}' of {1} of {2} '{3}' is missing.",
                    attributeName, elementName, objectType, objectName));
            }
        }

        /// <summary>
        /// Throws exception with proper error message if element is null.
        /// </summary>
        /// <param name="element">Xml node.</param>
        /// <param name="elementName">Element name.</param>
        /// <param name="objectType">Deserialized object type.</param>
        /// <exception cref="MissingXmlElement">If element is missing.</exception>
        public static void ThrowExceptionIfElementIsNull(XmlNode element, string elementName, string objectType)
        {
            if (element == null)
            {
                throw new MissingXmlElement(string.Format("Element '{0}' of {1} is missing.", elementName, objectType));
            }
        }

        /// <summary>
        /// Throws exception with proper error message if element is null.
        /// </summary>
        /// <param name="element">Xml node.</param>
        /// <param name="elementName">Element name.</param>
        /// <param name="objectType">Deserialized object type.</param>
        /// <param name="objectName">Object name.</param>
        /// <exception cref="MissingXmlElement">If element is missing.</exception>
        public static void ThrowExceptionIfElementIsNull(XmlNode element, string elementName, string objectType, string objectName)
        {
            if (element == null)
            {
                throw new MissingXmlElement(string.Format("Element '{0}' of {1} '{2}' is missing.", elementName, objectType, objectName));
            }
        }

        #endregion

        #endregion
    }
}
