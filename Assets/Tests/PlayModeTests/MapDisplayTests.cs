using System.Collections;
using ChanceNET;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.PlayModeTests
{
    public class MapDisplayTests
    {
        private readonly Chance _chance;
        private readonly MapDisplay _sut;
        private readonly Texture2D _expectedTexture;

        public MapDisplayTests()
        {
            _chance = new Chance();
            
            
            var gameObject = new GameObject();
            _sut = gameObject.AddComponent<MapDisplay>();
            _sut.textureRenderer = GetMockTextureRenderer();
            
            _expectedTexture = GetMockExpectedTexture();
        }

        [UnityTest]
        public IEnumerator GivenNoiseMap_WhenCallingDrawNoiseMap_ThenShouldSetTexture()
        {
            _sut.textureRenderer.sharedMaterial.mainTexture.Should().BeNull();
            
            _sut.DrawTexture(_expectedTexture);
            yield return null;
            
            _sut.textureRenderer.sharedMaterial.mainTexture.Should().NotBeNull();
            _sut.textureRenderer.sharedMaterial.mainTexture.Should().BeEquivalentTo(_expectedTexture);
        }

        [UnityTest]
        public IEnumerator GivenNoiseMap_WhenCallingDrawNoiseMap_ThenShouldSetTextureSize()
        {
            Assert.AreEqual(Vector3.one, _sut.textureRenderer.transform.localScale);
            
            _sut.DrawTexture(_expectedTexture);
            yield return null;
            
            var expectedScale = new Vector3(_expectedTexture.width,1, _expectedTexture.height);
            
            Assert.AreEqual(expectedScale, _sut.textureRenderer.transform.localScale);
            
        }

        [SetUp]
        public void BeforeEach()
        {
            _sut.textureRenderer = GetMockTextureRenderer();
        }

        private Texture2D GetMockExpectedTexture()
        {
            var texture= new Texture2D(_chance.Integer(2, 20), _chance.Integer(2, 20));
            for (var x = 0; x < texture.width; x++)
            {
                for (var y = 0; y < texture.height; y++)
                {
                    texture.SetPixel(x, y, new Color(_chance.Integer(0, 255), _chance.Integer(0, 255), _chance.Integer(0, 255)));
                }
            }
            texture.Apply();
            return texture;
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