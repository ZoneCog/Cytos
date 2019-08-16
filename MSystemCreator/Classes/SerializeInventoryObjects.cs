using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using SharedComponents.Tools;

namespace MSystemCreator.Classes
{
    /// <summary>
    /// Serialize new inventory objects to Xml structure.
    /// </summary>
    [Obsolete("User SerializeMSystem instead", true)]
    class SerializeInventoryObjects
    {
        #region Private data

        /// <summary>
        /// Xml document containing Inventory file.
        /// </summary>
        private static XmlDocument v_InventoryXmlDocument;

        /// <summary>
        /// Holds the type of the error message.
        /// </summary>
        private enum ErrorMessages
        { IsNullOrEmpty, IncorectCharacters, IncorectCharactersWithApostroph, IncorectNumber, IncorectFloatingNumber, EmptyList }

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
                    return string.Format("Incorect input number/s in {0}\nAllowed numbers are 0 - 9", exceptionName);
                case 4:
                    return string.Format("Incorect input number/s in {0}\nAllowed numbers are 0 - 9, 0.[0 - 9] *", exceptionName);
                case 5:
                    return string.Format("Parametr {0} can't be null or empty list.", exceptionName);
                default:
                    return "Incorect input.";
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Saves XML document as XML file.
        /// </summary>
        /// <param name="path">String location where you want to save Inventory file.</param>
        public void SaveXmlFile(string path)
        {
            v_InventoryXmlDocument.Save(path);
        }

        /// <summary>
        /// Gets Xml document as a string.
        /// </summary>
        /// <returns></returns>
        public string GetXmlDocAsAString()
        {
            XDocument inventoryFileDocument = XDocument.Parse(v_InventoryXmlDocument.OuterXml);
            return inventoryFileDocument.ToString();
        }

        /// <summary>
        /// Creates new Xml document.
        /// </summary>
        public void InitializeInventoryXmlDocument()
        {
            v_InventoryXmlDocument = new XmlDocument();
            // Master inventory.
            XmlElement inventory = (XmlElement)v_InventoryXmlDocument.AppendChild(v_InventoryXmlDocument.CreateElement("inventory"));
            // Child mSystemDeclaration.
            XmlElement mSystemDeclaration = (XmlElement)inventory.AppendChild(v_InventoryXmlDocument.CreateElement("mSystemDeclaration"));
            // Child floatingObjects.
            mSystemDeclaration.AppendChild(v_InventoryXmlDocument.CreateElement("floatingObjects"));
            // Child proteins.
            mSystemDeclaration.AppendChild(v_InventoryXmlDocument.CreateElement("proteins"));
            // Child fixedObjects.
            mSystemDeclaration.AppendChild(v_InventoryXmlDocument.CreateElement("fixedObjects"));
            // Child initialObjects.
            mSystemDeclaration.AppendChild(v_InventoryXmlDocument.CreateElement("initialObjects"));
            // Child environmentalObjects.
            mSystemDeclaration.AppendChild(v_InventoryXmlDocument.CreateElement("environmentalObjects"));
        }

        /// <summary>
        /// Creates new floating object.
        /// </summary>
        /// <param name="name">Floating object name.</param>
        /// <param name="valueOfSides">Sides value.</param>
        /// <param name="valueOfRadius">Radius value.</param>
        /// <param name="errorMessage">
        /// Output parameter, which holds error message catched during serialization.
        /// </param>
        /// <returns>
        /// True if serialization was succesfull otherwise returns false.
        /// </returns>
        public bool FloatingObjects(string name, string valueOfSides, string valueOfRadius, out string errorMessage)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    throw new ArgumentException(ExceptionsMessage("floating object", (int)ErrorMessages.IsNullOrEmpty));
                }
                if (!Regexp.CheckInputText(name, Regexp.Check.String))
                {
                    throw new ArgumentException(ExceptionsMessage("floating object", (int)ErrorMessages.IncorectCharacters));
                }
                if (string.IsNullOrEmpty(valueOfSides))
                {
                    throw new ArgumentException(ExceptionsMessage("sides", (int)ErrorMessages.IsNullOrEmpty));
                }
                if (!Regexp.CheckInputText(valueOfSides, Regexp.Check.FloatingNumber))
                {
                    throw new ArgumentException(ExceptionsMessage("sides", (int)ErrorMessages.IncorectFloatingNumber));
                }
                if (string.IsNullOrEmpty(valueOfRadius))
                {
                    throw new ArgumentException(ExceptionsMessage("radius", (int)ErrorMessages.IsNullOrEmpty));
                }
                if (!Regexp.CheckInputText(valueOfRadius, Regexp.Check.FloatingNumber))
                {
                    throw new ArgumentException(ExceptionsMessage("radius", (int)ErrorMessages.IncorectFloatingNumber));
                }

