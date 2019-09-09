using System.Collections;
using ChanceNET;
using FluentAssertions;
using NSubstitute;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.PlayModeTests
{
    public class MapDisplayTests
    {
        private readonly Chance _chance;
        private readonly IMapDisplayController _mapDisplayController;
        private readonly MapDisplay _sut;
        private readonly Texture _expectedTexture;
        private readonly float[,] _noiseMap;

        public MapDisplayTests()
        {
            _chance = new Chance();
            
            var gameObject = new GameObject();
            _sut = gameObject.AddComponent<MapDisplay>();
            
            _expectedTexture = GetMockExpectedTexture();
            _mapDisplayController = GetMockDisplayController(_expectedTexture);
            
            _noiseMap = GetMockNoiseMap();
        }

        [UnityTest]
        public IEnumerator GivenNoiseMap_WhenCallingDrawNoiseMap_ThenShouldCallGenerateTextureWithNoiseMap()
        {
            _sut.DrawNoiseMap(_noiseMap);
            yield return null;
            
            _mapDisplayController.Received(1).GenerateTexture(_noiseMap);
        }

        [UnityTest]
        public IEnumerator GivenNoiseMap_WhenCallingDrawNoiseMap_ThenShouldSetTexture()
        {
            _sut.textureRenderer.sharedMaterial.mainTexture.Should().BeNull();
            
            _sut.DrawNoiseMap(_noiseMap);
            yield return null;
            
            _sut.textureRenderer.sharedMaterial.mainTexture.Should().NotBeNull();
            _sut.textureRenderer.sharedMaterial.mainTexture.Should().BeEquivalentTo(_expectedTexture);
        }

        [UnityTest]
        public IEnumerator GivenNoiseMap_WhenCallingDrawNoiseMap_ThenShouldSetTextureSize()
        {
            _sut.textureRenderer.transform.localScale.Should().BeEquivalentTo(Vector3.one);
            
            _sut.DrawNoiseMap(_noiseMap);
            yield return null;
            
            var expectedScale = new Vector3(_expectedTexture.width,1, _expectedTexture.height);
            
            _sut.textureRenderer.transform.localScale.Should().BeEquivalentTo(expectedScale);
            
        }

        [UnitySetUp]
        public IEnumerator BeforeEach()
        {
            yield return null;
            
            var textureRenderer = GetMockTextureRenderer();
            _mapDisplayController.ClearReceivedCalls();
            
            _sut.Construct(_mapDisplayController, textureRenderer);
        }

        private float[,] GetMockNoiseMap()
        {
            var mapWidth = _chance.Integer(2, 20);
            var mapHeight = _chance.Integer(2, 20);
            var noiseMap = new float[mapWidth, mapHeight];
            for (int xIndex = 0; xIndex < mapWidth; xIndex++)
            {
                for (int yIndex = 0; yIndex < mapHeight; yIndex++)
                {
                    noiseMap[xIndex, yIndex] = (float) _chance.Double();
                }
            }

            return noiseMap;
        }

        private Texture GetMockExpectedTexture()
        {
            return new Texture2D(_chance.Integer(2, 20), _chance.Integer(2, 20));
        }

        private IMapDisplayController GetMockDisplayController(Texture expectedTexture)
        {
            var controller = Substitute.For<IMapDisplayController>();
            controller .GenerateTexture(default).ReturnsForAnyArgs(expectedTexture);
            return controller;
        }

        private Renderer GetMockTextureRenderer()
        {
            var renderer = Resources.Load<Renderer>("Tests/MapDisplay");
            renderer.sharedMaterial.mainTexture = default;
            renderer.transform.localScale = Vector3.one;
            
            return renderer;
        }
    }
}