using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSystemSimulationEngine.Classes;
using MSystemSimulationEngine.Interfaces;
using SharedComponents.Tools;

namespace Cytos_v2_Tests.Classes
{
    [TestClass]
    public class EvolutionRuleTest
    {
        [TestMethod]
        public void TestNewRuleReturnsCorrectMetabolicRule()
        {
            FloatingObject floatingObject = new FloatingObject("a", 1, 1);
            Protein protein = new Protein("p1");
            List<ISimulationObject> leftSide = new List<ISimulationObject> {floatingObject, protein};
            List<ISimulationObject> rightSide = new List<ISimulationObject> {protein, floatingObject};
            EvoMetabolicRule metabolicRule = TypeUtil.Cast<EvoMetabolicRule>(EvolutionRule.NewRule("Metabolic", 0, leftSide, rightSide, 0));

            Assert.AreEqual(0, metabolicRule.Priority);
            Assert.AreEqual(EvolutionRule.RuleType.Metabolic, metabolicRule.Type);
            Assert.AreEqual(EvoMetabolicRule.MetabolicRuleType.Symport, metabolicRule.SubType);
            Assert.AreEqual(2, metabolicRule.LeftSideObjects.Count);
            Assert.AreEqual("a", metabolicRule.LeftSideObjects[0].Name);
            Assert.AreEqual("p1", metabolicRule.LeftSideObjects[1].Name);
            Assert.AreEqual(2, metabolicRule.RightSideObjects.Count);
            Assert.AreEqual("p1", metabolicRule.RightSideObjects[0].Name);
            Assert.AreEqual("a", metabolicRule.RightSideObjects[1].Name);
            Assert.AreEqual(0, metabolicRule.MLeftInNames.Count);
            Assert.AreEqual(1, metabolicRule.MLeftOutNames.Count);
            Assert.AreEqual(1, metabolicRule.MLeftOutNames.ToDictionary()["a"]);
            Assert.AreEqual(1, metabolicRule.MRightInNames.Count);
            Assert.AreEqual(1, metabolicRule.MRightInNames.ToDictionary()["a"]);
            Assert.AreEqual(0, metabolicRule.MRightOutNames.Count);
            Assert.AreEqual("p1", metabolicRule.RProtein.Name);
        }

        [TestMethod]
        public void TestNewRuleReturnsCorrectNonMetabolicRule()
        {
            Glue glue = new Glue("pa");
            FloatingObject floatingObject = new FloatingObject("a", 1, 1);
            List<ISimulationObject> leftSide = new List<ISimulationObject> { glue, glue, floatingObject, floatingObject };
            List<ISimulationObject> rightSide = new List<ISimulationObject> { glue, glue };
            EvoNonMetabolicRule divideRule = TypeUtil.Cast<EvoNonMetabolicRule>(EvolutionRule.NewRule("Divide", 1, leftSide, rightSide, 0));

            Assert.AreEqual(1, divideRule.Priority);
            Assert.AreEqual(EvolutionRule.RuleType.Divide, divideRule.Type);
            Assert.AreEqual(4, divideRule.LeftSideObjects.Count);
            Assert.AreEqual("pa", divideRule.LeftSideObjects[0].Name);
            Assert.AreEqual("pa", divideRule.LeftSideObjects[1].Name);
            Assert.AreEqual("a", divideRule.LeftSideObjects[2].Name);
            Assert.AreEqual("a", divideRule.LeftSideObjects[3].Name);
            Assert.AreEqual(2, divideRule.RightSideObjects.Count);
            Assert.AreEqual("pa", divideRule.RightSideObjects[0].Name);
            Assert.AreEqual("pa", divideRule.RightSideObjects[1].Name);
            Assert.AreEqual(2, divideRule.MLeftSideFloatingNames.Count);
            Assert.AreEqual(2, divideRule.MLeftSideFloatingNames.ToDictionary()["a"]);
            Assert.AreEqual(0, divideRule.MRightSideFloatingNames.Count);
        }

        [TestMethod]
        public void TestToStringReturnsCompleteString()
        {
            FloatingObject floatingObject = new FloatingObject("a", 1, 1);
            Protein protein = new Protein("p1");
            List<ISimulationObject> leftSide = new List<ISimulationObject> { floatingObject, protein };
            List<ISimulationObject> rightSide = new List<ISimulationObject> { protein, floatingObject };
            EvolutionRule metabolicRule = EvolutionRule.NewRule("Metabolic", 0, leftSide, rightSide, 0);
            string expectedOutput = "Rule: a,p1 -> p1,a, type = Metabolic, priority = 0";
            Assert.AreEqual(expectedOutput, metabolicRule.ToString());
        }
    }
}
