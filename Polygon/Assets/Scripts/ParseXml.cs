using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using Assets.Scripts.Class;
using Crosstales.FB;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class ParseXml : MonoBehaviour
    {

        private readonly DrawTile v_DrawTile = new DrawTile();
        private readonly DrawRod v_DrawRod = new DrawRod();

        private readonly List<XmlNode> v_SnapshotStepList = new List<XmlNode>();
        private static readonly List<XmlNode> v_TemporaryStepList = new List<XmlNode>();
        // ReSharper disable once CollectionNeverQueried.Local
        private static readonly List<NewSteps> v_NewStepList = new List<NewSteps>();

        // Dictionary rod thickens value
        private static readonly Dictionary<string, float> v_ThickensRodList = new Dictionary<string, float>();
        private static readonly Dictionary<string, string> v_ColorList = new Dictionary<string, string>();
        // ReSharper disable once CollectionNeverQueried.Local
        private static readonly Dictionary<string, BorderTileProperties> v_BorderColorList = new Dictionary<string, BorderTileProperties>();

        // If omitted color set default 4000bfff
        private const string c_DefaultColor = "4000bfff";
        // If omitted border color set default ff000000
        private const string c_DefaultBorderColor = "ff000000";
        // If omitted thickness set default 0.05f
        private const float c_DefaultThickness = 0.05f;
        private const float c_DefaultBorderThickness = 0.1f;
        // hold actual step number
        private int v_ActualStep;
        /// <summary>
        /// Unity start procedure
        /// </summary>
        [UsedImplicitly]
        private void Start()
        {
        }

        [UsedImplicitly]
        private void Update()
        {
            // Update Actual step number on screen.
            GameObject.Find("actualText").GetComponent<Text>().text = $"Actual step: {v_ActualStep}";
            // Next step
            if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                if (v_ActualStep < v_SnapshotStepList.Count - 1)
                {
                    if (v_ActualStep != v_SnapshotStepList.Count - 1)
                    {
                        DestroyAllGameObjects();
                        v_ActualStep++;
                        DrawStep(v_NewStepList[v_ActualStep]);
                        LogMessage("v_ActualStep (->)", $"Actual step is: {v_ActualStep} from {v_SnapshotStepList.Count - 1}");
                    }
                }
                else
                {
                    LogMessage("Update", "-> Nemam zadny krok pro provedeni");
                }
            }
            // Previous step
            else if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                if (v_ActualStep != 0)
                {
                    DestroyAllGameObjects();
                    v_ActualStep--;
                    DrawStep(v_NewStepList[v_ActualStep]);
                    LogMessage("v_ActualStep (<-) if", $"Actual step is: {v_ActualStep} from {v_SnapshotStepList.Count - 1}");
                }
                else
                {
                    LogMessage("Update", "<- Np next step");
                }
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                DestroyAllGameObjects();
                v_ActualStep = 0;
            }
            else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
#if !UNITY_EDITOR
                if (Input.GetKeyUp(KeyCode.O))
#else
                if (Input.GetKeyUp(KeyCode.F10))
