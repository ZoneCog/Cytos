using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Cytos_v2.Exceptions;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using MSystemSimulationEngine.Classes.Tools;
using MSystemSimulationEngine.Interfaces;
using SharedComponents.XML;

namespace MSystemSimulationEngine.Classes.Xml
{
    /// <summary>
    /// Container for deserialized objects.
    /// </summary>
    public class DeserializedObjects : IDeserializedObjects
    {
        #region PrivateData

        private Dictionary<string, List<ProteinOnTile>> v_ProteinsOnTile = new Dictionary<string, List<ProteinOnTile>>();

        #endregion

        #region Public data

        /// <summary>
        /// Path to the M system description.
        /// </summary>
        /// <remarks>
        /// This parameter is used during snapshot serialization for inserting InnerXML into snapshot.
        /// </remarks>
        public string MSystemFilePath { get; private set; }

        /// <summary>
        /// List of floating objects.
        /// </summary>
        public readonly Dictionary<string, FloatingObject> FloatingObjects = new Dictionary<string, FloatingObject>();

        /// <summary>
        /// List of proteins.
        /// </summary>
        public readonly Dictionary<string, Protein> Proteins = new Dictionary<string, Protein>();

        /// <summary>
        /// List of glues.
        /// </summary>
        public readonly Dictionary<string, Glue> Glues = new Dictionary<string, Glue>();

        /// <summary>
        /// List of tiles.
        /// </summary>
        public readonly Dictionary<string, Tile> Tiles = new Dictionary<string, Tile>();

        /// <summary>
        /// List of initial objects used of simulation.
        /// </summary>
        public List<TileInSpace> SeedTiles { get; private set; }

        /// <summary>
        /// Glue radius of P system.
        /// </summary>
        public double GlueRadius { get; }

        public double TilingRandomMovement { get; }

        /// <summary>
        /// List of glue relation objects.
        /// </summary>
        public GlueRelation GluePRelation { get; private set; }

        /// <summary>
        /// List of evolution rules.
        /// </summary>
        public List<EvolutionRule> EvolutionRules { get; private set; }

        /// <summary>
        ///Battery electric voltage.
        /// </summary>
        public double Nu0 { get; }

        /// <summary>
        /// Electric threshold potential for tile connection.
        /// </summary>
        public double Tau { get; }

        /// <summary>
        /// Multiplicative coefficient of pushing distance.
        /// </summary>
        public double PushingCoef { get; }

        /// <summary>
        /// Scan connectors for attachment of new tiles in random order? 
        /// If false, connectors are grouped by tiles in order "as created".
        /// </summary>
        public readonly bool RandomizeConnectors;

        /// <summary>
        /// Keep constant concentration of environmental objects as the environment grows?
        /// </summary>
        public readonly bool RefillEnvironment;

        #endregion

        #region Constructor