                /* <floatingObject name="a">
                        <sides value="5"/>
                        <radius value="0.05"/>
                   </floatingObject> */
                XmlElement floatingObject = v_InventoryXmlDocument.CreateElement("floatingObject");
                floatingObject.SetAttribute("name", name);

                XmlElement sides = (XmlElement)floatingObject.AppendChild(v_InventoryXmlDocument.CreateElement("sides"));
                sides.SetAttribute("value", valueOfSides);

                XmlElement radius = (XmlElement)floatingObject.AppendChild(v_InventoryXmlDocument.CreateElement("radius"));
                radius.SetAttribute("value", valueOfRadius);

                XmlNode floatingObjects = v_InventoryXmlDocument.SelectSingleNode("inventory/mSystemDeclaration/floatingObjects");
                floatingObjects?.AppendChild(floatingObject);

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
        /// Creates new protein element.
        /// </summary>
        /// <param name="name">Protein name.</param>
        /// <param name="errorMessage">
        /// Output parameter, which holds error message catched during serialization.
        /// </param>
        /// <returns>
        /// True if serialization was succesfull otherwise returns false.
        /// </returns>
        public bool Proteins(string name, out string errorMessage)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    throw new ArgumentException(ExceptionsMessage("name", (int)ErrorMessages.IsNullOrEmpty));
                }
                if (!Regexp.CheckInputText(name, Regexp.Check.StringWithApostroph))
                {
                    throw new ArgumentException(ExceptionsMessage("name", (int)ErrorMessages.IncorectCharactersWithApostroph));
                }

                // < protein name = "p0" />
                XmlElement protein = v_InventoryXmlDocument.CreateElement("protein");
                protein.SetAttribute("name", name);

                XmlNode proteins = v_InventoryXmlDocument.SelectSingleNode("inventory/mSystemDeclaration/proteins");
                proteins?.AppendChild(protein);

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
        /// Creates new initial object element.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="posX">Parameter posX.</param>
        /// <param name="posY">Parameter posY.</param>
        /// <param name="posZ">Parameter posZ.</param>
        /// <param name="angleX">Parameter angleX.</param>
        /// <param name="angleY">Parameter angleY.</param>
        /// <param name="angleZ">Parameter angleZ.</param>
        /// <param name="errorMessage">
        /// Output parameter, which holds error message catched during serialization.
        /// </param>
        /// <returns>
        /// True if serialization was succesfull otherwise returns false.
        /// </returns>
        public bool SeedTiles(string name, string posX, string posY, string posZ, string angleX, string angleY, string angleZ, out string errorMessage)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    throw new ArgumentException(ExceptionsMessage("name", (int)ErrorMessages.IsNullOrEmpty));
                }
                if (!Regexp.CheckInputText(name, Regexp.Check.String))
                {
                    throw new ArgumentException(ExceptionsMessage("name", (int)ErrorMessages.IncorectCharacters));
                }
                if (string.IsNullOrEmpty(posX))
                {
                    throw new ArgumentException(ExceptionsMessage("posX", (int)ErrorMessages.IsNullOrEmpty));
                }
                if (!Regexp.CheckInputText(posX, Regexp.Check.FloatingNumber))
                {
                    throw new ArgumentException(ExceptionsMessage("posX", (int)ErrorMessages.IncorectFloatingNumber));
                }
                if (string.IsNullOrEmpty(posY))
                {
                    throw new ArgumentException(ExceptionsMessage("posY", (int)ErrorMessages.IsNullOrEmpty));
                }
                if (!Regexp.CheckInputText(posY, Regexp.Check.FloatingNumber))
                {
                    throw new ArgumentException(ExceptionsMessage("posY", (int)ErrorMessages.IncorectFloatingNumber));
                }
                if (string.IsNullOrEmpty(posZ))
                {
                    throw new ArgumentException(ExceptionsMessage("posZ", (int)ErrorMessages.IsNullOrEmpty));
                }
                if (!Regexp.CheckInputText(posZ, Regexp.Check.FloatingNumber))
                {
                    throw new ArgumentException(ExceptionsMessage("posZ", (int)ErrorMessages.IncorectFloatingNumber));
                }
                if (string.IsNullOrEmpty(angleX))
                {
                    throw new ArgumentException(ExceptionsMessage("angleX", (int)ErrorMessages.IsNullOrEmpty));
                }
                if (!Regexp.CheckInputText(angleX, Regexp.Check.FloatingNumber))
                {
                    throw new ArgumentException(ExceptionsMessage("angleX", (int)ErrorMessages.IncorectFloatingNumber));
                }
                if (string.IsNullOrEmpty(angleY))
                {
                    throw new ArgumentException(ExceptionsMessage("angleY", (int)ErrorMessages.IsNullOrEmpty));
                }
                if (!Regexp.CheckInputText(angleY, Regexp.Check.FloatingNumber))
                {
                    throw new ArgumentException(ExceptionsMessage("angleY", (int)ErrorMessages.IncorectFloatingNumber));
                }
                if (string.IsNullOrEmpty(angleZ))
                {
                    throw new ArgumentException(ExceptionsMessage("angleZ", (int)ErrorMessages.IsNullOrEmpty));
                }
                if (!Regexp.CheckInputText(angleZ, Regexp.Check.FloatingNumber))
                {
                    throw new ArgumentException(ExceptionsMessage("angleZ", (int)ErrorMessages.IncorectFloatingNumber));
                }

                /* <initialObject name="q2">
                        <posX value="0"/>
                        <posY value="0"/>   
                        <posZ value="-16.18"/>
                        <angleX value="0"/> 
                        <angleY value="0"/>  
                        <angleZ value="0"/>
                   </initialObject> */
                XmlElement seedTile = v_InventoryXmlDocument.CreateElement("initialObject");
                seedTile.SetAttribute("name", name);

                XmlElement posXElement = (XmlElement)seedTile.AppendChild(v_InventoryXmlDocument.CreateElement("posX"));
                posXElement.SetAttribute("value", posX);
                XmlElement posYElement = (XmlElement)seedTile.AppendChild(v_InventoryXmlDocument.CreateElement("posY"));
                posYElement.SetAttribute("value", posY);
                XmlElement posZElement = (XmlElement)seedTile.AppendChild(v_InventoryXmlDocument.CreateElement("posZ"));
                posZElement.SetAttribute("value", posZ);
                XmlElement angleXElement = (XmlElement)seedTile.AppendChild(v_InventoryXmlDocument.CreateElement("angleX"));
                angleXElement.SetAttribute("value", angleX);
                XmlElement angleYElement = (XmlElement)seedTile.AppendChild(v_InventoryXmlDocument.CreateElement("angleY"));
                angleYElement.SetAttribute("value", angleY);
                XmlElement angleZElement = (XmlElement)seedTile.AppendChild(v_InventoryXmlDocument.CreateElement("angleZ"));
                angleZElement.SetAttribute("value", angleZ);

                XmlNode seedTiles = v_InventoryXmlDocument.SelectSingleNode("inventory/mSystemDeclaration/initialObjects");
                seedTiles?.AppendChild(seedTile);

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
        /// Creates new radius element.
        /// </summary>
        /// <param name="value">Radius value.</param>
        /// <param name="errorMessage">
        /// Output parameter, which holds error message catched during serialization.
        /// </param>
        /// <returns>
        /// True if serialization was succesfull otherwise returns false.
        /// </returns>
        public bool Radius(string value, out string errorMessage)
        {
            try
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException(ExceptionsMessage("value", (int)ErrorMessages.IsNullOrEmpty));
                }
                if (!Regexp.CheckInputText(value, Regexp.Check.FloatingNumber))
                {
                    throw new ArgumentException(ExceptionsMessage("value", (int)ErrorMessages.IncorectFloatingNumber));
                }

                //<radius value="0.1"/>
                XmlElement radiuseElement = v_InventoryXmlDocument.CreateElement("radius");
                radiuseElement.SetAttribute("name", value);

                XmlNode radius = v_InventoryXmlDocument.SelectSingleNode("inventory/mSystemDeclaration");
                radius?.AppendChild(radiuseElement);

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
        /// Creates new enviromental object element.
        /// </summary>
        /// <param name="name">Environmental object name.</param>
        /// <param name="errorMessage">
        /// Output parameter, which holds error message catched during serialization.
        /// </param>
        /// <returns>
        /// True if serialization was succesfull otherwise returns false.
        /// </returns>
        public bool EnvironmentalObjects(string name, out string errorMessage)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    throw new ArgumentException(ExceptionsMessage("name", (int)ErrorMessages.IsNullOrEmpty));
                }
                if (!Regexp.CheckInputText(name, Regexp.Check.String))
                {
                    throw new ArgumentException(ExceptionsMessage("name", (int)ErrorMessages.IncorectCharacters));
                }

                // <environmentalObject name="a"/>
                XmlElement environmentalObjectElement = v_InventoryXmlDocument.CreateElement("environmentalObject");
                environmentalObjectElement.SetAttribute("name", name);

                XmlNode environmentalObject = v_InventoryXmlDocument.SelectSingleNode("inventory/mSystemDeclaration/environmentalObjects");
                environmentalObject?.AppendChild(environmentalObjectElement);

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
        /// Creates new tile element.
        /// </summary>
        /// <param name="tileName">Tile name.</param>
        /// <param name="verticeses">List of vertexs.</param>
        /// <param name="positionsList">List of positions.</param>
        /// <param name="connectorList">List of connectors.</param>
        /// <param name="surfaceGluen">Surface glue value.</param>
        /// <param name="proteinsList">List of protein.</param>
        /// <param name="errorMessage">
        /// Output parameter, which holds error message catched during serialization.
        /// </param>
        /// <returns>
        /// True if serialization was succesfull otherwise returns false.
        /// </returns>
        public bool Tiles(string tileName, List<Vertices> verticeses, List<Positions> positionsList, List<Connect> connectorList, string surfaceGluen, List<string> proteinsList, out string errorMessage)
        {
            try
            {
                if (string.IsNullOrEmpty(tileName))
                {
                    throw new ArgumentException(ExceptionsMessage("tileName", (int)ErrorMessages.IsNullOrEmpty));
                }
                if (!Regexp.CheckInputText(tileName, Regexp.Check.String))
                {
                    throw new ArgumentException(ExceptionsMessage("tileName", (int)ErrorMessages.IncorectCharacters));
                }
                if (string.IsNullOrEmpty(surfaceGluen))
                {
                    throw new ArgumentException(ExceptionsMessage("surfaceGlue", (int)ErrorMessages.IsNullOrEmpty));
                }
                if (!Regexp.CheckInputText(surfaceGluen, Regexp.Check.String))
                {
                    throw new ArgumentException(ExceptionsMessage("surfaceGlue", (int)ErrorMessages.IncorectCharacters));
                }
                if (connectorList.Count == 0)
                {
                    throw new ArgumentException(ExceptionsMessage("positionList", (int)ErrorMessages.EmptyList));
                }

                XmlElement xmlTile = v_InventoryXmlDocument.CreateElement("fixedObject");
                xmlTile.SetAttribute("name", tileName);

                XmlElement vertices = (XmlElement)xmlTile.AppendChild(v_InventoryXmlDocument.CreateElement("vertices"));
                foreach (Vertices vertic in verticeses)
                {
                    XmlElement vertex = (XmlElement)vertices.AppendChild(v_InventoryXmlDocument.CreateElement("vertex"));
                    vertex.SetAttribute("name", vertic.VertexName);
                    XmlElement posX = (XmlElement)vertex.AppendChild(v_InventoryXmlDocument.CreateElement("posX"));
                    posX.SetAttribute("value", vertic.PosX.ToString(CultureInfo.InvariantCulture));
                    XmlElement posY = (XmlElement)vertex.AppendChild(v_InventoryXmlDocument.CreateElement("posY"));
                    posY.SetAttribute("value", vertic.PosY.ToString(CultureInfo.InvariantCulture));
                    XmlElement posZ = (XmlElement)vertex.AppendChild(v_InventoryXmlDocument.CreateElement("posZ"));
                    posZ.SetAttribute("value", vertic.PosZ.ToString(CultureInfo.InvariantCulture));
                }
                if (positionsList.Count != 0)
                {
                    XmlElement positions = (XmlElement)xmlTile.AppendChild(v_InventoryXmlDocument.CreateElement("positions"));
                    foreach (Positions posit in positionsList)
                    {
                        XmlElement position = (XmlElement)positions.AppendChild(v_InventoryXmlDocument.CreateElement("position"));
                        position.SetAttribute("name", posit.PositionName);
                        XmlElement positionPosX = (XmlElement)position.AppendChild(v_InventoryXmlDocument.CreateElement("posX"));
                        positionPosX.SetAttribute("value", posit.PositionPosX.ToString(CultureInfo.InvariantCulture));
                        XmlElement positionPosY = (XmlElement)position.AppendChild(v_InventoryXmlDocument.CreateElement("posY"));
                        positionPosY.SetAttribute("value", posit.PositionPosY.ToString(CultureInfo.InvariantCulture));
                        XmlElement positionPosZ = (XmlElement)position.AppendChild(v_InventoryXmlDocument.CreateElement("posZ"));
                        positionPosZ.SetAttribute("value", posit.PositionPosZ.ToString(CultureInfo.InvariantCulture));
                    }
                }

                XmlElement connectors = (XmlElement)xmlTile.AppendChild(v_InventoryXmlDocument.CreateElement("connectors"));
                foreach (Connect connect in connectorList)
                {
                    XmlElement connector = (XmlElement)connectors.AppendChild(v_InventoryXmlDocument.CreateElement("connector"));
                    connector.SetAttribute("name", connect.ConnectorAngle);

                    XmlElement connectorPositions = (XmlElement)connector.AppendChild(v_InventoryXmlDocument.CreateElement("positions"));

                    foreach (string position in connect.ConnectorPosition)
                    {
                        XmlElement connectorPosition = (XmlElement)connectorPositions.AppendChild(v_InventoryXmlDocument.CreateElement("position"));
                        connectorPosition.SetAttribute("name", position);
                    }

                    XmlElement protein = (XmlElement)connector.AppendChild(v_InventoryXmlDocument.CreateElement("protein"));
                    protein.SetAttribute("name", connect.ConnectorName);

                    if (connect.ConnectorAngle != string.Empty)
                    {
                        XmlElement angle = (XmlElement)connector.AppendChild(v_InventoryXmlDocument.CreateElement("angle"));
                        angle.SetAttribute("value", connect.ConnectorAngle.ToString(CultureInfo.InvariantCulture));
                    }
                }

                XmlElement surfaceGlue = (XmlElement)xmlTile.AppendChild(v_InventoryXmlDocument.CreateElement("surfaceGlue"));
                surfaceGlue.SetAttribute("name", surfaceGluen);

                XmlElement proteins = (XmlElement)xmlTile.AppendChild(v_InventoryXmlDocument.CreateElement("proteins"));
                if (proteinsList.Count != 0)
                {
                    foreach (string protein in proteinsList)
                    {
                        XmlElement proteinsProtein = (XmlElement)proteins.AppendChild(v_InventoryXmlDocument.CreateElement("protein"));
                        proteinsProtein.SetAttribute("name", protein);
                    }
                }

                XmlNode tiles = v_InventoryXmlDocument.SelectSingleNode("inventory/mSystemDeclaration/fixedObjects");
                tiles?.AppendChild(xmlTile);

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
        /// Creates new tile element.
        /// </summary>
        /// <param name="tileName">Tile name.</param>
        /// <param name="polygonSides">Polygon sides value.</param>
        /// <param name="polygonVertexDistance">Polygon vertex distance value.</param>
        /// <param name="polygonAngle">Polygon angle value.</param>
        /// <param name="positionsList">List of positions.</param>
        /// <param name="connectorList">List of connectors.</param>
        /// <param name="surfaceGluen">Surface glue value.</param>
        /// <param name="proteinsList">List of proteins.</param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public bool Tiles(string tileName, string polygonSides, string polygonVertexDistance, string polygonAngle, List<Positions> positionsList, List<Connect> connectorList, string surfaceGluen, List<string> proteinsList, out string errorMessage)
        {
            try
            {
                if (string.IsNullOrEmpty(tileName))
                {
                    throw new ArgumentException(ExceptionsMessage("tileName", (int)ErrorMessages.IsNullOrEmpty));
                }
                if (!Regexp.CheckInputText(tileName, Regexp.Check.String))
                {
                    throw new ArgumentException(ExceptionsMessage("tileName", (int)ErrorMessages.IncorectCharacters));
                }
                if (string.IsNullOrEmpty(polygonSides))
                {
                    throw new ArgumentException(ExceptionsMessage("polygonSides", (int)ErrorMessages.IsNullOrEmpty));
                }
                if (!Regexp.CheckInputText(polygonSides, Regexp.Check.Number))
                {
                    throw new ArgumentException(ExceptionsMessage("polygonSides", (int)ErrorMessages.IncorectNumber));
                }
                if (string.IsNullOrEmpty(polygonVertexDistance))
                {
                    throw new ArgumentException(ExceptionsMessage("polygonVertexDistance", (int)ErrorMessages.IsNullOrEmpty));
                }
                if (!Regexp.CheckInputText(polygonVertexDistance, Regexp.Check.Number))
                {
                    throw new ArgumentException(ExceptionsMessage("polygonVertexDistance", (int)ErrorMessages.IncorectNumber));
                }
                if (string.IsNullOrEmpty(polygonAngle))
                {
                    throw new ArgumentException(ExceptionsMessage("polygonAngle", (int)ErrorMessages.IsNullOrEmpty));
                }
                if (!Regexp.CheckInputText(polygonAngle, Regexp.Check.FloatingNumber))
                {
                    throw new ArgumentException(ExceptionsMessage("polygonAngle", (int)ErrorMessages.IncorectFloatingNumber));
                }
                if (string.IsNullOrEmpty(surfaceGluen))
                {
                    throw new ArgumentException(ExceptionsMessage("surfaceGlue", (int)ErrorMessages.IsNullOrEmpty));
                }
                if (!Regexp.CheckInputText(surfaceGluen, Regexp.Check.String))
                {
                    throw new ArgumentException(ExceptionsMessage("surfaceGlue", (int)ErrorMessages.IncorectCharacters));
                }
                if (connectorList.Count == 0)
                {
                    throw new ArgumentException(ExceptionsMessage("connectorList", (int)ErrorMessages.EmptyList));
                }

                XmlElement xmlTiles = v_InventoryXmlDocument.CreateElement("tile");
                xmlTiles.SetAttribute("name", tileName);

                XmlElement polygon = (XmlElement)xmlTiles.AppendChild(v_InventoryXmlDocument.CreateElement("polygon"));
                XmlElement sides = (XmlElement)polygon.AppendChild(v_InventoryXmlDocument.CreateElement("sides"));
                sides.SetAttribute("value", polygonSides);
                XmlElement vertexDistance = (XmlElement)polygon.AppendChild(v_InventoryXmlDocument.CreateElement("vertexDistance"));
                vertexDistance.SetAttribute("value", polygonVertexDistance);
                XmlElement angleElement = (XmlElement)polygon.AppendChild(v_InventoryXmlDocument.CreateElement("angleElement"));
                angleElement.SetAttribute("value", polygonAngle);

                if (positionsList.Count != 0)
                {
                    XmlElement positions = (XmlElement)xmlTiles.AppendChild(v_InventoryXmlDocument.CreateElement("positions"));
                    foreach (Positions posit in positionsList)
                    {
                        XmlElement position = (XmlElement)positions.AppendChild(v_InventoryXmlDocument.CreateElement("position"));
                        position.SetAttribute("name", posit.PositionName);
                        XmlElement positionPosX = (XmlElement)position.AppendChild(v_InventoryXmlDocument.CreateElement("posX"));
                        positionPosX.SetAttribute("value", posit.PositionPosX.ToString(CultureInfo.InvariantCulture));
                        XmlElement positionPosY = (XmlElement)position.AppendChild(v_InventoryXmlDocument.CreateElement("posY"));
                        positionPosY.SetAttribute("value", posit.PositionPosY.ToString(CultureInfo.InvariantCulture));
                        XmlElement positionPosZ = (XmlElement)position.AppendChild(v_InventoryXmlDocument.CreateElement("posZ"));
                        positionPosZ.SetAttribute("value", posit.PositionPosZ.ToString(CultureInfo.InvariantCulture));
                    }
                }

                XmlElement connectors = (XmlElement)xmlTiles.AppendChild(v_InventoryXmlDocument.CreateElement("connectors"));
                foreach (Connect connect in connectorList)
                {

                    XmlElement connector = (XmlElement)connectors.AppendChild(v_InventoryXmlDocument.CreateElement("connector"));
                    connector.SetAttribute("name", connect.ConnectorAngle);

                    XmlElement connectorPositions = (XmlElement)connector.AppendChild(v_InventoryXmlDocument.CreateElement("positions"));

                    foreach (string position in connect.ConnectorPosition)
                    {
                        XmlElement connectorPosition = (XmlElement)connectorPositions.AppendChild(v_InventoryXmlDocument.CreateElement("position"));
                        connectorPosition.SetAttribute("name", position);
                    }

                    XmlElement protein = (XmlElement)connector.AppendChild(v_InventoryXmlDocument.CreateElement("protein"));
                    protein.SetAttribute("name", connect.ConnectorProtein);

                    if (connect.ConnectorAngle != string.Empty)
                    {
                        XmlElement angle = (XmlElement)connector.AppendChild(v_InventoryXmlDocument.CreateElement("angle"));
                        angle.SetAttribute("value", connect.ConnectorAngle.ToString(CultureInfo.InvariantCulture));
                    }
                }

                XmlElement surfaceGlue = (XmlElement)xmlTiles.AppendChild(v_InventoryXmlDocument.CreateElement("surfaceGlue"));
                surfaceGlue.SetAttribute("name", surfaceGluen);

                XmlElement proteins = (XmlElement)xmlTiles.AppendChild(v_InventoryXmlDocument.CreateElement("proteins"));
                if (proteinsList.Count != 0)
                {
                    foreach (string protein in proteinsList)
                    {
                        XmlElement proteinsProtein = (XmlElement)proteins.AppendChild(v_InventoryXmlDocument.CreateElement("protein"));
                        proteinsProtein.SetAttribute("name", protein);
                    }
                }

                XmlNode tiles = v_InventoryXmlDocument.SelectSingleNode("inventory/mSystemDeclaration/fixedObjects");
                tiles?.AppendChild(xmlTiles);
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

        #region Tile structures

        /// <summary>
        /// Vertex structure.
        /// </summary>
        // <vertex name="v1">
        //     <posX value="10"/>
        //     <posY value="0"/>
        //     <posZ value="0"/>
        // </vertex>
        public struct Vertices
        {
            public string VertexName;
            public string PosX;
            public string PosY;
            public string PosZ;
        }

        /// <summary>
        /// Position structure.
        /// </summary>
        // <position name="point1" >
        //      <posX value="0"/>
        //      <posY value="0"/>
        //      <posZ value="0"/>
        // </position>
        public struct Positions
        {
            public string PositionName;
            public string PositionPosX;
            public string PositionPosY;
            public string PositionPosZ;
        }

        /// <summary>
        /// Connect structure.
        /// </summary>
        /// <connector name="c1"/>
        //      <positions>
        //          <position name="v1"/>
        //          <position name="v2"/>
        //      </positions >
        // <protein name="pa"/>
        // <angle value="2.0345"/>
        // </connector>
        public struct Connect
        {
            public string ConnectorName;
            public List<string> ConnectorPosition;
            public string ConnectorProtein;
            public string ConnectorAngle;
        }

        #endregion
    }
}