#endif
                {
                    // Restore to default - START
                    DestroyAllGameObjects();
                    v_ActualStep = 0;
                    GameObject.Find("actualText").GetComponent<Text>().text = "Actual step: " + v_ActualStep;
                    v_NewStepList.Clear();
                    v_ColorList.Clear();
                    v_SnapshotStepList.Clear();
                    v_TemporaryStepList.Clear();
                    v_ThickensRodList.Clear();
                    // Restore to default - END

                    // open open file dialog.
                    string path = FileBrowser.OpenSingleFile("Open File", "", new[] { new ExtensionFilter("XML Files", "xml") });
                    //string path = new OpenFileDialogUnity.OpenFileDialogForUnity().GetOpenFileDialogForUnity(); // Old open file dialog
                    if (path != string.Empty)
                    {
                        // First step: Load LoadMSystemDescription.
                        LoadMSystemDescription(path);
                        // Second step: Load snapshot file and draw to environment.
                        LoadSnapshotFile(path);
                        // Set text to stepIDtext component in app
                        GameObject.Find("stepIDtext").GetComponent<Text>().text = $"SnapshotStep total: {v_SnapshotStepList.Count - 1}";
                        // Create New Steps to list
                        CreateNewSteps();
                        // draw first step in snapshot file
                        DrawStep(v_NewStepList[0]);
                    }
                }
            }
            else if (Input.GetKeyUp("escape"))
            {
                Application.Quit();
            }
            else if (Input.GetKeyUp(KeyCode.F1))
            {
                if (GameObject.Find("InfoText").GetComponent<Text>().enabled)
                {
                    GameObject.Find("InfoText").GetComponent<Text>().enabled = false;
                    GameObject.Find("O").GetComponent<Text>().enabled = false;
                    GameObject.Find("R").GetComponent<Text>().enabled = false;
                    GameObject.Find("ESC").GetComponent<Text>().enabled = false;
                    GameObject.Find("RightArrow").GetComponent<Text>().enabled = false;
                    GameObject.Find("LeftArrow").GetComponent<Text>().enabled = false;
                    GameObject.Find("Mouse").GetComponent<Text>().enabled = false;
                }
                else
                {
                    GameObject.Find("InfoText").GetComponent<Text>().enabled = true;
                    GameObject.Find("O").GetComponent<Text>().enabled = true;
                    GameObject.Find("R").GetComponent<Text>().enabled = true;
                    GameObject.Find("ESC").GetComponent<Text>().enabled = true;
                    GameObject.Find("RightArrow").GetComponent<Text>().enabled = true;
                    GameObject.Find("LeftArrow").GetComponent<Text>().enabled = true;
                    GameObject.Find("Mouse").GetComponent<Text>().enabled = true;
                }

            }

        }

        /// <summary>
        /// Method for Draw single step in world, decide between tile or rod 
        /// </summary>
        /// <param name="actualStep">Object NewSteps contains stepID, ListOfObjects</param>
        private void DrawStep(NewSteps actualStep)
        {
            List<ListObjects> listOfObjects = actualStep.ListOfObjects;
            foreach (var objectInList in listOfObjects)
            {
                string objectType = objectInList.ObjectType;
                switch (objectType)
                {
                    case "tile":
                        {
                            v_DrawTile.DrawOneTile(objectInList.ListOfVertexs, objectInList.ObjectId, objectInList.ObjectName, objectInList.NameColor);
                        }
                        break;
                    case "rod":
                        {
                            v_DrawRod.DrawOneRod(objectInList.ListOfVertexs[0], objectInList.ListOfVertexs[1], objectInList.ObjectId, objectInList.ObjectName, objectInList.Thickness, objectInList.NameColor);
                        }
                        break;
                    default:
                        LogMessage("FixedObjectSwitch", $" Default value not found : {objectType}");
                        break;
                }
            }
        }

        #region MSystemDescription

        /// <summary>
        /// Method for control if MSystemDescription file is correct.
        /// </summary>
        /// <param name="msystemPath">MSystemDescription file path.</param>
        private void LoadMSystemDescription(string msystemPath)
        {
            XmlDocument doc = new XmlDocument();

            doc.Load(msystemPath);
            if (doc.SelectSingleNode("root/MSystemDescription") == null)
            {
                LogMessage("MSystemDescription region", "Selected file is not Msystem file.");
            }

            XmlNodeList mSystemDescriptionTiles = doc.SelectNodes("root/MSystemDescription/tiling/tiles/tile");
            if (mSystemDescriptionTiles == null)
            {
                LogMessage("Snapshot region", "Missing 'snapshots' element or some elements on path.");
            }
            DeserializeMSystemDescriptionTiles(mSystemDescriptionTiles);
        }

        /// <summary>
        /// Deserialize MSystemDescription file.
        /// </summary>
        /// <param name="mSystemDescriptionTiles">XMLNodeList MSystemDescription (LoadMSystemDescription)</param>
        private void DeserializeMSystemDescriptionTiles(XmlNodeList mSystemDescriptionTiles)
        {
            foreach (XmlNode tile in mSystemDescriptionTiles)
            {
                if (tile.Attributes != null)
                {
                    string nameObject = tile.Attributes["name"].Value;
                    XmlNode thickness = tile.SelectSingleNode("thickness");
                    //                    if (thickness != null)
                    //                    {
                    //                        LogMessage("Object - thickness", "Thickness Value: " + thickness.Attributes["value"].Value);
                    //                    }
                    //                    else
                    //                    {
                    //                        LogMessage("Tile - thickness", "Není hodnota.");
                    //                    }

                    float thicknessValue = thickness != null ? float.Parse(thickness.Attributes["value"].Value, CultureInfo.InvariantCulture) : c_DefaultThickness;
                    LogMessage("Object - DeserializeMSystemDescriptionTiles", $"Name object: {nameObject} | thickness value: {thicknessValue}");
                    v_ThickensRodList.Add(nameObject, thicknessValue);

                    XmlNode color = tile.SelectSingleNode("color");
                    if (color?.Attributes != null)
                    {
                        string colorValue = color.Attributes["name"].Value;
                        LogMessage("Object - DeserializeMSystemDescriptionTiles", $"Name object: {nameObject} | color value: {colorValue}");
                        v_ColorList.Add(nameObject, colorValue);
                    }

                    XmlNode borderColor = tile.SelectSingleNode("bordercolor");
                    if (borderColor?.Attributes != null)
                    {
                        string borderColorValue = borderColor.Attributes["name"].Value ?? c_DefaultBorderColor;
                        string borderThickness = borderColor.Attributes["value"].Value;
                        float borderThicknessValue = borderThickness != null ? float.Parse(borderThickness, CultureInfo.InvariantCulture) : c_DefaultBorderThickness;
                        LogMessage("Object - DeserializeMSystemDescriptionTiles", $"Name object: {nameObject} | color value: {borderColorValue}");
                        v_BorderColorList.Add(nameObject, new BorderTileProperties(borderColorValue, borderThicknessValue));
                    }
                }
                else
                {
                    LogMessage("DeserializeMSystemDescriptionTiles", "No find tile attributes...");
                }
            }
        }
        #endregion

        #region SnapshotFile

        /// <summary>
        /// Load snapshot file
        /// input: SnapshotFile.xml
        /// output: procedure DeserializeSnapshotSteps
        /// </summary>
        private void LoadSnapshotFile(string msystemPath)
        {
            XmlDocument doc = new XmlDocument();

            doc.Load(msystemPath);
            if (doc.SelectSingleNode("root/snapshots") == null)
            {
                LogMessage("Load snapshot", "Snapshot regio: Selected file is not snapshot file.");
            }

            XmlNodeList snapshotSteps = doc.SelectNodes("root/snapshots/snapshot");
            if (snapshotSteps == null)
            {
                LogMessage("Load snapshot", "Snapshot region: Missing 'snapshots' element or some elements on path.");
            }

            if (snapshotSteps != null)
                foreach (XmlNode step in snapshotSteps)
                {
                    v_SnapshotStepList.Add(step);
                }
        }
        private void CreateNewSteps()
        {
            int actualStep = 0;

            // ReSharper disable once UnusedVariable
            foreach (XmlNode t in v_SnapshotStepList)
            {
                for (int j = 0; j <= actualStep; j++)
                {
                    v_TemporaryStepList.Add(v_SnapshotStepList[j]);
                }
                v_NewStepList.Add(new NewSteps(actualStep, AddToNewXmlStepList));
                v_TemporaryStepList.Clear();
                actualStep++;
            }
        }

        /// <summary>
        /// Create new world stepList, where one step contains each and others (explain step number 3 contains steps 0, 1, 2, 3, etc..)
        /// </summary>
        private static List<ListObjects> AddToNewXmlStepList
        {
            get
            {
                List<ListObjects> listOfObjects = new List<ListObjects>();
                foreach (XmlNode step in v_TemporaryStepList)
                {
                    if (step.Attributes != null)
                    {
                        int stepId = int.Parse(step.Attributes["stepID"].Value, CultureInfo.InvariantCulture);
                        //XmlNodeList fixedObjects = step.SelectNodes("fixedObjects/fixedObject");
                        XmlNodeList fixedObjects = step.SelectNodes("tiles/tile");
                        if (fixedObjects != null)
                            foreach (XmlNode fixedObject in fixedObjects)
                            {
                                if (fixedObject.Attributes != null)
                                {
                                    string state = fixedObject.Attributes["state"].Value;
                                    switch (state)
                                    {
                                        case "Create":
                                            {
                                                string objectId = fixedObject.Attributes["objectID"].Value;
                                                string objectName = fixedObject.Attributes["name"].Value;
                                                string objectType = fixedObject.Attributes["type"].Value;
                                                Color color = Color.magenta;
                                                XmlNodeList vertices = fixedObject.SelectNodes("vertices/vertex");
                                                List<Vector3> listOfVertexs = new List<Vector3>();
                                                if (vertices != null)
                                                {
                                                    listOfVertexs.AddRange(from XmlNode vertex in vertices let positionX = vertex.SelectSingleNode("posX") where positionX?.Attributes != null let posX = float.Parse(positionX.Attributes["value"].Value, CultureInfo.InvariantCulture) let positionY = vertex.SelectSingleNode("posY") where positionY != null && positionY.Attributes != null let posY = float.Parse(positionY.Attributes["value"].Value, CultureInfo.InvariantCulture) let positionZ = vertex.SelectSingleNode("posZ") where positionZ != null && positionZ.Attributes != null let posZ = float.Parse(positionZ.Attributes["value"].Value, CultureInfo.InvariantCulture) select new Vector3(posX, posY, posZ));
                                                }
                                                XmlNode colors = fixedObject.SelectSingleNode("color");
                                                if (colors != null)
                                                {
                                                    if (colors.Attributes != null)
                                                    {
                                                        string nameColor = colors.Attributes["name"].Value;
                                                        color = HexToColor(ControlColor(nameColor) ? nameColor : v_ColorList[objectName]);
                                                    }
                                                    else
                                                    {
                                                        color = HexToColor(c_DefaultColor);
                                                    }
                                                }
                                                //XmlNode bordercolor = fixedObject.SelectSingleNode("bordercolor");
                                                //if (bordercolor != null)
                                                //{
                                                //    if (bordercolor.Attributes != null)
                                                //    {
                                                //        string nameColor = bordercolor.Attributes["name"].Value;
                                                //        color = HexToColor(ControlColor(nameColor) ? nameColor : v_BorderColorList[objectName].Color);
                                                //    }
                                                //    else
                                                //    {
                                                //        color = HexToColor(c_DefaultColor);      
                                                //    }
                                                //}
                                                XmlNode thickness = fixedObject.SelectSingleNode("thickness");
                                                //<thickness value="0.2"/>
                                                float rodThickness = thickness != null ? float.Parse(thickness.Attributes["value"].Value, CultureInfo.InvariantCulture) : v_ThickensRodList[objectName];
                                                listOfObjects.Add(new ListObjects(stepId, objectId, objectName, objectType, listOfVertexs, color, rodThickness));
                                            }
                                            break;
                                        case "Move":
                                            {
                                                listOfObjects.RemoveAll(s => s.ObjectId == fixedObject.Attributes["objectID"].Value);

                                                string objectId = fixedObject.Attributes["objectID"].Value;
                                                string objectName = fixedObject.Attributes["name"].Value;
                                                string objectType = fixedObject.Attributes["type"].Value;
                                                Color color = Color.magenta;
                                                XmlNodeList vertices = fixedObject.SelectNodes("vertices/vertex");
                                                List<Vector3> listOfVertexs = new List<Vector3>();
                                                if (vertices != null)
                                                {
                                                    listOfVertexs.AddRange(from XmlNode vertex in vertices let positionX = vertex.SelectSingleNode("posX") where positionX?.Attributes != null let posX = float.Parse(positionX.Attributes["value"].Value, CultureInfo.InvariantCulture) let positionY = vertex.SelectSingleNode("posY") where positionY?.Attributes != null let posY = float.Parse(positionY.Attributes["value"].Value, CultureInfo.InvariantCulture) let positionZ = vertex.SelectSingleNode("posZ") where positionZ?.Attributes != null let posZ = float.Parse(positionZ.Attributes["value"].Value, CultureInfo.InvariantCulture) select new Vector3(posX, posY, posZ));
                                                }
                                                XmlNode colors = fixedObject.SelectSingleNode("color");
                                                if (colors != null)
                                                {
                                                    if (colors.Attributes != null)
                                                    {
                                                        string nameColor = colors.Attributes["name"].Value;
                                                        color = HexToColor(ControlColor(nameColor) ? nameColor : v_ColorList[objectName]);
                                                    }
                                                    else
                                                    {
                                                        color = HexToColor(c_DefaultColor);
                                                    }
                                                }
                                                XmlNode thickness = fixedObject.SelectSingleNode("thickness");
                                                //<thickness value="0.2"/>
                                                float rodThickness = thickness != null ? float.Parse(thickness.Attributes["value"].Value, CultureInfo.InvariantCulture) : v_ThickensRodList[objectName];
                                                listOfObjects.Add(new ListObjects(stepId, objectId, objectName, objectType, listOfVertexs, color, rodThickness));

                                            }
                                            break;
                                        case "Destroy":
                                            {
                                                listOfObjects.RemoveAll(s => s.ObjectId == fixedObject.Attributes["objectID"].Value);
                                            }
                                            break;
                                        default:
                                            LogMessage("FixedOjectSwitch", string.Format(" Default value not found : {0}", state));
                                            break;
                                    }
                                }
                            }
                    }
                }
                return listOfObjects;
            }
        }
        #endregion

        #region private methods

        /// <summary>
        /// Method for exception.
        /// </summary>
        /// <param name="module">Module name like (snapshot, inventory, etc).</param>
        /// <param name="message">Exception message.</param>
        private static void LogMessage(string module, string message)
        {
            Debug.Log(string.Format("{0}: {1}", module, message));
        }

        private static void DestroyAllGameObjects()
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag("ObjectInSpace");
            foreach (GameObject gameObject in objects)
                Destroy(gameObject);
        }

        /// <summary>
        /// Method for translate color from HEX syntax (RRGGBBAA) to RGBA 
        /// </summary>
        /// <param name="hex">Value can be like RRGGBBAA</param>
        /// <returns>Our color on RGB with Alpha value</returns>
        private static Color HexToColor(string hex)
        {
            byte a = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
            byte r = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(6, 2), NumberStyles.HexNumber);

            return new Color32(r, g, b, a);
        }

        /// <summary>
        /// Method for checking color in hex format.
        /// </summary>
        /// <param name="color">Color in hex format</param>
        /// <returns>Color status, if the color in hex format return value is True.</returns>
        private static bool ControlColor(string color)
        {
            const string pattern = "^([0-9a-fA-F]{2}){4}$";
            Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
            MatchCollection match = rgx.Matches(color);
            return match.Count > 0;
        }

        #endregion

        #region Structurec

        private struct BorderTileProperties
        {
            public BorderTileProperties(string color, float thickness) : this()
            {
                Color = color;
                Thickness = thickness;
            }

            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            private string Color { get; }
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            private float Thickness { get; }
        }

        #endregion
    }
}