        /// <summary>
        /// Deserialize input M System describtion into objects
        /// </summary>
        /// <param name="doc">XML document</param>
        /// <param name="filePath">Path to the XML Document</param>
        public DeserializedObjects(XDocument doc, string filePath)
        {
            //This parameter is used during snapshot serialization for inserting InnerXML into snapshot
            MSystemFilePath = filePath;

            XElement rootOfTilingDoc = doc.Element("root").Element("tiling");
            if (rootOfTilingDoc == null)
            {
                throw new MissingXmlElement("Missing 'tiling' element in tiling file.");
            }

            XElement rootOfMSystemDoc = doc.Element("root").Element("Msystem");
            if (rootOfMSystemDoc == null)
            {
                throw new MissingXmlElement("Missing 'Msystem' element in M system file.");
            }

            // Default constant values can be found here
            Nu0 = GetDouble(rootOfTilingDoc, "batteryVoltage", 0);
            Tau = GetDouble(rootOfTilingDoc, "thresholdPotential", 0);
            PushingCoef = GetDouble(rootOfTilingDoc, "pushingCoef", 2.2);
            if (PushingCoef < 1)
                throw new InvalidOperationException($"Pushing coefficient must be at least 1.");
            RandomizeConnectors = GetBool(rootOfTilingDoc, "randomizeConnectors", true);

            GlueRadius = GetDouble(rootOfTilingDoc, "glueRadius", 0.1);
            TilingRandomMovement = GetDouble(rootOfTilingDoc, "randomMovement", 0);

            DeserializeFloatingObjects(rootOfMSystemDoc.GetElements("floatingObjects/floatingObject"));
            RefillEnvironment = GetBool(rootOfMSystemDoc, "refillEnvironment", true);
            DeserializeProteins(rootOfMSystemDoc.GetElements("proteins/protein"));
            DeserializeProteinsOnTiles(rootOfMSystemDoc.GetElements("proteinsOnTiles/tile"));
            DeserializeGlues(rootOfTilingDoc.GetElements("glues/glue"));
            DeserializeTiles(rootOfTilingDoc.GetElements("tiles/tile").ToList());

            DeserializeGlueRelations(rootOfTilingDoc.GetElements("glueRelations/glueTuple"));
            DeserializeEvolutionRules(rootOfMSystemDoc.GetElements("evoRulesWithPriority/evoRule").ToList());

            DeserializeSignalObjects(rootOfMSystemDoc.GetElements("signalObjects/glueTuple"));
            DeserializeSeedTiles(rootOfTilingDoc.GetElements("initialObjects/initialObject"));
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Returns value of a deserialized optional XML element of type double
        /// </summary>
        /// <param name="parentElement">parent element in which the "name" is searched for</param>
        /// <param name="name">Name of the element.</param>
        /// <param name="defaultValue">return value if the element does not exist</param>
        /// <exception cref="InvalidOperationException">The name is already in use.</exception>
        private double GetDouble(XElement parentElement, string name, double defaultValue)
        {
            XElement element = parentElement.Element(name);
            if (element == null)
                return defaultValue;

            return Convert.ToDouble(Xmlizer.GetAttributeValueWithException(element, name, "value"), CultureInfo.InvariantCulture);
        }


        /// Returns value of a deserialized optional XML element of type double
        /// </summary>
        /// <param name="parentElement">parent element in which the "name" is searched for</param>
        /// <param name="name">Name of the element.</param>
        /// <param name="defaultValue">return value if the element does not exist</param>
        /// <exception cref="InvalidOperationException">The name is already in use.</exception>
        private bool GetBool(XElement parentElement, string name, bool defaultValue)
        {
            XElement element = parentElement.Element(name);
            if (element == null)
                return defaultValue;

            return Convert.ToBoolean(Xmlizer.GetAttributeValueWithException(element, name, "value"), CultureInfo.InvariantCulture);
        }


        /// <summary>
        /// Checks whether a newly deserialzed name of a tile, floating object, glue or protein
        /// is not already used by some of these entities.
        /// </summary>
        /// <param name="name">Name of the object.</param>
        /// <exception cref="InvalidOperationException">The name is already in use.</exception>
        private void AmbiguityTest(string name)
        {
            if (FloatingObjects.ContainsKey(name) || Tiles.ContainsKey(name) || Glues.ContainsKey(name) || Proteins.ContainsKey(name))
            {
                throw new InvalidOperationException($"Name '{name}' in the definition of P system is multiply used.");
            }
        }

        /// <summary>
        /// Gets ISimulation object based on its name.
        /// </summary>
        /// <param name="nameOfTheObject">Name of the object.</param>
        /// <returns>Simulation object.</returns>
        /// <exception cref="InvalidOperationException">If the object was not found.</exception>
        private ISimulationObject GetSimulationObject(string nameOfTheObject)
        {
            if (Tiles.ContainsKey(nameOfTheObject)) { return Tiles[nameOfTheObject]; }
            if (FloatingObjects.ContainsKey(nameOfTheObject)) { return FloatingObjects[nameOfTheObject]; }
            if (Proteins.ContainsKey(nameOfTheObject)) { return Proteins[nameOfTheObject]; }
            if (Glues.ContainsKey(nameOfTheObject)) { return Glues[nameOfTheObject]; }

            throw new InvalidOperationException(
                $"Simulation object named '{nameOfTheObject}' is not defined for tile, protein, glue or floating objects.");
        }

        /// <summary>
        /// Returns angle for given parent element
        /// </summary>
        /// <param name="angle">Angle element</param>
        /// <param name="defaultAngle">Default value if the XML node is missing.</param>
        /// <returns></returns>
        private Angle GetAngle(XElement angle, Angle defaultAngle)
        {
            if (angle == null)
            {
                return defaultAngle;
            }
            double angleValue = Convert.ToDouble(Xmlizer.GetAttributeValueWithException(angle, angle.Name.ToString(), "value"), CultureInfo.InvariantCulture);
            if (angle.Attribute("unit")?.Value == "rad")
            {
                return Angle.FromRadians(angleValue);
            }
            return Angle.FromDegrees(angleValue);
        }

        /// <summary>
        /// Deserialize color of a P system's object.
        /// </summary>
        /// <param name="colorElement">XML element of the color.</param>
        /// <param name="color">If the color node is null, the Black color is returned.</param>
        /// <param name="alpha">If no 'alpha' attribute is specified, color is fully opaque.</param>
        /// <exception cref="MissingXmlAttribute">If the color name is missing.</exception>
        /// <exception cref="InvalidOperationException">If the color name is not valid.</exception>
        private void DeserializeColor(XElement colorElement, out Color color, out int alpha)
        {
            color = Color.Black;
            alpha = 255;
            if (colorElement != null)
            {
                string colorName = Xmlizer.GetAttributeValueWithException(colorElement, "color", "name");
                color = Color.FromName(colorName);
                if (!color.IsNamedColor)
                {
                    throw new InvalidOperationException($"'{colorName}' is not a valid color name.");
                }

                XAttribute alphaAttribute = colorElement.Attribute("alpha");
                if (alphaAttribute != null)
                {
                    alpha = Convert.ToInt32(alphaAttribute.Value);
                }
            }
        }

        /// <summary>
        /// Deserialize given XML list of floating objects.
        /// </summary>
        /// <param name="floatingObjects">XML element list of floating objects.</param>
        /// <exception cref="MissingXmlAttribute">If some attribute of floating object is missing.</exception>
        /// <exception cref="MissingXmlElement">If some element of floating object is missing.</exception>
        private void DeserializeFloatingObjects(List<XElement> floatingObjects)
        {
            foreach (XElement floatingObject in floatingObjects)
            {
                string name = Xmlizer.GetAttributeValueWithException(floatingObject, "floatingObject", "name");
                AmbiguityTest(name);
                XElement mobilityElement = floatingObject.Element("mobility");
                double mobility = Convert.ToDouble(Xmlizer.GetAttributeValueWithException(mobilityElement, "mobility", "value"), CultureInfo.InvariantCulture);
                double concentration = GetDouble(floatingObject, "concentration", 0);

                FloatingObjects[name] = new FloatingObject(name, mobility, concentration);
            }
        }

        /// <summary>
        /// Deserialize given XML list of proteins.
        /// </summary>
        /// <param name="proteins">XML element list of proteins.</param>
        /// <exception cref="MissingXmlAttribute">If some attribute of protein is missing.</exception>
        /// <exception cref="MissingXmlElement">If some element of protein is missing.</exception>
        private void DeserializeProteins(List<XElement> proteins)
        {
            foreach (XElement protein in proteins)
            {
                string name = Xmlizer.GetAttributeValueWithException(protein, "protein", "name");
                AmbiguityTest(name);
                Proteins[name] = new Protein(name);
            }
        }

        /// <summary>
        /// Deserialize given XML list of proteins.
        /// </summary>
        /// <param name="tiles">XML element list of tiles wit proteins.</param>
        /// <exception cref="MissingXmlAttribute">If some attribute of protein is missing.</exception>
        /// <exception cref="MissingXmlElement">If some element of protein is missing.</exception>
        private void DeserializeProteinsOnTiles(List<XElement> tiles)
        {
            foreach (XElement tile in tiles)
            {
                string tileName = Xmlizer.GetAttributeValueWithException(tile, "tile", "name");
                List<ProteinOnTile> proteins = new List<ProteinOnTile>();
                foreach (XElement protein in tile.Elements("protein"))
                {
                    string name = Xmlizer.GetAttributeValueWithException(protein, "protein", "name");
                    Point3D position = DeserializePosition(protein);
                    proteins.Add(new ProteinOnTile(name, position));
                }
                v_ProteinsOnTile[tileName] = proteins;
            }
        }

        /// <summary>
        /// Deserialize given XML list of glues.
        /// </summary>
        /// <param name="glues">XML element list of glues.</param>
        /// <returns>Dictionary of glues.</returns>
        /// <exception cref="MissingXmlAttribute">If some attribute of glue is missing.</exception>
        /// <exception cref="MissingXmlElement">If some element of glue is missing.</exception>
        private void DeserializeGlues(List<XElement> glues)
        {
            foreach (XElement glue in glues)
            {
                string glueName = Xmlizer.GetAttributeValueWithException(glue, "glue", "name");
                Glues[glueName] = new Glue(glueName);
            }
        }

        /// <summary>
        /// Deserialize given XML list of tiles.
        /// </summary>
        /// <param name="tiles">XML element list of tiles.</param>
        /// <exception cref="MissingXmlAttribute">If some attribute of tile is missing.</exception>
        /// <exception cref="MissingXmlElement">If some element of tile is missing.</exception>
        private void DeserializeTiles(List<XElement> tiles)
        {
            foreach (XElement tile in tiles)
            {
                string name = Xmlizer.GetAttributeValueWithException(tile, "tile", "name");
                AmbiguityTest(name);
                Tiles[name] = DeserializeTile(name, tile);
            }
        }

        /// <summary>
        /// Deserialize tile from a given XML node.
        /// </summary>
        /// <param name="name">Tile name .</param>
        /// <param name="tileElement">XML element of tile.</param>
        /// <exception cref="MissingXmlAttribute">If some attribute of a tile is missing.</exception>
        /// <exception cref="MissingXmlElement">If some element of tile is missing.</exception>
        /// <returns>Deserialized tile.</returns>
        private Tile DeserializeTile(string name, XElement tileElement)
        {
            NamedVertices vertices;
            XElement verticesElement = tileElement.Element("vertices");
            if (verticesElement != null)
            {
                vertices = DeserializeVertices(verticesElement);
            }
            else
            {
                XElement polygonElement = tileElement.Element("polygon");
                if (polygonElement == null)
                {
                    throw new MissingXmlElement("Element 'vertices' or 'polygon' are missing. One of them is required.");
                }
                vertices = DeserializePolygon(polygonElement);
            }

            XElement positionsElement = tileElement.Element("positions");
            List<NamedPosition> namedPositions = new List<NamedPosition>();
            if (positionsElement != null)
            {
                List<XElement> positionElements = positionsElement.Elements("position").ToList();
                if (positionElements.Any())
                {
                    namedPositions = DeserializeNamedPositions(positionElements);
                }
            }
            
            Angle defaultAngle = GetAngle(tileElement.Element("connectingAngle"), default(Angle));

            List<Connector> connectors = new List<Connector>();
            List<XElement> connectorElements = tileElement.GetElements("connectors/connector");
            foreach (XElement connectorElement in connectorElements)
            {
                string connectorName = Xmlizer.GetAttributeValueWithException(connectorElement, "connector", "name");
                List<XElement> connectorsPositionElements = connectorElement.GetElements("positions/position");
                List<Point3D> positions = new List<Point3D>();
                foreach (XElement connectorsPositionElement in connectorsPositionElements)
                {
                    string positionName = Xmlizer.GetAttributeValueWithException(connectorsPositionElement, "position", "name");
                    NamedPosition vertex = vertices.FirstOrDefault(x => x.Name == positionName);
                    if (vertex.Name != null)
                    {
                        positions.Add(vertex.Position);
                    }
                    else
                    {
                        NamedPosition position = namedPositions.Find(x => x.Name == positionName);
                        if (position.Name != null)
                        {
                            positions.Add(position.Position);
                        }
                        else
                        {
                            throw new MissingXmlElement(
                                $"Vertex/Position '{positionName}' is not defined in input xml file for connector '{connectorName}'.");
                        }
                    }
                }
                XElement glueElement = connectorElement.Element("glue");
                string glueName = Xmlizer.GetAttributeValueWithException(glueElement, "glue", "name");

                if (!Glues.ContainsKey(glueName))
                {
                    throw new InvalidOperationException($"Glue named '{glueName}' is not defined in input xml file.");
                }
                Glue glue = Glues[glueName];
                Angle angle = GetAngle(connectorElement.Element("angle"), defaultAngle);

                double resistance = GetDouble(connectorElement, "resistance", 0);

                Connector connector = new Connector(connectorName, positions, glue, angle, resistance);
                connectors.Add(connector);
            }

            XElement surfaceGlueElement = tileElement.Element("surfaceGlue");
            string surfaceGlueName = Xmlizer.GetAttributeValueWithException(surfaceGlueElement, "surfaceGlue", "name");
            if (!Glues.ContainsKey(surfaceGlueName))
            {
                throw new InvalidOperationException($"Surface glue named '{surfaceGlueName}' is not defined in input xml file.");
            }

            Color color;
            int alpha;
            DeserializeColor(tileElement.Element("color"), out color, out alpha);

            List<ProteinOnTile> proteinsOnTile = null;
            if (v_ProteinsOnTile.ContainsKey(name))
            {
                proteinsOnTile = v_ProteinsOnTile[name];
            }

            double alphaRatio = GetDouble(tileElement, "alphaRatio", 1);
            if (alphaRatio <= 0)
                 throw new InvalidOperationException($"Alpha resistance ratio of the tile '{name}' must be positive.");

            return new Tile(name, vertices.Select(vertex => vertex.Position).ToList(), connectors, Glues[surfaceGlueName], proteinsOnTile, color, alpha, alphaRatio);
        }

        /// <summary>
        /// Deserialize given XML list of vertices of a tile.
        /// </summary>
        /// <param name="verticesElement">XML element list of vertices.</param>
        /// <returns>List of deserialized vertices.</returns>
        /// <exception cref="MissingXmlAttribute">If some attribute of protein is missing.</exception>
        /// <exception cref="MissingXmlElement">If some element of protein is missing.</exception>
        private NamedVertices DeserializeVertices(XElement verticesElement)
        {
            List<XElement> vertexElements = verticesElement.Elements("vertex").ToList();
            if (vertexElements.Count == 0)
            {
                throw new MissingXmlElement("Element vertex of vertices of tile is missing or empty.");
            }
            List<NamedPosition> vertices = new List<NamedPosition>();
            foreach (XElement vertexElement in vertexElements)
            {
                string name = Xmlizer.GetAttributeValueWithException(vertexElement, "vertex", "name");
                Point3D position = DeserializePosition(vertexElement);
                if (position.Z != 0)
                {
                    throw new InvalidOperationException("3D tiles with 'Z' coordinate not supported in this version.");
                }
                NamedPosition vertex = new NamedPosition(name, position);
                vertices.Add(vertex);
            }
            return new NamedVertices(vertices);
        }

        /// <summary>
        /// Deserialize given XML polygon node of fixed objecs.
        /// </summary>
        /// <param name="polygonElement">XML polygon element.</param>
        /// <returns>Tile vertices.</returns>
        /// <exception cref="MissingXmlAttribute">If some attribute of polygon is missing.</exception>
        /// <exception cref="MissingXmlElement">If some element of polygon is missing.</exception>
        private NamedVertices DeserializePolygon(XElement polygonElement)
        {
            XElement sidesElement = polygonElement.Element("sides");
            int sides = Convert.ToInt32(Xmlizer.GetAttributeValueWithException(sidesElement, "sides", "value"));

            XElement radiusElement = polygonElement.Element("radius");
            double radius = Convert.ToDouble(Xmlizer.GetAttributeValueWithException(radiusElement, "radius", "value"), CultureInfo.InvariantCulture);

            return new NamedVertices(sides, radius);
        }

        /// <summary>
        /// Deserialize given XML list of glue realations.
        /// </summary>
        /// <param name="glueRelations">XML element list of glue realations.</param>
        /// <exception cref="MissingXmlAttribute">If some attribute of floating object is missing.</exception>
        /// <exception cref="MissingXmlElement">If some element of floating object is missing.</exception>
        private void DeserializeGlueRelations(List<XElement> glueRelations)
        {
            GluePRelation = new GlueRelation();
            foreach (XElement glueRelation in glueRelations)
            {
                string nameOfTheGlue1 = Xmlizer.GetAttributeValueWithException(glueRelation, "glueTuple", "glue1");
                string nameOfTheGlue2 = Xmlizer.GetAttributeValueWithException(glueRelation, "glueTuple", "glue2");

                if (!Glues.ContainsKey(nameOfTheGlue1))
                {
                    throw new InvalidOperationException(string.Format("Glue named '{0}' is not defined in input xml file.",
                        nameOfTheGlue1));
                }

                if (!Glues.ContainsKey(nameOfTheGlue2))
                {
                    throw new InvalidOperationException(string.Format("Glue named '{0}' is not defined in input xml file.",
                        nameOfTheGlue2));
                }
                Glue glue1 = Glues[nameOfTheGlue1];
                Glue glue2 = Glues[nameOfTheGlue2];
                GluePRelation[Tuple.Create(glue1, glue2)] = new NamedMultiset();
            }
        }

        /// <summary>
        /// Deserialize given XML list of evolution rules.
        /// </summary>
        /// <param name="evolutionRules">XML list of evolution rules.</param>
        /// <exception cref="MissingXmlAttribute">If some attribute of floating object is missing.</exception>
        /// <exception cref="MissingXmlElement">If some element of floating object is missing.</exception>
        private void DeserializeEvolutionRules(List<XElement> evolutionRules)
        {
            EvolutionRules = new List<EvolutionRule>();
            foreach (XElement evolutionRule in evolutionRules)
            {
                string type = Xmlizer.GetAttributeValueWithException(evolutionRule, "evoRule", "type");
                int priority = Convert.ToInt32(evolutionRule.Attribute("priority")?.Value);
                int delay = Convert.ToInt32(evolutionRule.Attribute("delay")?.Value);
                XElement leftSideObject = evolutionRule.Element("leftside");
                if (leftSideObject == null)
                {
                    throw new MissingXmlElement("Element leftside of evoRule is missing");
                }
                char[] delimiter = { ',' };
                var leftSideObjectNames = Xmlizer.GetAttributeValueWithException(leftSideObject, "leftside", "value")
                    .Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

                XElement rightSideObject = evolutionRule.Element("rightside");
                if (rightSideObject == null)
                {
                    throw new MissingXmlElement("Element rightside of evoRule is missing");
                }
                var rightSideObjectNames = Xmlizer.GetAttributeValueWithException(rightSideObject, "rightside", "value")
                    .Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

                var leftSideObjects = leftSideObjectNames.Select(GetSimulationObject).ToList();
                var rightSideObjects = rightSideObjectNames.Select(GetSimulationObject).ToList();

                EvolutionRules.Add(EvolutionRule.NewRule(type, priority, leftSideObjects, rightSideObjects, delay));
            }
        }

        /// <summary>
        /// Deserialize given XML list of signal objects.
        /// </summary>
        /// <param name="glueTuples">XML element list of glue tuples.</param>
        /// <exception cref="MissingXmlAttribute">If some attribute of floating object is missing.</exception>
        /// <exception cref="MissingXmlElement">If some element of floating object is missing.</exception>
        private void DeserializeSignalObjects(List<XElement> glueTuples)
        {
            foreach (XElement glueTuple in glueTuples)
            {
                string nameOfTheGlue1 = Xmlizer.GetAttributeValueWithException(glueTuple, "glueTuple", "glue1");
                string nameOfTheGlue2 = Xmlizer.GetAttributeValueWithException(glueTuple, "glueTuple", "glue2");
                NamedMultiset signalObjects = new NamedMultiset();
                XElement objectElement = glueTuple.Element("objects");
                List<string> floatingObjectNames = Xmlizer.GetAttributeValueWithException(objectElement, "objects", "value").Split(',').ToList();
                foreach (string floatingObjectName in floatingObjectNames)
                {
                    if (!FloatingObjects.ContainsKey(floatingObjectName))
                    {
                        throw new InvalidOperationException(string.Format("Floating object named '{0}' is not defined in input xml file.",
                            floatingObjectName));
                    }
                    signalObjects.Add(floatingObjectName);
                }
                //TODO check if it find correct value
                GluePRelation[new Tuple<Glue, Glue>(Glues[nameOfTheGlue1], Glues[nameOfTheGlue2])] = signalObjects;
            }
        }

        /// <summary>
        /// Deserialize given XML list of initial objects.
        /// </summary>
        /// <param name="seedTiles">XML element list of tiles.</param>
        /// <exception cref="MissingXmlAttribute">If some attribute of seedTile is missing.</exception>
        /// <exception cref="MissingXmlElement">If some element of seedTile is missing.</exception>
        private void DeserializeSeedTiles(List<XElement> seedTiles)
        {
            SeedTiles = new List<TileInSpace>();
            foreach (XElement seedTile in seedTiles)
            {
                string name = Xmlizer.GetAttributeValueWithException(seedTile, "initialObject", "name");
                if (!Tiles.ContainsKey(name))
                {
                    throw new InvalidOperationException($"Seed tile named '{name}' is not defined in input xml file.");
                }
                List<Angle> angles = DeserializeAngles(seedTile);
                Point3D point = DeserializePosition(seedTile);
                SeedTiles.Add(new TileInSpace(Tiles[name], point, new EulerAngles(angles[0], angles[1], angles[2]).ToQuaternion()));
            }
        }

        /// <summary>
        /// Deserialize X, Y, Z angles of given parent element.
        /// </summary>
        /// <param name="parentElement">Parent element</param>
        /// <returns>List of angles - X, Y, Z. If any element does not exists it returns default angle instead.</returns>
        /// <exception cref="MissingXmlAttribute">If some attribute is missing.</exception>
        /// <exception cref="MissingXmlElement">If some element is missing.</exception>
        private List<Angle> DeserializeAngles(XElement parentElement)
        {
            List<Angle> angles = new List<Angle>
            {
                GetAngle(parentElement.Element("angleX"), default(Angle)),
                GetAngle(parentElement.Element("angleY"), default(Angle)),
                GetAngle(parentElement.Element("angleZ"), default(Angle))
            };

            return angles;
        }

        /// <summary>
        /// Deserialize X, Y, Z positions of given parent element.
        /// </summary>
        /// <param name="parentElement">Parent element</param>
        /// <returns>Point from deserialized values.</returns>
        private Point3D DeserializePosition(XElement parentElement)
        {
            double posX = GetDouble(parentElement, "posX", 0);
            double posY = GetDouble(parentElement, "posY", 0);
            double posZ = GetDouble(parentElement, "posZ", 0);

            return new Point3D(posX, posY, posZ);
        }

        /// <summary>
        /// Deserialize given XML list of named positions.
        /// </summary>
        /// <param name="positions">XML element list of positions.</param>
        /// <returns>List of deserialized positions.</returns>
        /// <exception cref="MissingXmlAttribute">If some attribute of position is missing.</exception>
        /// <exception cref="MissingXmlElement">If some element of position is missing.</exception>
        private List<NamedPosition> DeserializeNamedPositions(List<XElement> positions)
        {
            List<NamedPosition> namedPositions = new List<NamedPosition>();
            foreach (XElement position in positions)
            {
                string name = Xmlizer.GetAttributeValueWithException(position, "position", "name");
                namedPositions.Add(new NamedPosition(name, DeserializePosition(position)));
            }
            return namedPositions;
        }

        #endregion

        #region Public methods
        
        #endregion
    }
}
