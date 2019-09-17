using AutoFixture;
using ChanceNET;
using FluentAssertions;
using NUnit.Framework;
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
            
            var result = Noise.GenerateNoiseMap(attributes);

            Assert.AreEqual(attributes.MapWidth, result.GetLength(1), attributes.ToString());
            Assert.AreEqual(attributes.MapHeight, result.GetLength(0), attributes.ToString());
        }

        [Test]
        public void GivenParameters_WhenCallingGenerateMap_ThenReturnMapWithDifferentValues()
        {
            var attributes = GenerateRandomMapAttributes();
            var result = Noise.GenerateNoiseMap(attributes);

            var firstPixel = result[0, 0];
            var testPixel = result[_chance.Integer(1, attributes.MapHeight), _chance.Integer(1, attributes.MapWidth)];

            Assert.AreNotEqual(firstPixel,testPixel, attributes.ToString());
        }

        [Test]
        public void GivenDifferentScaleValues_WhenCallingGenerateMap_ThenReturnDifferentMaps()
        {
            var attributes1 = GenerateRandomMapAttributes();
            
            var attributes2 = attributes1.DeepCopy();
            attributes2.NoiseScale = (float) _chance.Double() + _chance.Integer(0, 50);
            
            var result1 = Noise.GenerateNoiseMap(attributes1);
            var result2 = Noise.GenerateNoiseMap(attributes2);

            Assert.AreNotEqual(result1, result2, $"{attributes1}\n{attributes2}");
        }

        [Test]
        public void GivenScaleIsZero_WhenCallingGenerateMap_ShouldReturnNoiseMapWithValidNumbers()
        {
            var attributes = GenerateRandomMapAttributes();
            attributes.NoiseScale = 0;
            var result = Noise.GenerateNoiseMap(attributes);

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
            
            var result1 = Noise.GenerateNoiseMap(attributes1);
            var result2 = Noise.GenerateNoiseMap(attributes2);


            var midRow = attributes1.MapHeight / 2;
            var midCol = attributes1.MapWidth / 2;
            
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
            attributes2.Offset = new Vector2(attributes1.Offset.x + offsetDifference, attributes1.Offset.y);
            
            var result1 = Noise.GenerateNoiseMap(attributes1);
            var result2 = Noise.GenerateNoiseMap(attributes2);
            
            for (var row = 0; row < attributes1.MapHeight; row++)
            {
                for(var col = 0; col < attributes1.MapWidth+ attributes1.NoiseScale * offsetDifference; col++)
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
           attributes2.Offset = new Vector2(attributes1.Offset.x + offsetDifference, attributes1.Offset.y);
           
           var result1 = Noise.GenerateNoiseMap(attributes1);
           var result2 = Noise.GenerateNoiseMap(attributes2);
            
            for (var row = 0; row < attributes1.MapHeight; row++)
            {
                for(var col = (int)(attributes1.MapWidth + attributes1.NoiseScale * offsetDifference); col < attributes1.MapWidth; col++)
                {
                    var message = $"{attributes1}\n" +
                                  $"Current col: {col}\n" +
                                  $"offsetDifference: {offsetDifference}";
                    Assert.AreEqual(result1[row, col], result2[row, (int)(col - attributes1.NoiseScale * offsetDifference)], message);
                }
            }
        }
        
        [Test]
        public void GivenGeneratedMap_WhenDecreasingYOffset_ShouldScrollMapUp()
        {
            var offsetDifference = _chance.Integer(-5, 1);

            var attributes1 = GenerateRandomMapAttributesForScrollingTests();
            var attributes2 = attributes1.DeepCopy();
            attributes2.Offset = new Vector2(attributes1.Offset.x, attributes1.Offset.y + offsetDifference);
           
            var result1 = Noise.GenerateNoiseMap(attributes1);
            var result2 = Noise.GenerateNoiseMap(attributes2);
            
            for (var row = 0; row < attributes1.MapHeight + attributes1.NoiseScale * offsetDifference; row++)
            {
                for(var col = 0; col < attributes1.MapWidth; col++)
                {
                    var message = $"{attributes1}\n" +
                                  $"Current row: {row}\n" +
                                  $"offsetDifference: {offsetDifference}";
                    Assert.AreEqual(result1[row, col], result2[(int)(row - attributes1.NoiseScale * offsetDifference), col],  message);
                }
            }
        }
        
        [Test]
        public void GivenGeneratedMap_WhenIncreasingYOffset_ShouldScrollMapDown()
        {
            var offsetDifference = _chance.Integer(0, 6);

            var attributes1 = GenerateRandomMapAttributesForScrollingTests();
            var attributes2 = attributes1.DeepCopy();
            attributes2.Offset = new Vector2(attributes1.Offset.x, attributes1.Offset.y + offsetDifference);
           
            var result1 = Noise.GenerateNoiseMap(attributes1);
            var result2 = Noise.GenerateNoiseMap(attributes2);
            
            for (var row = (int)(attributes1.MapHeight + attributes1.NoiseScale * offsetDifference); row < attributes1.MapHeight; row++)
            {
                for(var col = 0; col < attributes1.MapWidth; col++)
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
            
            
            var result1 = Noise.GenerateNoiseMap(attributes1);
            var result2 = Noise.GenerateNoiseMap(attributes2);

            Assert.AreNotEqual(result1, result2, $"{attributes1}\n{attributes2}");
        }

        [Test]
        public void GivenConstantSeed_WhenRegeneratingMap_ShouldGenerateSameMap()
        {
            var attributes = GenerateRandomMapAttributes();
            var attributes2 = attributes.DeepCopy();
            
            var result1 = Noise.GenerateNoiseMap(attributes);
            var result2 = Noise.GenerateNoiseMap(attributes2);
            
            Assert.AreEqual(result1, result2);
        }

        private MapAttributes GenerateRandomMapAttributes()
        {
            return _fixture.Build<MapAttributes>()
                .With(attributes => attributes.MapWidth, _chance.Integer(2, 100))
                .With(attributes => attributes.MapHeight, _chance.Integer(2, 100))
                .With(attributes => attributes.NoiseScale, (float) _chance.Double() + _chance.Integer(0, 50))
                .With(attributes => attributes.Octaves, _chance.Integer(1, 10))
                .With(attributes => attributes.Persistence, (float) _chance.Double())
                .With(attributes => attributes.Lacunarity, (float) _chance.Double() + _chance.Integer(0, 10))
                .With(attributes => attributes.Offset, new Vector2(_chance.Integer(-10000, 10000), _chance.Integer(-10000, 10000)))
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
            
            attributes.MapWidth = largeEnoughWidth;
            attributes.MapHeight = largeEnoughHeight;
            attributes.NoiseScale = integerScale;

            return attributes;
        }
        

    }
}