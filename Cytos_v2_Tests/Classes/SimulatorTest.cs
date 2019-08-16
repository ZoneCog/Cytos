using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;
using MathNet.Spatial.Euclidean;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSystemSimulationEngine.Classes;
using MSystemSimulationEngine.Classes.Xml;

namespace Cytos_v2_Tests.Classes
{
    internal class SimulatorFacade : Simulator
    {
        #region Constructor

        internal SimulatorFacade(DeserializedObjects mSystemObjects) :
            base(mSystemObjects, false) {}

        #endregion

        internal new bool ApplyMetabolicRules() => base.ApplyMetabolicRules().Any();
        internal new bool ApplyDestructionRules() => base.ApplyDestructionRules().Any();
        internal new bool ApplyDivisionRules() => base.ApplyDivisionRules().Any();
        internal new bool ApplyCreationRules() => base.ApplyCreationRules().Any();
        internal new void FinalizeStep() => base.FinalizeStep();
        internal FloatingObjectsWorld TestFltObjectsWorld => FltObjectsWorld;
        internal TilesWorld TestTilesWorld => TilesWorld;
    }


    [TestClass]
    public class SimulatorTest
    {
        private static MSystem _testMSystem;
        private static DeserializedObjects _testDeserializedMSystemObjects;
        private static SimulatorFacade _testSimulator;
        private static string _path;
        private static XDocument _mSystemDescription;
        private static readonly NamedMultiset InfiniteMultiset = new NamedMultiset(new Dictionary<string, int>
        {
            {"a", NamedMultiset.Infinity},
            {"b", NamedMultiset.Infinity},
            {"c", NamedMultiset.Infinity}
        });


        [ClassInitialize]
        public static void InitializeClass(TestContext context)
        {
            _path = @"../../Classes/Xml/TestXML/testMSystemDescription.xml";
            _mSystemDescription = XDocument.Load(_path);
        }

        [TestInitialize]
        //Initialize object/s before each test is run. It will be called for each test.
        public void Initialize()
        {
            _testDeserializedMSystemObjects = new DeserializedObjects(_mSystemDescription, _path);
            _testMSystem = new MSystem(_testDeserializedMSystemObjects);
            _testSimulator = new SimulatorFacade(_testDeserializedMSystemObjects);
        }

        // TODO this tests only the single symport rule in the evolution file. Add antiport and catalytic rules and corresponding test methods
        [TestMethod]
        public void MetabolicRulesTest()
        {
            Assert.IsFalse(_testSimulator.ApplyMetabolicRules());

            var aObject = _testMSystem.FloatingObjects["a"];
            _testSimulator.TestFltObjectsWorld.AddAt(new NamedMultiset(new[] { aObject, aObject }), new Point3D(0, 0, -0.5));
            _testSimulator.TestFltObjectsWorld.FinalizeStep(true);
            Assert.IsTrue(_testSimulator.ApplyMetabolicRules());

            _testSimulator.TestFltObjectsWorld.FinalizeStep(true);
            var testTile = _testSimulator.TestTilesWorld.First();
            var detectedObjects = _testSimulator.TestFltObjectsWorld.GetNearObjects(testTile.Proteins.First(), Tile.SideType.outside, InfiniteMultiset);
            Assert.AreEqual(0, detectedObjects.Count(obj => obj.Name == "a"));

            detectedObjects = _testSimulator.TestFltObjectsWorld.GetNearObjects(testTile.Proteins.First(), Tile.SideType.inside, InfiniteMultiset);
            Assert.AreEqual(2, detectedObjects.Count(obj => obj.Name == "a"));
        }

        [TestMethod]
        public void DestructionRulesTest()  // Tests also the FinalizeStep method
        {
            var baseTile = _testSimulator.TestTilesWorld.First();
            var tubule = _testMSystem.Tiles["s1"];
            _testSimulator.TestTilesWorld.Add(tubule, baseTile.Connectors[5]);
            var verticalTubule = _testSimulator.TestTilesWorld.First(obj => obj.Name == "s1");

            Assert.IsFalse(_testSimulator.ApplyDestructionRules());

            var aObject = _testMSystem.FloatingObjects["a"];
            _testSimulator.TestFltObjectsWorld.AddAt(new NamedMultiset(new[] { aObject }), verticalTubule.Position);
            _testSimulator.TestFltObjectsWorld.FinalizeStep(true);
            Assert.IsFalse(_testSimulator.ApplyDestructionRules());

            _testSimulator.TestFltObjectsWorld.AddAt(new NamedMultiset(new[] { aObject }), verticalTubule.Position);
            _testSimulator.TestFltObjectsWorld.FinalizeStep(true);
            Assert.IsTrue(_testSimulator.ApplyDestructionRules());

            _testSimulator.TestFltObjectsWorld.FinalizeStep(true);
            var detectedObjects = _testSimulator.TestFltObjectsWorld.GetNearObjects(verticalTubule, InfiniteMultiset);
            Assert.AreEqual(2, detectedObjects.Count(obj => obj.Name == "c"));

            _testSimulator.FinalizeStep();
            CollectionAssert.DoesNotContain(_testSimulator.TestTilesWorld.ToArray(), verticalTubule);
        }

