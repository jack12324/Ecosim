using NUnit.Framework;
using UnityEngine;

namespace Tests.EditModeTests.EndlessTerrain
{
    public class TerrainChunkTests
    {
        private TerrainChunk _sut;

        [Test]
        public void GivenTerrainChunk_WhenCallingSetVisible_ThenShouldSetVisible()
        {
            _sut = new TerrainChunk(Vector2.one, 100, new RectTransform(), null);
            Assert.IsFalse(_sut.IsVisible());
            _sut.SetVisible(true);
            Assert.IsTrue(_sut.IsVisible());
            _sut.SetVisible(false);
            Assert.IsFalse(_sut.IsVisible());
            
        }
        
        [Test]
        public void GivenViewerPositionInViewRange_WhenCallingUpdateTerrainChunk_ThenSetVisibleToTrue()
        {
            _sut = new TerrainChunk(Vector2.one, 100, new RectTransform(), null);
            Assert.IsFalse(_sut.IsVisible());
            _sut.UpdateTerrainChunk(Vector2.zero, 200);
            Assert.IsTrue(_sut.IsVisible());
        }

        [Test]
        public void GivenViewerPositionOutOfViewRange_WhenCallingUpdateTerrainChunk_ThenSetVisibleToFalse()
        {
            _sut = new TerrainChunk(Vector2.one, 100, new RectTransform(), null);
            _sut.SetVisible(true);
            _sut.UpdateTerrainChunk(Vector2.negativeInfinity, 200);
            Assert.IsFalse(_sut.IsVisible());
        }
    }
}