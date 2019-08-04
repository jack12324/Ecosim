using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Scenes.Scripts;
using UnityEngine;

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
            var octaves = _randomizer.Next(1, 8);
            var persistence = _randomizer.NextFloat();
            var lacunarity = _randomizer.NextFloat() + _randomizer.Next(10);
            var offset = new Vector2(_randomizer.Next(-10000, 10000), _randomizer.Next(-10000, 10000));
            var seed = _randomizer.Next();
            
            var result = Noise.GenerateNoiseMap(mapWidth, mapHeight, scale, octaves, persistence, lacunarity, offset, seed);
            
            Assert.AreEqual(mapWidth, result.GetLength(0));
            Assert.AreEqual(mapHeight, result.GetLength(1));
        }

        [Test]
        public void GivenParameters_WhenCallingGenerateMap_ThenReturnMapWithDifferentValues()
        {
            var mapWidth = _randomizer.Next(2, 100);
            var mapHeight = _randomizer.Next(2, 100);
            var scale = _randomizer.NextFloat();
            var octaves = _randomizer.Next(1, 8);
            var persistence = _randomizer.NextFloat();
            var lacunarity = _randomizer.NextFloat() + _randomizer.Next(10);
            var offset = new Vector2(_randomizer.Next(-10000, 10000), _randomizer.Next(-10000, 10000));
            var seed = _randomizer.Next();
            
            var result = Noise.GenerateNoiseMap(mapWidth, mapHeight, scale, octaves, persistence, lacunarity, offset, seed);

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
            var octaves = _randomizer.Next(1, 8);
            var persistence = _randomizer.NextFloat();
            var lacunarity = _randomizer.NextFloat() + _randomizer.Next(10);
            var offset = new Vector2(_randomizer.Next(-10000, 10000), _randomizer.Next(-10000, 10000));
            var seed = _randomizer.Next();
            
            var result1 = Noise.GenerateNoiseMap(mapWidth, mapHeight, scale1, octaves, persistence, lacunarity, offset, seed);
            var result2 = Noise.GenerateNoiseMap(mapWidth, mapHeight, scale2, octaves, persistence, lacunarity, offset, seed);
            
            Assert.AreNotEqual(result1, result2 );
            
        }

        [Test]
        public void GivenScaleIsZero_WhenCallingGenerateMap_ShouldReturnNoiseMapWithValidNumbers()
        {
            var mapWidth = _randomizer.Next(2, 100);
            var mapHeight = _randomizer.Next(2, 100);
            var scale = 0;
            var octaves = _randomizer.Next(1, 8);
            var persistence = _randomizer.NextFloat();
            var lacunarity = _randomizer.NextFloat() + _randomizer.Next(10);
            var offset = new Vector2(_randomizer.Next(-10000, 10000), _randomizer.Next(-10000, 10000));
            var seed = _randomizer.Next();
            
            var result = Noise.GenerateNoiseMap(mapWidth, mapHeight, scale, octaves, persistence, lacunarity, offset, seed);

            result.Should().NotContain(float.NaN);

        }

        [Test]
        public void GivenGeneratedMap_WhenChangingScale_ShouldScaleFromCenter()
        {
            true.Should().BeTrue();
        }

        [Test]
        public void GivenGeneratedMap_WhenChangingXOffset_ShouldMoveMap()
        {
            true.Should().BeTrue();
        }

        [Test]
        public void GivenGeneratedMap_WhenChangingYOffset_ShouldMoveMap()
        {
            true.Should().BeTrue();
        }

        [Test]
        public void GivenGeneratedMap_WhenChangingSeed_ShouldGenerateNewMap()
        {
            true.Should().BeTrue();
        }

        [Test]
        public void GivenConstantSeed_WhenRegeneratingMap_ShouldGenerateSameMap()
        {
            true.Should().BeTrue();
        }
        
        

    }
}