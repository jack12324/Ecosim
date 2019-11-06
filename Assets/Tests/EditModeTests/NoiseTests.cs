using AutoFixture;
using ChanceNET;
using FluentAssertions;
using Generators;
using NUnit.Framework;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Tests.EditModeTests
{
    public class NoiseTests
    {
        private readonly Fixture _fixture;
        private readonly Chance _chance;

        public NoiseTests()
        {
            _chance = new Chance();
            _fixture = new Fixture();
        }
        
        [Test]
        public void GivenMapDimensions_WhenCallingGenerateMap_ThenReturnMapWithGivenDimensions()
        {
            var attributes = GenerateRandomMapAttributes();
            
            var result = GenerateNoiseMap(attributes);

            Assert.AreEqual(attributes.MapSideLength, result.GetLength(1), attributes.ToString());
            Assert.AreEqual(attributes.MapSideLength, result.GetLength(0), attributes.ToString());
        }

        [Test]
        public void GivenParameters_WhenCallingGenerateMap_ThenReturnMapWithDifferentValues()
        {
            var attributes = GenerateRandomMapAttributes();
            var result = GenerateNoiseMap(attributes);

            var firstPixel = result[0, 0];
            var testPixel = result[_chance.Integer(1, attributes.MapSideLength), _chance.Integer(1, attributes.MapSideLength)];

            Assert.AreNotEqual(firstPixel,testPixel, attributes.ToString());
        }

        [Test]
        public void GivenDifferentScaleValues_WhenCallingGenerateMap_ThenReturnDifferentMaps()
        {
            var attributes1 = GenerateRandomMapAttributes();
            
            var attributes2 = attributes1.DeepCopy();
            attributes2.NoiseScale = (float) _chance.Double() + _chance.Integer(0, 50);
            
            var result1 = GenerateNoiseMap(attributes1);
            var result2 = GenerateNoiseMap(attributes2);

            Assert.AreNotEqual(result1, result2, $"{attributes1}\n{attributes2}");
        }

        [Test]
        public void GivenScaleIsZero_WhenCallingGenerateMap_ShouldReturnNoiseMapWithValidNumbers()
        {
            var attributes = GenerateRandomMapAttributes();
            attributes.NoiseScale = 0;
            var result = GenerateNoiseMap(attributes);

            for (var x = 0; x < result.GetLength(0); x++)
            {
                for (var y = 0; y < result.GetLength(1); y++)
                {
                    Assert.AreNotEqual(result[x, y], float.NaN, attributes.ToString());
                }
            }
        }

        [Test]
        public void GivenGeneratedMap_WhenChangingScale_ShouldScaleFromCenter()
        {
            var attributes1 = GenerateRandomMapAttributes();
            
            var attributes2 = attributes1.DeepCopy();
            attributes2.NoiseScale = (float) _chance.Double() + _chance.Integer(0, 50);
            
            var result1 = GenerateNoiseMap(attributes1);
            var result2 = GenerateNoiseMap(attributes2);


            var midRow = attributes1.MapSideLength / 2;
            var midCol = attributes1.MapSideLength / 2;
            
            var middlePixel1 = result1[midRow, midCol];
            var middlePixel2 = result2[midRow, midCol];

            Assert.AreEqual(middlePixel1, middlePixel2, $"{attributes1}\n{attributes2}");
            middlePixel1.Should().Be(middlePixel2);
        }
        
        [Test]
        public void GivenGeneratedMap_WhenDecreasingXOffset_ShouldScrollMapRight()
        {
            var offsetDifference = _chance.Integer(-5, 1);

            var attributes1 = GenerateRandomMapAttributesForScrollingTests();
            
            var attributes2 = attributes1.DeepCopy();
            attributes2.Offset = new Offset(attributes1.Offset.x + offsetDifference, attributes1.Offset.y);
            
            var result1 = GenerateNoiseMap(attributes1);
            var result2 = GenerateNoiseMap(attributes2);
            
            for (var row = 0; row < attributes1.MapSideLength; row++)
            {
                for(var col = 0; col < attributes1.MapSideLength+ attributes1.NoiseScale * offsetDifference; col++)
                {
                    var message = $"{attributes1}\n" +
                                  $"Current col: {col}\n" +
                                  $"offsetDifference: {offsetDifference}";
                    Assert.AreEqual(result1[row, col], result2[row, (int)(col - attributes1.NoiseScale * offsetDifference)], message);
                }
            }
        }
        
        [Test]
        public void GivenGeneratedMap_WhenIncreasingXOffset_ShouldScrollMapLeft()
        {
           var offsetDifference = _chance.Integer(0, 6);

           var attributes1 = GenerateRandomMapAttributesForScrollingTests();
           
           var attributes2 = attributes1.DeepCopy();
           attributes2.Offset = new Offset(attributes1.Offset.x + offsetDifference, attributes1.Offset.y);
           
           var result1 = GenerateNoiseMap(attributes1);
           var result2 = GenerateNoiseMap(attributes2);
            
            for (var row = 0; row < attributes1.MapSideLength; row++)
            {
                for(var col = (int)(attributes1.MapSideLength + attributes1.NoiseScale * offsetDifference); col < attributes1.MapSideLength; col++)
                {
                    var message = $"{attributes1}\n" +
                                  $"Current col: {col}\n" +
                                  $"offsetDifference: {offsetDifference}";
                    Assert.AreEqual(result1[row, col], result2[row, (int)(col - attributes1.NoiseScale * offsetDifference)], message);
                }
            }
        }
        
        [Test]
        public void GivenGeneratedMap_WhenDecreasingYOffset_ShouldScrollMapDown()
        {
            var offsetDifference = _chance.Integer(-5, 1);

            var attributes1 = GenerateRandomMapAttributesForScrollingTests();
            var attributes2 = attributes1.DeepCopy();
            attributes2.Offset = new Offset(attributes1.Offset.x, attributes1.Offset.y + offsetDifference);
           
            var result1 = GenerateNoiseMap(attributes1);
            var result2 = GenerateNoiseMap(attributes2);
            
            for (var row = 0; row < attributes1.MapSideLength + attributes1.NoiseScale * offsetDifference; row++)
            {
                for(var col = 0; col < attributes1.MapSideLength; col++)
                {
                    var message = $"{attributes1}\n" +
                                  $"Current row: {row}\n" +
                                  $"offsetDifference: {offsetDifference}";
                    Assert.AreEqual(result1[(int)(row - attributes1.NoiseScale * offsetDifference), col], result2[row, col],  message);
                }
            }
        }
        
        [Test]
        public void GivenGeneratedMap_WhenIncreasingYOffset_ShouldScrollMapUp()
        {
            var offsetDifference = _chance.Integer(0, 6);

            var attributes1 = GenerateRandomMapAttributesForScrollingTests();
            var attributes2 = attributes1.DeepCopy();
            attributes2.Offset = new Offset(attributes1.Offset.x, attributes1.Offset.y + offsetDifference);
           
            var result1 = GenerateNoiseMap(attributes1);
            var result2 = GenerateNoiseMap(attributes2);
            
            for (var row = (int)(attributes1.MapSideLength + attributes1.NoiseScale * offsetDifference); row < attributes1.MapSideLength; row++)
            {
                for(var col = 0; col < attributes1.MapSideLength; col++)
                {
                    var message = $"{attributes1}\n" +
                                  $"Current row: {row}\n" +
                                  $"offsetDifference: {offsetDifference}";
                    Assert.AreEqual(result1[row, col], result2[(int)(row - attributes1.NoiseScale * offsetDifference), col], message);
                }
            }
        }

        [Test]
        public void GivenGeneratedMap_WhenChangingOnlySeed_ShouldGenerateNewMap()
        {
            var attributes1 = GenerateRandomMapAttributes();
            var attributes2 = attributes1.DeepCopy();
            attributes2.Seed = _chance.Integer();
            
            
            var result1 = GenerateNoiseMap(attributes1);
            var result2 = GenerateNoiseMap(attributes2);

            Assert.AreNotEqual(result1, result2, $"{attributes1}\n{attributes2}");
        }

        [Test]
        public void GivenConstantSeed_WhenRegeneratingMap_ShouldGenerateSameMap()
        {
            var attributes = GenerateRandomMapAttributes();
            var attributes2 = attributes.DeepCopy();
            
            var result1 = GenerateNoiseMap(attributes);
            var result2 = GenerateNoiseMap(attributes2);
            
            Assert.AreEqual(result1, result2);
        }

        private MapAttributes GenerateRandomMapAttributes()
        {
            return _fixture.Build<MapAttributes>()
                .With(attributes => attributes.MapSideLength, _chance.Integer(2, 100))
                .With(attributes => attributes.MapSideLength, _chance.Integer(2, 100))
                .With(attributes => attributes.NoiseScale, (float) _chance.Double() + _chance.Integer(0, 50))
                .With(attributes => attributes.Octaves, _chance.Integer(1, 10))
                .With(attributes => attributes.Persistence, (float) _chance.Double())
                .With(attributes => attributes.Lacunarity, (float) _chance.Double() + _chance.Integer(0, 10))
                .With(attributes => attributes.Offset, new Offset(_chance.Integer(-10000, 10000), _chance.Integer(-10000, 10000)))
                .Create();
        } 
        
        private MapAttributes GenerateRandomMapAttributesForScrollingTests()
        {
            var attributes = GenerateRandomMapAttributes();
            //map must be wider than 2 * max scale * offset difference
            var largeEnoughWidth = _chance.Integer(51, 100);
            var largeEnoughHeight = _chance.Integer(51, 100);
            //reverse scaling calculation is not exact when scale is not integer
            var integerScale = _chance.Integer(1, 5);
            
            attributes.MapSideLength = largeEnoughWidth;
            attributes.MapSideLength = largeEnoughHeight;
            attributes.NoiseScale = integerScale;

            return attributes;
        }
        private float[,] GenerateNoiseMap(MapAttributes attributes)
        {
            
            var sysRand = new System.Random(attributes.Seed);
            attributes.Seed = sysRand.Next();
            
            var noiseMapFlat = new NativeArray<float>(attributes.MapSideLength * attributes.MapSideLength, Allocator.TempJob);

            var mapDataJob = new Noise.GenerateNoiseMapJob
            {
                sideLength = attributes.MapSideLength,
                lacunarity = attributes.Lacunarity,
                NoiseMap = noiseMapFlat,
                noiseScale = attributes.NoiseScale,
                Octaves = attributes.Octaves,
                offset = attributes.Offset,
                persistence = attributes.Persistence,
                Seed = (uint)attributes.Seed
            };

            var handle = mapDataJob.Schedule();
            handle.Complete();
            
            var noiseMap = new float[attributes.MapSideLength, attributes.MapSideLength];
            var index = 0;
            for (var row = 0; row < noiseMap.GetLength(0); row++)
            {
                for (var col = 0; col < noiseMap.GetLength(1); col++)
                {
                    noiseMap[row, col] = noiseMapFlat[index];
                    ++index;
                }
            }
                
            noiseMapFlat.Dispose();

            return noiseMap;
        }
        

    }
}