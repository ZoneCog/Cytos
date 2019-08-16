using SharedComponents.Forms;
using SharedComponents.Tools;
using SharedComponents.XML;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace MSystemCreator.Classes
{
    /// <summary>
    /// Main class for M System serialization.
    /// </summary>
    class SerializeMSystem
    {
        #region Public Data

        /// <summary>
        /// Main XML document which contains all created elements.
        /// </summary>
        public XDocument MSystemXml { get; private set; }

        #endregion

        #region Private data

        /// <summary>
        /// Root element.
        /// </summary>
        private XElement v_Root;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public SerializeMSystem()
        {
            CreateEmptyXml();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Creates tile element.
        /// </summary>
        public void CreateTile()
        {
            InputBox input = new InputBox("Tile", "OK", new List<string> { "Tile name", "Polygon sides - number", "Polygon radius - number", "Position name",
                    "Position X - number", "Position Y - number", "Connection angle - floating number", "Angle unit - rad or deg", "Surface glue name",
                    "Alpha ratio - floating number (optional)", "Color name", "Alpha color - number 0-255", "Thickness - floating number (optional)" },
                new List<Regexp.Check> { Regexp.Check.String, Regexp.Check.Number, Regexp.Check.Number, Regexp.Check.String, Regexp.Check.Number, Regexp.Check.Number,
                    Regexp.Check.FloatingNumber, Regexp.Check.String, Regexp.Check.String, Regexp.Check.FloatingNumberOrEmptyString, Regexp.Check.String, Regexp.Check.Number,
                    Regexp.Check.FloatingNumberOrEmptyString});
            input.ShowDialog();

            List<string> tileParameters = input.OutputTexts;

            if (tileParameters.Count == 13)
            {
                string tileName = tileParameters[0];
                string polygonSides = tileParameters[1];
                string polygonRadius = tileParameters[2];
                string positionName = tileParameters[3];
                string posX = tileParameters[4];
                string posY = tileParameters[5];
                string connectionAngle = tileParameters[6];
                string angleUnit = tileParameters[7];
                string surfaceGlueName = tileParameters[8];
                string alphaRatio = tileParameters[9];
                string colorName = tileParameters[10];
                string alphaColor = tileParameters[11];
                string thickness = tileParameters[12];

                XElement tile = new XElement("tile", new XAttribute("name", tileName),
                    new XElement("polygon", new XElement("sides", new XAttribute("value", polygonSides)), new XElement("radius", new XAttribute("value", polygonRadius))),
                    new XElement("positions",
                        new XElement("position", new XAttribute("name", positionName), new XElement("posX", new XAttribute("value", posX)), new XElement("posY", new XAttribute("value", posY)))),
                    new XElement("connectingAngle", new XAttribute("value", connectionAngle), new XAttribute("unit", angleUnit)),
                    new XElement("connectors"),
                    new XElement("surfaceGlue", new XAttribute("name", surfaceGlueName))
                );

                if (!string.IsNullOrEmpty(alphaRatio))
                {
                    tile.Add(new XElement("alphaRatio", new XAttribute("value", alphaRatio)));
                }

                tile.Add(new XElement("color", new XAttribute("name", colorName), new XAttribute("alpha", alphaColor)));

                if (!string.IsNullOrEmpty(thickness))
                {
                    tile.Add(new XElement("thickness", new XAttribute("value", thickness)));
                }

                v_Root.GetElement("tiling/tiles").Add(tile);
            }
        }

        /// <summary>
        /// Creates tile element using vertices.
        /// </summary>
        public void CreateTileUsingVertices()
        {
            //TODO low priority: serialize tile using vertices
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates connector element.
        /// </summary>
        /// <param name="errorMessage">Output error message</param>
        /// <returns>Whether object was created</returns>
        public bool CreateConnector(out string errorMessage)
        {
            errorMessage = string.Empty;

            InputBox input = new InputBox("Connector", "OK",
                new List<string> { "Tile name", "Connector name", "Position name", "Glue name", "Angle - number (optional)",
                    "Angle unit - rad or deg (optional)", "Resistance - number (optional)" },
                new List<Regexp.Check> { Regexp.Check.String, Regexp.Check.String, Regexp.Check.String, Regexp.Check.String, Regexp.Check.NumberOrEmptyString,
                    Regexp.Check.StringOrEmptyString, Regexp.Check.NumberOrEmptyString });
            input.ShowDialog();

            List<string> connectorParameters = input.OutputTexts;

            if (connectorParameters.Count == 7)
            {
                string tileName = connectorParameters[0];
                string connectorName = connectorParameters[1];
                string positionName = connectorParameters[2];
                string glueName = connectorParameters[3];
                string angle = connectorParameters[4];
                string angleUnit = connectorParameters[5];
                string resistance = connectorParameters[6];

                XElement tiles = v_Root.GetElement("tiling/tiles");

                if (!tiles.ExistsElementWithSpecificAttribute("tile", "name", tileName))
                {
                    errorMessage = $"Tile with name '{tileName}' does not exists!";
                    return false;
                }
                XElement tile = tiles.GetElementWithSpecificAttribute("tile", "name", tileName);

                XElement connector = new XElement("connector", new XAttribute("name", connectorName),
                    new XElement("positions", new XElement("position", new XAttribute("name", positionName))),
                    new XElement("glue", new XAttribute("name", glueName))
                );
                if (!string.IsNullOrEmpty(angle) && !string.IsNullOrEmpty(angleUnit))
                {
                    connector.Add(new XElement("angle", new XAttribute("value", angle), new XAttribute("unit", angleUnit)));
                }
                if (!string.IsNullOrEmpty(resistance))
                {
                    connector.Add(new XElement("resistance", new XAttribute("value", resistance)));
                }

                // ReSharper disable once PossibleNullReferenceException
                tile.Element("connectors").Add(connector);
            }

            return true;
        }

        /// <summary>
        /// Creates glue element.
        /// </summary>
        public void CreateGlue()
        {
            InputBox input = new InputBox("Glue", "OK", new List<string> { "Glue name" }, new List<Regexp.Check> { Regexp.Check.String });
            input.ShowDialog();

            List<string> glueParameters = input.OutputTexts;

            if (glueParameters.Count == 1)
            {
                string name = glueParameters[0];
                v_Root.GetElement("tiling/glues").Add(new XElement("glue", new XAttribute("name", name)));
            }
        }

        /// <summary>
        /// Creates glue relation element.
        /// </summary>
        public void CreateGlueRelation()
        {
            InputBox input = new InputBox("Glue tuple", "OK", new List<string> { "Glue 1", "Glue 2" }, new List<Regexp.Check> { Regexp.Check.String, Regexp.Check.String });
            input.ShowDialog();

            List<string> glueRelationParameters = input.OutputTexts;

            if (glueRelationParameters.Count == 2)
            {
                string glue1 = glueRelationParameters[0];
                string glue2 = glueRelationParameters[1];

                v_Root.GetElement("tiling/glueRelations").Add(new XElement("glueTuple", new XAttribute("glue1", glue1), new XAttribute("glue2", glue2)));
            }
        }

        /// <summary>
        /// Creates initial object element.
        /// </summary>
        public void CreateInitialObject()
        {
            InputBox input = new InputBox("Initial object", "OK",
                new List<string> { "Initial object name", "Pos X - floating number", "Pos Y - floating number", "Pos Z - floating number",
                    "Angle X - floating number", "Angle Y - floating number", "Angle Z - floating number" },
                new List<Regexp.Check> { Regexp.Check.String, Regexp.Check.FloatingNumber, Regexp.Check.FloatingNumber, Regexp.Check.FloatingNumber,
                    Regexp.Check.FloatingNumber, Regexp.Check.FloatingNumber, Regexp.Check.FloatingNumber });
            input.ShowDialog();

            List<string> initialObjectParameters = input.OutputTexts;

            if (initialObjectParameters.Count == 7)
            {
                string name = initialObjectParameters[0];
                string posX = initialObjectParameters[1];
                string posY = initialObjectParameters[2];
                string posZ = initialObjectParameters[3];
                string angleX = initialObjectParameters[4];
                string angleY = initialObjectParameters[5];
                string angleZ = initialObjectParameters[6];

                XElement initialObject = new XElement("initialObject", new XAttribute("name", name));
                AddPositionToElement(initialObject, posX, posY, posZ);
                AddAngleToElement(initialObject, angleX, angleY, angleZ);

                v_Root.GetElement("tiling/initialObjects").Add(initialObject);
            }
        }

        /// <summary>
        /// Creates glue radius element.
        /// </summary>
        /// <param name="errorMessage">Output error message</param>
        /// <returns>Whether object was created</returns>
        public bool CreateGlueRadius(out string errorMessage)
        {
            errorMessage = string.Empty;

            if (CheckElementsExistanceAndShowMessage("tiling/glueRadius", out errorMessage))
            {
                return false;
            }

            InputBox input = new InputBox("Glue radius", "OK", new List<string> { "Value of radius - floating number" }, new List<Regexp.Check> { Regexp.Check.FloatingNumber });
            input.ShowDialog();

            List<string> glueRadiusParameters = input.OutputTexts;

            if (glueRadiusParameters.Count == 1)
            {
                string reactionRadius = glueRadiusParameters[0];
                v_Root.GetElement("tiling").Add(new XElement("glueRadius", new XAttribute("value", reactionRadius)));
            }

            return true;
        }

        /// <summary>
        /// Creates battery voltage element.
        /// </summary>
        /// <param name="errorMessage">Output error message</param>
        /// <returns>Whether object was created</returns>
        public bool CreateBatteryVoltage(out string errorMessage)
        {
            errorMessage = string.Empty;

            if (CheckElementsExistanceAndShowMessage("tiling/batteryVoltage", out errorMessage))
            {
                return false;
            }

            InputBox input = new InputBox("Battery voltage", "OK", new List<string> { "Battery voltage - floating number" },
                new List<Regexp.Check> { Regexp.Check.FloatingNumber });
            input.ShowDialog();

            List<string> batteryVoltageParameters = input.OutputTexts;

            if (batteryVoltageParameters.Count == 1)
            {
                string batteryVoltage = batteryVoltageParameters[0];
                v_Root.GetElement("tiling").Add(new XElement("batteryVoltage", new XAttribute("value", batteryVoltage)));
            }

            return true;
        }

        /// <summary>
        /// Creates threshold potential element.
        /// </summary>
        /// <param name="errorMessage">Output error message</param>
        /// <returns>Whether object was created</returns>
        public bool CreateThresholdPotential(out string errorMessage)
        {
            errorMessage = string.Empty;

            if (CheckElementsExistanceAndShowMessage("tiling/thresholdPotential", out errorMessage))
            {
                return false;
            }

            InputBox input = new InputBox("Threshold potential", "OK", new List<string> { "Threshold potential - floating number" },
                new List<Regexp.Check> { Regexp.Check.FloatingNumber });
            input.ShowDialog();

            List<string> thresholdPotentialParameters = input.OutputTexts;

            if (thresholdPotentialParameters.Count == 1)
            {
                string thresholdPotential = thresholdPotentialParameters[0];
                v_Root.GetElement("tiling").Add(new XElement("thresholdPotential", new XAttribute("value", thresholdPotential)));
            }

            return true;
        }

        /// <summary>
        /// Creates pushing coef. element.
        /// </summary>
        /// <param name="errorMessage">Output error message</param>
        /// <returns>Whether object was created</returns>
        public bool CreatePushingCoef(out string errorMessage)
        {
            errorMessage = string.Empty;

            if (CheckElementsExistanceAndShowMessage("tiling/pushingCoef", out errorMessage))
            {
                return false;
            }

            InputBox input = new InputBox("Pushing coefficient", "OK", new List<string> { "Pushing coefficient - floating number" },
                new List<Regexp.Check> { Regexp.Check.FloatingNumber });
            input.ShowDialog();

            List<string> pushingCoefParameters = input.OutputTexts;

            if (pushingCoefParameters.Count == 1)
            {
                string thresholdPotential = pushingCoefParameters[0];
                v_Root.GetElement("tiling").Add(new XElement("pushingCoef", new XAttribute("value", thresholdPotential)));
            }

            return true;
        }

        /// <summary>
        /// Creates floating object element.
        /// </summary>
        public void CreateFloatingObject()
        {
            InputBox input = new InputBox("Floating object", "OK",
                new List<string> { "Name", "Shape", "Size - floating number", "Color name", "Alpha color - number 0-255", "Mobility - number", "Concentration - floating number" },
                new List<Regexp.Check>
                    { Regexp.Check.String, Regexp.Check.String, Regexp.Check.FloatingNumber, Regexp.Check.String, Regexp.Check.Number, Regexp.Check.Number, Regexp.Check.FloatingNumber });
            input.ShowDialog();

            List<string> floatingObjectParameters = input.OutputTexts;

            if (floatingObjectParameters.Count == 7)
            {
                string name = floatingObjectParameters[0];
                string shape = floatingObjectParameters[1];
                string size = floatingObjectParameters[2];
                string colorName = floatingObjectParameters[3];
                string alpha = floatingObjectParameters[4];
                string mobility = floatingObjectParameters[5];
                string concentration = floatingObjectParameters[6];

                v_Root.GetElement("Msystem/floatingObjects").Add(new XElement("floatingObject", new XAttribute("name", name),
                    new XElement("shape", new XAttribute("value", shape)),
                    new XElement("size", new XAttribute("value", size)),
                    new XElement("color", new XAttribute("name", colorName), new XAttribute("alpha", alpha)),
                    new XElement("mobility", new XAttribute("value", mobility)),
                    new XElement("concentration", new XAttribute("value", concentration))
                ));
            }
        }

        /// <summary>
        /// Creates protein element.
        /// </summary>
        public void CreateProtein()
        {
            InputBox input = new InputBox("Protein", "OK", new List<string> { "Protein name" }, new List<Regexp.Check> { Regexp.Check.String });
            input.ShowDialog();

            List<string> proteinParameters = input.OutputTexts;

            if (proteinParameters.Count == 1)
            {
                string proteinName = proteinParameters[0];
                v_Root.GetElement("Msystem/proteins").Add(new XElement("protein", new XAttribute("name", proteinName)));
            }
        }

        /// <summary>
        /// Creates protein on tile element.
        /// </summary>
        public void CreateProteinOnTile()
        {
            InputBox input = new InputBox("Protein on tile", "OK", new List<string> { "Tile name", "Protein name", "Position X - number", "Position Y - number" },
                new List<Regexp.Check> { Regexp.Check.String, Regexp.Check.String, Regexp.Check.Number, Regexp.Check.Number });
            input.ShowDialog();

            List<string> proteinOnTileParameters = input.OutputTexts;

            if (proteinOnTileParameters.Count == 4)
            {
                string tileName = proteinOnTileParameters[0];
                string proteinName = proteinOnTileParameters[1];
                string posX = proteinOnTileParameters[2];
                string posY = proteinOnTileParameters[3];

                XElement proteinOnTiles = v_Root.GetElement("Msystem/proteinsOnTiles");

                if (!proteinOnTiles.ExistsElementWithSpecificAttribute("tile", "name", tileName))
                {
                    proteinOnTiles.Add(new XElement("tile", new XAttribute("name", tileName)));
                }
                XElement proteinOnTileTile = proteinOnTiles.GetElementWithSpecificAttribute("tile", "name", tileName);

                proteinOnTileTile.Add(new XElement("protein", new XAttribute("name", proteinName),
                    new XElement("posX", new XAttribute("value", posX)), new XElement("posY", new XAttribute("value", posY))));
            }
        }

        /// <summary>
        /// Creates evo rule element.
        /// </summary>
        public void CreateEvoRule()
        {
            InputBox input = new InputBox("Evo rule", "OK", new List<string> { "Rule type - string", "Priority - number (optional)", "Left side - string", "Right side - string" },
                new List<Regexp.Check> { Regexp.Check.String, Regexp.Check.NumberOrEmptyString, Regexp.Check.String, Regexp.Check.String });
            input.ShowDialog();

            List<string> evoRuleParameters = input.OutputTexts;

            if (evoRuleParameters.Count == 4)
            {
                string ruleType = evoRuleParameters[0];
                string priority = evoRuleParameters[1];
                string leftSide = evoRuleParameters[2];
                string rightSide = evoRuleParameters[3];

                XElement rule = new XElement("evoRule", new XAttribute("type", ruleType));

                if (!string.IsNullOrEmpty(priority))
                {
                    rule.Add(new XAttribute("priority", priority));
                }

                XElement leftSideElement = new XElement("leftside", new XAttribute("value", leftSide));
                XElement rightSideElement = new XElement("rightside", new XAttribute("value", rightSide));

                rule.Add(leftSideElement, rightSideElement);
                v_Root.GetElement("Msystem/evoRulesWithPriority").Add(rule);
            }
        }

        /// <summary>
        /// Creates signal object element.
        /// </summary>
        public void CreateSignalObject()
        {
            InputBox input = new InputBox("Signal object ", "OK", new List<string> { "Glue 1", "Glue 2", "Objects - separated by ','" },
                new List<Regexp.Check> { Regexp.Check.String, Regexp.Check.String, Regexp.Check.String });
            input.ShowDialog();

            List<string> signalObjectParameters = input.OutputTexts;

            if (signalObjectParameters.Count == 3)
            {
                string glue1 = signalObjectParameters[0];
                string glue2 = signalObjectParameters[1];
                string signalObjects = signalObjectParameters[2];

                v_Root.GetElement("Msystem/signalObjects").Add(new XElement("glueTuple", new XAttribute("glue1", glue1), new XAttribute("glue2", glue2),
                    new XElement("objects", new XAttribute("value", signalObjects))));
            }
        }

        /// <summary>
        /// Creates reaction radius element.
        /// </summary>
        /// <param name="errorMessage">Output error message</param>
        /// <returns>Whether object was created</returns>
        public bool CreateReactionRadius(out string errorMessage)
        {
            errorMessage = string.Empty;

            if (CheckElementsExistanceAndShowMessage("Msystem/reactionRadius", out errorMessage))
            {
                return false;
            }

            InputBox input = new InputBox("Reaction radius", "OK", new List<string> { "Reaction radius - floating number" },
                new List<Regexp.Check> { Regexp.Check.FloatingNumber });
            input.ShowDialog();

            List<string> reactionRadiusParameters = input.OutputTexts;

            if (reactionRadiusParameters.Count == 1)
            {
                string reactionRadius = reactionRadiusParameters[0];
                v_Root.GetElement("Msystem").Add(new XElement("reactionRadius", new XAttribute("value", reactionRadius)));
            }

            return true;
        }

        /// <summary>
        /// Created new XDocument from given XML text.
        /// </summary>
        /// <param name="xmlText">XML in text</param>
        /// <param name="errorMessage">Output error message</param>
        /// <returns>
        /// True if XML was created, otherwise false.
        /// </returns>
        public bool UpdateXmlWithManualModifications(string xmlText, out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                MSystemXml = XDocument.Parse(xmlText);
                v_Root = MSystemXml.Root;
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Loads XML from existing file.
        /// </summary>
        /// <param name="filePath">File path.</param>
        /// <param name="errorMessage">Output error message</param>
        /// <returns>
        /// True if XML was loaded succesfully, otherwise false.
        /// </returns>
        public bool LoadExistingXmlFromFile(string filePath, out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                MSystemXml = XDocument.Load(filePath);
                v_Root = MSystemXml.Root;
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
                return false;
            }

            return true;
        }

        #endregion

        #region Private methods
        
        /// <summary>
        /// Creates new XML with all required elements.
        /// </summary>
        private void CreateEmptyXml()
        {
            MSystemXml = new XDocument();
            XElement root = new XElement("root");

            XElement tiling = new XElement("tiling",
                new XElement("tiles"),
                new XElement("glues"),
                new XElement("glueRelations"),
                new XElement("initialObjects"));

            XElement mSystem = new XElement("Msystem",
                new XElement("floatingObjects"),
                new XElement("proteins"),
                new XElement("proteinsOnTiles"),
                new XElement("evoRulesWithPriority"),
                new XElement("signalObjects"));

            root.Add(tiling, mSystem);
            MSystemXml.Add(root);
            v_Root = MSystemXml.Root;
        }

        /// <summary>
        /// Adds position to parent element.
        /// </summary>
        /// <param name="parentElement">Parent Element.</param>
        /// <param name="x">Position X.</param>
        /// <param name="y">Position Y.</param>
        /// <param name="z">Position Z.</param>
        private void AddPositionToElement(XElement parentElement, string x, string y, string z)
        {
            parentElement.Add(new XElement("posX", new XAttribute("value", x)));
            parentElement.Add(new XElement("posY", new XAttribute("value", y)));
            parentElement.Add(new XElement("posZ", new XAttribute("value", z)));
        }

        /// <summary>
        /// Adds angle to parent element.
        /// </summary>
        /// <param name="parentElement">Parent Element.</param>
        /// <param name="x">Angle X.</param>
        /// <param name="y">Angle Y.</param>
        /// <param name="z">Angle Z.</param>
        private void AddAngleToElement(XElement parentElement, string x, string y, string z)
        {
            parentElement.Add(new XElement("angleX", new XAttribute("value", x)));
            parentElement.Add(new XElement("angleY", new XAttribute("value", y)));
            parentElement.Add(new XElement("angleZ", new XAttribute("value", z)));
        }

        /// <summary>
        /// Checks whether element exists or not.
        /// </summary>
        /// <param name="path">Path without root.</param>
        /// <param name="errorMessage">Output error message.</param>
        /// <returns>True if element exists, otherwise false.</returns>
        private bool CheckElementsExistanceAndShowMessage(string path, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (MSystemXml.Root.ExistsElement(path))
            {
                errorMessage = $"Element in path {path} already exists!";
                return true;
            }
            return false;
        }

        #endregion
    }
}
