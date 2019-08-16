using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSystemSimulationEngine.Classes;
using SharedComponents.Tools;

namespace Cytos_v2_Tests.Classes.Tools
{
    [TestClass]
    public class RandomizerTest
    {
        [TestMethod]
        public void GetRandomItemFromCollectionTest()
        {
            Protein protein1 = new Protein("protein1");
            Protein protein2 = new Protein("protein2");
            var listOfProteins = new List<Protein> {protein1, protein2};
            Protein protein = listOfProteins.GetRandomItem();
            CollectionAssert.Contains(listOfProteins, protein);
        }

        [TestMethod]
        public void ShuffleTest()
        {
            var testList = Enumerable.Range(0, 10).ToArray();
            testList.Shuffle();
            for (int i=1; i < 10; i++)
                CollectionAssert.Contains(testList, i);
        }
    }
}