        [TestMethod]
        public void DivisionRulesTest()  // Tests also the FinalizeStep method
        {
            var baseTile = _testSimulator.TestTilesWorld.First();
            var tubule = _testMSystem.Tiles["s1"];
            _testSimulator.TestTilesWorld.Add(tubule, baseTile.Connectors[5]);
            var verticalTubule = _testSimulator.TestTilesWorld.Last(obj => obj.Name == "s1");

            Assert.AreEqual(baseTile.Connectors[5], verticalTubule.Connectors[0].ConnectedTo);
            Assert.IsFalse(_testSimulator.ApplyDivisionRules());

            var aObject = _testMSystem.FloatingObjects["a"];
            _testSimulator.TestFltObjectsWorld.AddAt(new NamedMultiset(new[] { aObject }), verticalTubule.Connectors[0].Positions[0]);
            _testSimulator.TestFltObjectsWorld.FinalizeStep(true);
            Assert.IsFalse(_testSimulator.ApplyDivisionRules());

            _testSimulator.TestFltObjectsWorld.AddAt(new NamedMultiset(new[] { aObject }), verticalTubule.Connectors[0].Positions[0]);
            _testSimulator.TestFltObjectsWorld.FinalizeStep(true);
            Assert.IsTrue(_testSimulator.ApplyDivisionRules());

            _testSimulator.FinalizeStep();
            Assert.IsNull(verticalTubule.Connectors[0].ConnectedTo);
        }

        [TestMethod]
        public void CreationRulesTest()
        {
            var baseTile = _testSimulator.TestTilesWorld.First();
            Assert.IsFalse(_testSimulator.ApplyCreationRules());

            var a = _testMSystem.FloatingObjects["a"];
            _testSimulator.TestFltObjectsWorld.AddAt(new NamedMultiset(new[] { a, a }), baseTile.Connectors[0].Positions[0]);
            _testSimulator.TestFltObjectsWorld.FinalizeStep(true);
            Assert.IsFalse(_testSimulator.ApplyCreationRules());

            _testSimulator.TestFltObjectsWorld.AddAt(new NamedMultiset(new[] { a }), baseTile.Connectors[0].Positions[0]);
            _testSimulator.TestFltObjectsWorld.FinalizeStep(true);
            Assert.IsTrue(_testSimulator.ApplyCreationRules());

            Assert.AreEqual(4, _testSimulator.TestTilesWorld.Count());
            _testSimulator.TestFltObjectsWorld.AddAt(new NamedMultiset(new[] { a, a, a }), baseTile.Connectors[0].Positions[0]);

            // Add six objects to test whether only free connectors are used to try to add new object
            _testSimulator.TestFltObjectsWorld.AddAt(new NamedMultiset(new[] { a, a, a, a, a, a }), baseTile.Connectors[0].Positions[0]);
            _testSimulator.TestFltObjectsWorld.FinalizeStep(true);
            Assert.IsTrue(_testSimulator.ApplyCreationRules());
            Assert.AreEqual(5, _testSimulator.TestTilesWorld.Count());

            foreach (var obj in _testSimulator.TestTilesWorld.Where(obj => obj.Name=="q1"))
                Assert.AreEqual(2, obj.Connectors.Count(conn => conn.ConnectedTo != null));
        }

        [TestMethod]
        public void SimulationStepFinalyzeMethodResetColorWasChangedFlag()
        {
            var baseTile = _testSimulator.TestTilesWorld.First();
            var tubule = _testMSystem.Tiles["s1"];
            _testSimulator.TestTilesWorld.Add(tubule, baseTile.Connectors[5]);

            TileInSpace[] tilesInSpace = _testSimulator.TestTilesWorld.ToArray();

            Assert.AreEqual(4, tilesInSpace.Length);
            foreach (TileInSpace tileInSpace in tilesInSpace)
            {
                Assert.IsFalse(tileInSpace.ColorWasChanged);
            }

            foreach (TileInSpace tileInSpace in tilesInSpace)
            {
                tileInSpace.SetNewColor(Color.Green);
            }

            foreach (TileInSpace tileInSpace in tilesInSpace)
            {
                Assert.IsTrue(tileInSpace.ColorWasChanged);
            }

            _testSimulator.FinalizeStep();

            foreach (TileInSpace tileInSpace in tilesInSpace)
            {
                Assert.IsFalse(tileInSpace.ColorWasChanged);
            }
        }

    }
}
