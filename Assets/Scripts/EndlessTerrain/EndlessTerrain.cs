using System.Collections.Generic;
using Generators;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
        private const float MaxViewDistance = 600;
        public Transform viewer;

        private static Vector2 _viewerPosition;
        private int _chunkSize;
        private int _chunksVisibleInViewDistance;

        private readonly Dictionary<Vector2, TerrainChunk> _terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
        private readonly List<TerrainChunk> _terrainChunksVisibleLastUpdate = new List<TerrainChunk>();

        private void Start()
        {
                _chunkSize = MapGenerator.MapChunkSize - 1;
                _chunksVisibleInViewDistance = Mathf.RoundToInt(MaxViewDistance / _chunkSize);
        }

        private void Update()
        {
                var position = viewer.position;
                _viewerPosition = new Vector2(position.x, position.z);
                UpdateVisibleChunks();
        }

        private void UpdateVisibleChunks()
        {
                foreach (var chunk in _terrainChunksVisibleLastUpdate)
                {
                        chunk.SetVisible(false);
                }
                _terrainChunksVisibleLastUpdate.Clear();
                
                var currentChunkXCoordinate = Mathf.RoundToInt(_viewerPosition.x / _chunkSize);
                var currentChunkYCoordinate = Mathf.RoundToInt(_viewerPosition.y / _chunkSize);

                for (var yOffset = -_chunksVisibleInViewDistance; yOffset <= _chunksVisibleInViewDistance; yOffset++)
                {
                        for (var xOffset = -_chunksVisibleInViewDistance; xOffset <= _chunksVisibleInViewDistance; xOffset++)
                        {
                                var viewedChunkCoordinates = new Vector2(currentChunkXCoordinate + xOffset, currentChunkYCoordinate + yOffset);
                                if (_terrainChunkDictionary.ContainsKey(viewedChunkCoordinates))
                                {
                                        var currentChunk = _terrainChunkDictionary[viewedChunkCoordinates];
                                        currentChunk.UpdateTerrainChunk(_viewerPosition, MaxViewDistance);
                                        if (currentChunk.IsVisible())
                                        {
                                                _terrainChunksVisibleLastUpdate.Add(currentChunk);
                                        }
                                }
                                else
                                {
                                        _terrainChunkDictionary.Add(viewedChunkCoordinates, new TerrainChunk(viewedChunkCoordinates, _chunkSize, transform));
                                }
                        }
                }
        }
}