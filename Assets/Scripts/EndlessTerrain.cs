using System;
using System.Collections.Generic;
using Generators;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
        public const float maxViewDistance = 600;
        public Transform viewer;

        public static Vector2 viewerPosition;
        private int chunkSize;
        private int chunksVisibleInViewDistance;
        
        Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
        List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();

        private void Start()
        {
                chunkSize = MapGenerator.MapChunkSize - 1;
                chunksVisibleInViewDistance = Mathf.RoundToInt(maxViewDistance / chunkSize);
        }

        private void Update()
        {
                viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
                UpdateVisibleChunks();
        }

        private void UpdateVisibleChunks()
        {
                foreach (var t in terrainChunksVisibleLastUpdate)
                {
                        t.SetVisible(false);
                }
                terrainChunksVisibleLastUpdate.Clear();
                
                var currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
                var currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);

                for (var yOffset = -chunksVisibleInViewDistance; yOffset <= chunksVisibleInViewDistance; yOffset++)
                {
                        for (var xOffset = -chunksVisibleInViewDistance; xOffset <= chunksVisibleInViewDistance; xOffset++)
                        {
                                var viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);
                                if (terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                                {
                                        var currentChunk = terrainChunkDictionary[viewedChunkCoord];
                                        currentChunk.UpdateTerrainChunk();
                                        if (currentChunk.isVisible())
                                        {
                                                terrainChunksVisibleLastUpdate.Add(currentChunk);
                                        }
                                }
                                else
                                {
                                        terrainChunkDictionary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, transform));
                                }
                        }
                }
        }

        public class TerrainChunk
        {
                private GameObject meshObject;
                private Vector2 position;
                private Bounds bounds;
                public TerrainChunk(Vector2 coord, int size, Transform parent)
                {
                        position = coord * size;
                        bounds = new Bounds(position, Vector2.one * size);
                        var positionV3 = new Vector3(position.x, 0, position.y);

                        meshObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
                        meshObject.transform.position = positionV3;
                        meshObject.transform.localScale = (Vector3.one * size) / 10f;
                        meshObject.transform.parent = parent;
                        SetVisible(false);
                }

                public void UpdateTerrainChunk()
                {
                        var viewerDistanceFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
                        var visible = viewerDistanceFromNearestEdge <= maxViewDistance;
                        SetVisible(visible);
                }

                public void SetVisible(bool visible)
                {
                        meshObject.SetActive((visible));
                }

                public bool isVisible()
                {
                        return meshObject.activeSelf;
                }
        }
}