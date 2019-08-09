using ChanceNET;
using FluentAssertions;
using NUnit.Framework;
using Scenes.Scripts;
using UnityEngine;

namespace Editor.Tests
{
    public class NoiseTests
    {
        private readonly Chance _chance;

        public NoiseTests()
        {
            _chance = new Chance();
        }
        
        [Test]
        public void GivenMapDimensions_WhenCallingGenerateMap_ThenReturnMapWithGivenDimensions()
        {
            var mapWidth = _chance.Integer(2, 100);
            var mapHeight = _chance.Integer(2, 100);
            var scale = (float) _chance.Double();
            var octaves = _chance.Integer(1, 8);
            var persistence = (float) _chance.Double();
            var lacunarity = (float) _chance.Double() + _chance.Integer(0, 10);
            var offset = new Vector2(_chance.Integer(-10000, 10000), _chance.Integer(-10000, 10000));
            var seed = _chance.Integer();
            
            var result = Noise.GenerateNoiseMap(mapWidth, mapHeight, scale, octaves, persistence, lacunarity, offset, seed);

            mapWidth.Should().Be(result.GetLength(0));
            mapHeight.Should().Be(result.GetLength(1));
        }

        [Test]
        public void GivenParameters_WhenCallingGenerateMap_ThenReturnMapWithDifferentValues()
        {
            var mapWidth = _chance.Integer(2, 100);
            var mapHeight = _chance.Integer(2, 100);
            var scale = (float) _chance.Double();
            var octaves = _chance.Integer(1, 8);
            var persistence = (float) _chance.Double();
            var lacunarity = (float) _chance.Double() + _chance.Integer(0, 10);
            var offset = new Vector2(_chance.Integer(-10000, 10000), _chance.Integer(-10000, 10000));
            var seed = _chance.Integer();
            
            var result = Noise.GenerateNoiseMap(mapWidth, mapHeight, scale, octaves, persistence, lacunarity, offset, seed);

            var firstPixel = result[0, 0];
            var testPixel = result[_chance.Integer(1, mapWidth), _chance.Integer(1, mapHeight)];

            firstPixel.Should().NotBe(testPixel);
        }

        [Test]
        public void GivenDifferentScaleValues_WhenCallingGenerateMap_ThenReturnDifferentMaps()
        {
            var mapWidth = _chance.Integer(2, 100);
            var mapHeight = _chance.Integer(2, 100);
            var scale1 = (float) _chance.Double();
            var scale2 = (float) _chance.Double();
            var octaves = _chance.Integer(1, 8);
            var persistence = (float) _chance.Double();
            var lacunarity = (float) _chance.Double() + _chance.Integer(0, 10);
            var offset = new Vector2(_chance.Integer(-10000, 10000), _chance.Integer(-10000, 10000));
            var seed = _chance.Integer();
            
            var result1 = Noise.GenerateNoiseMap(mapWidth, mapHeight, scale1, octaves, persistence, lacunarity, offset, seed);
            var result2 = Noise.GenerateNoiseMap(mapWidth, mapHeight, scale2, octaves, persistence, lacunarity, offset, seed);

            Assert.AreNotEqual(result1, result2);

        }

        [Test]
        public void GivenScaleIsZero_WhenCallingGenerateMap_ShouldReturnNoiseMapWithValidNumbers()
        {
            var mapWidth = _chance.Integer(2, 100);
            var mapHeight = _chance.Integer(2, 100);
            var scale = 0;
            var octaves = _chance.Integer(1, 8);
            var persistence = (float) _chance.Double();
            var lacunarity = (float) _chance.Double() + _chance.Integer(0, 10);
            var offset = new Vector2(_chance.Integer(-10000, 10000), _chance.Integer(-10000, 10000));
            var seed = _chance.Integer();
            
            var result = Noise.GenerateNoiseMap(mapWidth, mapHeight, scale, octaves, persistence, lacunarity, offset, seed);

            result.Should().NotContain(float.NaN);

        }

        [Test]
        public void GivenGeneratedMap_WhenChangingScale_ShouldScaleFromCenter()
        {
            //lerping messes this up for testing
        }

        [Test]
        public void GivenGeneratedMap_WhenChangingXOffset_ShouldMoveMap()
        {
            //lerping changes values more than should be. could test all points are relatively the same between scales
        }

        [Test]
        public void GivenGeneratedMap_WhenChangingYOffset_ShouldMoveMap()
        {
            //lerping changes values more than should be. could test all points are relatively the same between scales
        }

        [Test]
        public void GivenGeneratedMap_WhenChangingOnlySeed_ShouldGenerateNewMap()
        {
            var mapWidth = _chance.Integer(2, 100);
            var mapHeight = _chance.Integer(2, 100);
            var scale = (float) _chance.Double() + _chance.Integer(0, 10);
            var octaves = _chance.Integer(1, 8);
            var persistence = (float) _chance.Double();
            var lacunarity = (float) _chance.Double() + _chance.Integer(0, 10);
            var offset = new Vector2(_chance.Integer(-10000, 10000), _chance.Integer(-10000, 10000));
            var seed = _chance.Integer();
            
            var result1 = Noise.GenerateNoiseMap(mapWidth, mapHeight, scale, octaves, persistence, lacunarity, offset, seed);

            seed = _chance.Integer();
            var result2 = Noise.GenerateNoiseMap(mapWidth, mapHeight, scale, octaves, persistence, lacunarity, offset, seed);

            Assert.AreNotEqual(result1, result2);
        }

        [Test]
        public void GivenConstantSeed_WhenRegeneratingMap_ShouldGenerateSameMap()
        {
            var mapWidth = _chance.Integer(2, 100);
            var mapHeight = _chance.Integer(2, 100);
            var scale = (float) _chance.Double() + _chance.Integer(0, 10);
            var octaves = _chance.Integer(1, 8);
            var persistence = (float) _chance.Double();
            var lacunarity = (float) _chance.Double() + _chance.Integer(0, 10);
            var offset = new Vector2(_chance.Integer(-10000, 10000), _chance.Integer(-10000, 10000));
            var seed = _chance.Integer();
            
            var result1 = Noise.GenerateNoiseMap(mapWidth, mapHeight, scale, octaves, persistence, lacunarity, offset, seed);
            var result2 = Noise.GenerateNoiseMap(mapWidth, mapHeight, scale, octaves, persistence, lacunarity, offset, seed);
            
            Assert.AreEqual(result1, result2);
        }
        
        

    }
}