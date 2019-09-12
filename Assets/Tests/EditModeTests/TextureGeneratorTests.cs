using NUnit.Framework;
using NUnit.Framework.Internal;
using UnityEngine;
using Random = System.Random;

namespace Tests.EditModeTests
{
    public class TextureGeneratorTests
    {
        private readonly Randomizer _random;
        private readonly float[,] _noiseMap;
        private readonly Color[] _colorMap;
        private readonly int _colorMapWidth;
        private readonly int _colorMapHeight;

        public TextureGeneratorTests()
        {
            _random = new Randomizer();
            _noiseMap = GenerateTestHeightMap();
            
            _colorMapWidth = _random.Next(2, 50);
            _colorMapHeight = _random.Next(2, 50);
            _colorMap = GenerateTestColorMap(_colorMapWidth, _colorMapHeight);
        }

        [Test]
        public void GivenHeightMap_WhenCallingTextureFromHeightMap_ThenReturnsATexture()
        {
            var textureResult = TextureGenerator.TextureFromHeightMap(_noiseMap);
            Assert.NotNull(textureResult);
        }

        [Test]
        public void GivenHeightMap_WhenCallingTextureFromHeightMap_ThenReturnsATextureOfTheExpectedSize()
        {
            var textureResult = TextureGenerator.TextureFromHeightMap(_noiseMap);

            var expectedWidth = _noiseMap.GetLength(0);
            var expectedHeight = _noiseMap.GetLength(1);
            
            Assert.AreEqual(expectedWidth, textureResult.width);
            Assert.AreEqual(expectedHeight, textureResult.height);
        }

        [Test]
        public void GivenColorMap_WhenCallingTextureFromColorMap_ThenReturnsATexture()
        {
            var textureResult = TextureGenerator.TextureFromColorMap(_colorMap, _colorMapWidth, _colorMapHeight);
            Assert.NotNull(textureResult);
        }

        [Test]
        public void GivenColorMap_WhenCallingTextureFromColorMap_ThenReturnsATextureOfTheExpectedSize()
        {
            var textureResult = TextureGenerator.TextureFromColorMap(_colorMap, _colorMapWidth, _colorMapHeight);

            Assert.AreEqual(_colorMapWidth, textureResult.width);
            Assert.AreEqual(_colorMapHeight, textureResult.height);
        }

        [Test]
        public void GivenHeightAndTerrainTypesInOrder_WhenCallingFindTerrainColor_ThenReturnCorrectColor()
        {
            var terrains = new TerrainType[]
            {
                new TerrainType {maxHeight = .25f, color = Color.blue},
                new TerrainType {maxHeight = .5f, color = Color.black},
                new TerrainType {maxHeight = .75f, color = Color.gray},
                new TerrainType {maxHeight = 1, color = Color.red}
            };
            var expectedColorBlue = TextureGenerator.FindTerrainColor(.2f, terrains);
            var expectedColorBlack = TextureGenerator.FindTerrainColor(.3f, terrains);
            var expectedColorGray = TextureGenerator.FindTerrainColor(.6f, terrains);
            var expectedColorRed = TextureGenerator.FindTerrainColor(.8f, terrains);
            
            Assert.AreEqual(Color.blue, expectedColorBlue);
            Assert.AreEqual(Color.black, expectedColorBlack);
            Assert.AreEqual(Color.gray, expectedColorGray);
            Assert.AreEqual(Color.red, expectedColorRed);
        }

        [Test]
        public void GivenHeightAndTerrainTypesOutOfOrder_WhenCallingFindTerrainColor_ThenReturnCorrectColor()
        {
            var terrains = new TerrainType[]
            {
                new TerrainType {maxHeight = .75f, color = Color.gray},
                new TerrainType {maxHeight = 1, color = Color.red},
                new TerrainType {maxHeight = .25f, color = Color.blue},
                new TerrainType {maxHeight = .5f, color = Color.black}
            };
            var expectedColorBlue = TextureGenerator.FindTerrainColor(.2f, terrains);
            var expectedColorBlack = TextureGenerator.FindTerrainColor(.3f, terrains);
            var expectedColorGray = TextureGenerator.FindTerrainColor(.6f, terrains);
            var expectedColorRed = TextureGenerator.FindTerrainColor(.8f, terrains);
            
            Assert.AreEqual(Color.blue, expectedColorBlue);
            Assert.AreEqual(Color.black, expectedColorBlack);
            Assert.AreEqual(Color.gray, expectedColorGray);
            Assert.AreEqual(Color.red, expectedColorRed);
        }

        [Test]
        public void GivenHeightAndTerrainTypes_WhenCallingFindTerrainColorAndHeightIsAtCutoff_ThenReturnCorrectColor()
        {
            var terrains = new TerrainType[]
            {
                new TerrainType {maxHeight = .75f, color = Color.gray},
                new TerrainType {maxHeight = 1, color = Color.red},
                new TerrainType {maxHeight = .25f, color = Color.blue},
                new TerrainType {maxHeight = .5f, color = Color.black}
            };
            var expectedColorRed = TextureGenerator.FindTerrainColor(1, terrains);
            
            Assert.AreEqual(Color.red, expectedColorRed);
        }

        [Test]
        public void GivenHeightAndTerrainTypes_WhenCallingFindTerrainColorAndHeightIsNotValid_ThenReturnWhite()
        {
            var terrains = new TerrainType[]
            {
                new TerrainType {maxHeight = .75f, color = Color.gray},
                new TerrainType {maxHeight = 1, color = Color.red},
                new TerrainType {maxHeight = .25f, color = Color.blue},
                new TerrainType {maxHeight = .5f, color = Color.black}
            };
            var expectedColorWhite = TextureGenerator.FindTerrainColor(33, terrains);
            
            Assert.AreEqual(Color.white, expectedColorWhite);
        }

        private float[,] GenerateTestHeightMap()
        {
            var mapWidth = _random.Next(2, 50);
            var mapHeight = _random.Next(2, 50);
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

        private Color[] GenerateTestColorMap(int colorMapWidth, int colorMapHeight)
        {

            var colorMap = new Color[colorMapHeight * colorMapWidth];
            for (var i = 0; i < colorMap.Length; i++)
            {
                colorMap[i] = new Color(_random.Next(0, 256), _random.Next(0, 256), _random.Next(0, 256));
            }

            return colorMap;
        }
    }
}
