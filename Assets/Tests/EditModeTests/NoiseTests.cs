using System;
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
            
            var attributes = new MapAttributes(mapWidth, mapHeight, scale, octaves, persistence, lacunarity, offset, seed);
            
            var result = Noise.GenerateNoiseMap(attributes);

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
            
            var attributes = new MapAttributes(mapWidth, mapHeight, scale, octaves, persistence, lacunarity, offset, seed);
            var result = Noise.GenerateNoiseMap(attributes);

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
            
            
            var attributes1 = new MapAttributes(mapWidth, mapHeight, scale1, octaves, persistence, lacunarity, offset, seed);
            var attributes2 = new MapAttributes(mapWidth, mapHeight, scale2, octaves, persistence, lacunarity, offset, seed);
            var result1 = Noise.GenerateNoiseMap(attributes1);
            var result2 = Noise.GenerateNoiseMap(attributes2);

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
            
            var attributes = new MapAttributes(mapWidth, mapHeight, scale, octaves, persistence, lacunarity, offset, seed);
            var result = Noise.GenerateNoiseMap(attributes);

            result.Should().NotContain(float.NaN);

        }

        [Test]
        public void GivenGeneratedMap_WhenChangingScale_ShouldScaleFromCenter()
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
            
            var attributes1 = new MapAttributes(mapWidth, mapHeight, scale1, octaves, persistence, lacunarity, offset, seed);
            var attributes2 = new MapAttributes(mapWidth, mapHeight, scale2, octaves, persistence, lacunarity, offset, seed);
            var result1 = Noise.GenerateNoiseMap(attributes1);
            var result2 = Noise.GenerateNoiseMap(attributes2);


            var midHeight = mapHeight / 2;
            var midWidth = mapWidth / 2;
            
            var middlePixel1 = result1[midWidth, midHeight];
            var middlePixel2 = result2[midWidth, midHeight];

            middlePixel1.Should().Be(middlePixel2);
        }
        
        [Test]
        public void GivenGeneratedMap_WhenDecreasingXOffset_ShouldScrollMapRight()
        {
            var mapWidth = _chance.Integer(2, 100);
            var mapHeight = _chance.Integer(2, 100);
            var scale = _chance.Integer(0,10);
            var octaves = _chance.Integer(1, 8);
            var persistence = (float) _chance.Double();
            var lacunarity = (float) _chance.Double() + _chance.Integer(0, 10);
            var offset = new Vector2(_chance.Integer(-10000, 10000), _chance.Integer(-10000, 10000));
            var offsetDifference = _chance.Integer(-5, 1);
            var offset2 = new Vector2(offset.x + offsetDifference, offset.y);
            var seed = _chance.Integer();
            
            var attributes = new MapAttributes(mapWidth, mapHeight, scale, octaves, persistence, lacunarity, offset, seed);
            var result1 = Noise.GenerateNoiseMap(attributes);
            
            var attributes2 = new MapAttributes(mapWidth, mapHeight, scale, octaves, persistence, lacunarity, offset2, seed);
            var result2 = Noise.GenerateNoiseMap(attributes2);
            
            for (var x = 0; x < mapWidth + scale * offsetDifference; x++)
            {
                for(var y = 0; y < mapHeight; y++)
                {
                    Assert.AreEqual(result1[x, y], result2[x - scale * offsetDifference, y]);
                }
            }
        }
        
        [Test]
        public void GivenGeneratedMap_WhenIncreasingXOffset_ShouldScrollMapLeft()
        {
            var mapWidth = _chance.Integer(2, 100);
            var mapHeight = _chance.Integer(2, 100);
            var scale = _chance.Integer(0,10);
            var octaves = _chance.Integer(1, 8);
            var persistence = (float) _chance.Double();
            var lacunarity = (float) _chance.Double() + _chance.Integer(0, 10);
            var offset = new Vector2(_chance.Integer(-10000, 10000), _chance.Integer(-10000, 10000));
            var offsetDifference = _chance.Integer(0, 6);
            var offset2 = new Vector2(offset.x + offsetDifference, offset.y);
            var seed = _chance.Integer();
            
            var attributes = new MapAttributes(mapWidth, mapHeight, scale, octaves, persistence, lacunarity, offset, seed);
            var result1 = Noise.GenerateNoiseMap(attributes);
            
            var attributes2 = new MapAttributes(mapWidth, mapHeight, scale, octaves, persistence, lacunarity, offset2, seed);
            var result2 = Noise.GenerateNoiseMap(attributes2);
            
            for (var x = scale * offsetDifference; x < mapWidth; x++)
            {
                for(var y = 0; y < mapHeight; y++)
                {
                    Assert.AreEqual(result1[x, y], result2[x - scale * offsetDifference, y]);
                }
            }
        }
        
        [Test]
        public void GivenGeneratedMap_WhenDecreasingYOffset_ShouldScrollMapUp()
        {
            var mapWidth = _chance.Integer(2, 100);
            var mapHeight = _chance.Integer(2, 100);
            var scale = _chance.Integer(0,10);
            var octaves = _chance.Integer(1, 8);
            var persistence = (float) _chance.Double();
            var lacunarity = (float) _chance.Double() + _chance.Integer(0, 10);
            var offset = new Vector2(_chance.Integer(-10000, 10000), _chance.Integer(-10000, 10000));
            var offsetDifference = _chance.Integer(-5, 1);
            var offset2 = new Vector2(offset.x, offset.y+ offsetDifference);
            var seed = _chance.Integer();
            
            var attributes = new MapAttributes(mapWidth, mapHeight, scale, octaves, persistence, lacunarity, offset, seed);
            var result1 = Noise.GenerateNoiseMap(attributes);
            
            var attributes2 = new MapAttributes(mapWidth, mapHeight, scale, octaves, persistence, lacunarity, offset2, seed);
            var result2 = Noise.GenerateNoiseMap(attributes2);
            
            for (var x = 0; x < mapWidth; x++)
            {
                for(var y = 0; y < mapHeight + scale * offsetDifference; y++)
                {
                    Assert.AreEqual(result1[x, y], result2[x, y - scale * offsetDifference]);
                }
            }
        }
        
        [Test]
        public void GivenGeneratedMap_WhenIncreasingYOffset_ShouldScrollMapDown()
        {
            var mapWidth = _chance.Integer(2, 100);
            var mapHeight = _chance.Integer(2, 100);
            var scale = _chance.Integer(0,10);
            var octaves = _chance.Integer(1, 8);
            var persistence = (float) _chance.Double();
            var lacunarity = (float) _chance.Double() + _chance.Integer(0, 10);
            var offset = new Vector2(_chance.Integer(-10000, 10000), _chance.Integer(-10000, 10000));
            var offsetDifference = _chance.Integer(0, 6);
            var offset2 = new Vector2(offset.x, offset.y+ offsetDifference);
            var seed = _chance.Integer();
            
            var attributes = new MapAttributes(mapWidth, mapHeight, scale, octaves, persistence, lacunarity, offset, seed);
            var result1 = Noise.GenerateNoiseMap(attributes);
            
            var attributes2 = new MapAttributes(mapWidth, mapHeight, scale, octaves, persistence, lacunarity, offset2, seed);
            var result2 = Noise.GenerateNoiseMap(attributes2);
            
            for (var x = 0; x < mapWidth; x++)
            {
                for(var y = scale * offsetDifference; y < mapHeight; y++)
                {
                    Assert.AreEqual(result1[x, y], result2[x, y - scale * offsetDifference]);
                }
            }
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
            
            var attributes = new MapAttributes(mapWidth, mapHeight, scale, octaves, persistence, lacunarity, offset, seed);
            var result1 = Noise.GenerateNoiseMap(attributes);

            attributes.Seed = _chance.Integer();
            var result2 = Noise.GenerateNoiseMap(attributes);

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
            
            
            var attributes = new MapAttributes(mapWidth, mapHeight, scale, octaves, persistence, lacunarity, offset, seed);
            var result1 = Noise.GenerateNoiseMap(attributes);
            var result2 = Noise.GenerateNoiseMap(attributes);
            
            Assert.AreEqual(result1, result2);
        }
        
        

    }
}