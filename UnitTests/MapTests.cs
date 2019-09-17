using DerivcoTestTask.Model;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace UnitTests
{
    public class MapTests
    {
        [Test]
        public void FillProcessedMapTest()
        {
            var tests = new Dictionary<string, Square[,]>
            {
                { "##\n#O", new [,] {
                    { new Square(0, 0, SquareType.Land), new Square(1, 0, SquareType.Land) },
                    { new Square(0, 1, SquareType.Land), new Square(1, 1, SquareType.Water, 1) }
                } },
                { "###\n#OO\n#O#", new [,] {
                    { new Square(0, 0, SquareType.Land), new Square(1, 0, SquareType.Land), new Square(2, 0, SquareType.Land) },
                    { new Square(0, 1, SquareType.Land), new Square(1, 1, SquareType.Water, 1), new Square(2, 1, SquareType.Water, 1) },
                    { new Square(0, 2, SquareType.Land), new Square(1, 2, SquareType.Water, 1), new Square(2, 2, SquareType.Land) },
                } },
                { "##o#\n#O#\n#o##", new [,] {
                    { new Square(0, 0, SquareType.Land), new Square(1, 0, SquareType.Land), new Square(2, 0, SquareType.Land) },
                    { new Square(0, 1, SquareType.Land), new Square(1, 1, SquareType.Water, 1), new Square(2, 1, SquareType.Land) },
                    { new Square(0, 2, SquareType.Land), new Square(1, 2, SquareType.Land), new Square(2, 2, SquareType.Land) },
                } },
                { "##\n##\n##\n", new [,] {
                    { new Square(0, 0, SquareType.Land), new Square(1, 0, SquareType.Land) },
                    { new Square(0, 1, SquareType.Land), new Square(1, 1, SquareType.Land) },
                    { new Square(0, 2, SquareType.Land), new Square(1, 2, SquareType.Land) },
                } },
            };
            var results = new Dictionary<Square[,], Square[,]>();

            foreach (var item in tests)
            {
                var expectedMap = item.Value;
                var actualMap = new Map(item.Key);
                actualMap.Proceed();
                results.Add(expectedMap, actualMap.Processed);
            }

            foreach (var item in results)
            {
                Assert.AreEqual(item.Key, item.Value);
            }
        }

        [Test]
        public void GetWaterSourcesTest()
        {
            var tests = new Dictionary<string, List<Square>>
            {
                { "##\n#O", new List<Square> { new Square(1, 1, 1)} },
                { "###\n#OO\n#O#", new List<Square> { new Square(1, 1, 3)} },
                { "##o#\n#O#\n#o##", new List<Square> { new Square(1, 1, 1)} },
                { "##\n##\n##\n", new List<Square>() },
                { "#O\n##\nO#\n", new List<Square> { new Square(1, 0, 1), new Square(0, 2, 1)} },
                { "####\n##O#\n#OO#\n####", new List<Square> { new Square(1, 2, 3)} },
            };

            foreach (var item in tests)
            {
                WaterSourcesTest(item.Key, item.Value);
            }
        }

        private void WaterSourcesTest(string rawMap, List<Square> squares)
        {
            var map = new Map(rawMap);
            map.Proceed();
            var waterSources = map.GetWaterSources();

            foreach(var expectedItem in squares)
            {
                var actualItem = waterSources.FirstOrDefault(s => s.X == expectedItem.X && s.Y == expectedItem.Y);

                Assert.NotNull(actualItem);
                Assert.AreEqual(expectedItem.SurfaceArea, actualItem.SurfaceArea);
            }
            
        }

        [TestCase("##\n#O", true)]
        [TestCase("##\n#O#", false)]
        [TestCase("#s#\n##", true)]
        [TestCase("", false)]
        public void ProceedTest(string rawMap, bool isPassed)
        {
            var map = new Map(rawMap);

            var result = map.Proceed();

            Assert.AreEqual(isPassed, result is null);
        }
    }
}