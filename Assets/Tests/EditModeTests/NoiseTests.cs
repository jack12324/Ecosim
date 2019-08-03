using NUnit.Framework;
using NUnit.Framework.Internal;
using Scenes.Scripts;

namespace Editor.Tests
{
    public class NoiseTests
    {
        private readonly Randomizer _randomizer;

        public NoiseTests()
        { 
            _randomizer = new Randomizer();
        }
        
        [Test]
        public void GivenMapDimensions_WhenCallingGenerateMap_ThenReturnMapWithGivenDimensions()
        {
            var mapWidth = _randomizer.Next(2, 100);
            var mapHeight = _randomizer.Next(2, 100);
            var scale = _randomizer.NextFloat();

            var result = Noise.GenerateNoiseMap(mapWidth, mapHeight, scale);
            
            Assert.AreEqual(mapWidth, result.GetLength(0));
            Assert.AreEqual(mapHeight, result.GetLength(1));
        }

        [Test]
        public void GivenParameters_WhenCallingGenerateMap_ThenReturnMapWithDifferentValues()
        {
            var mapWidth = _randomizer.Next(2, 100);
            var mapHeight = _randomizer.Next(2, 100);
            var scale = _randomizer.NextFloat();
            
            var result = Noise.GenerateNoiseMap(mapWidth, mapHeight, scale);

            var firstPixel = result[0, 0];
            var testPixel = result[_randomizer.Next(1, mapWidth), _randomizer.Next(1, mapHeight)];
            
            Assert.AreNotEqual(firstPixel, testPixel );
        }

        [Test]
        public void GivenDifferentScaleValues_WhenCallingGenerateMap_ThenReturnDifferentMaps()
        {
            var mapWidth = _randomizer.Next(2, 100);
            var mapHeight = _randomizer.Next(2, 100);
            var scale1 = _randomizer.NextFloat();
            var scale2 = _randomizer.NextFloat();
            
            var result1 = Noise.GenerateNoiseMap(mapWidth, mapHeight, scale1);
            var result2 = Noise.GenerateNoiseMap(mapWidth, mapHeight, scale2);
            
            Assert.AreNotEqual(result1, result2 );
            
        }

    }
}