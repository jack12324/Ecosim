using NUnit.Framework;
using NUnit.Framework.Internal;
using UnityEngine;

namespace Tests.EditModeTests
{
    public class MapDisplayControllerTests
    {
        private readonly Randomizer _random;
        private readonly float[,] _noiseMap;
        private readonly MapDisplayController _sut;

        public MapDisplayControllerTests()
        {
            _random = new Randomizer();
            _noiseMap = GenerateTestNoiseMap();
            _sut = new MapDisplayController();
        }

        [Test]
        public void GivenNoiseMap_WhenCallingGenerateTexture_ThenReturnsATexture()
        {
            var textureResult = _sut.GenerateTexture(_noiseMap);
            Assert.NotNull(textureResult);
        }

        [Test]
        public void GivenNoiseMap_WhenCallingGenerateTexture_ThenReturnsATextureOfTheExpectedSize()
        {
            var textureResult = _sut.GenerateTexture(_noiseMap);

            var expectedWidth = _noiseMap.GetLength(0);
            var expectedHeight = _noiseMap.GetLength(1);
            
            Assert.AreEqual(expectedWidth, textureResult.width);
            Assert.AreEqual(expectedHeight, textureResult.height);
        }

        [Test]
        public void GivenNoiseMap_WhenCallingGenerateTexture_ThenReturnsATextureWithBlackAndWhitePixels()
        {
            var textureResult = _sut.GenerateTexture(_noiseMap);
            var pixels = textureResult.GetPixels();

            foreach (var pixel in pixels)
            {
                Assert.AreEqual(pixel.r, pixel.g);
                Assert.AreEqual(pixel.r, pixel.b);
            }   
        }

        private float[,] GenerateTestNoiseMap()
        {
            var mapWidth = _random.Next(2, 15);
            var mapHeight = _random.Next(2, 15);
            var scale = _random.NextFloat();
            
            var noiseMap = new float[mapWidth, mapHeight];

            for (var y = 0; y < mapHeight; y++)
            {
                var ySample = y / scale;
                for (var x = 0; x < mapWidth; x++)
                { 
                    var xSample = x / scale;
                    var perlinNoise = Mathf.PerlinNoise(xSample, ySample);

                    noiseMap[x, y] = perlinNoise;
                }
            }

            return noiseMap;
        }
    }
}
