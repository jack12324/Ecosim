using System.Collections.Generic;
using Castle.Core.Internal;
using Generators;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Random = System.Random;

public class EndlessTerrain : MonoBehaviour
{
        public bool thread;
        private const float MaxViewDistance = 400;
        public Transform viewer;
        public Material mapMaterial;

        private static Vector2 _viewerPosition;
        private int _chunkSize;
        private int _chunksVisibleInViewDistance;

        private MapGenerator _mapGenerator;

        private readonly Dictionary<Vector2, TerrainChunk> _terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
        private readonly List<TerrainChunk> _terrainChunksVisibleLastUpdate = new List<TerrainChunk>();
        private readonly List<TerrainChunk> _newTerrainChunksToGenerate = new List<TerrainChunk>();

        private void Start()
        {
                _mapGenerator = GetComponent<MapGenerator>();
                _chunkSize = MapGenerator.MapChunkSize - 1;
                _chunksVisibleInViewDistance = Mathf.RoundToInt(MaxViewDistance / _chunkSize);
        }

        private void Update()
        {
                var position = viewer.position;
                _viewerPosition = new Vector2(position.x, position.z);
                UpdateVisibleChunks();

                if (!_newTerrainChunksToGenerate.IsNullOrEmpty())
                {
                        var meshData = GenerateNewMapDataForChunks();

                        for (var i = 0; i < _newTerrainChunksToGenerate.Count; i++)
                        {
                                _newTerrainChunksToGenerate[i]._meshFilter.mesh = meshData[i].CreateMesh();
                        }
                        
                }

                viewer.transform.Translate(Vector3.forward * Time.deltaTime * 100);
        }

        private List<MeshData> GenerateNewMapDataForChunks()
        {
                var jobs = new NativeArray<JobHandle>(_newTerrainChunksToGenerate.Count, Allocator.TempJob);

                var meshData = new List<MeshData>();
                var sysRand = new Random(_mapGenerator.seed);

                var attributes = new MapAttributes(_chunkSize + 1,
                        _mapGenerator.noiseScale, _mapGenerator.octaves, _mapGenerator.persistence, 
                        _mapGenerator.lacunarity, _mapGenerator.offset, sysRand.Next());

                var terrainRegions = new NativeArray<TerrainType>(_mapGenerator.regions.Length, Allocator.TempJob);
                for (var i = 0; i < terrainRegions.Length; i++)
                        terrainRegions[i] = _mapGenerator.regions[i];

                //map data job outputs
                var noiseMapsFlat = new List<NativeArray<float>>();

                //mesh data job outputs
                var triangles = new List<NativeArray<int>>();
                var vertices = new List<NativeArray<Vector3>>();
                var uvs = new List<NativeArray<Vector2>>();

                for (var i = 0; i < _newTerrainChunksToGenerate.Count; i++)
                {
                        noiseMapsFlat.Add(new NativeArray<float>(attributes.MapSideLength * attributes.MapSideLength, Allocator.TempJob));
                        
                        triangles.Add(new NativeArray<int>((_chunkSize) * (_chunkSize) * 6, Allocator.TempJob));
                        vertices.Add(new NativeArray<Vector3>(attributes.MapSideLength * attributes.MapSideLength, Allocator.TempJob));
                        uvs.Add(new NativeArray<Vector2>(attributes.MapSideLength * attributes.MapSideLength, Allocator.TempJob));

                        var mapDataJob = new Noise.GenerateNoiseMapJob()
                        {
                                sideLength = attributes.MapSideLength,
                                lacunarity = attributes.Lacunarity,
                                NoiseMap = noiseMapsFlat[i],
                                noiseScale = attributes.NoiseScale,
                                Octaves = attributes.Octaves,
                                offset = attributes.Offset,
                                persistence = attributes.Persistence,
                                Seed = (uint)attributes.Seed
                                
                        };
                        var meshDataJob = new MeshGeneratorJob
                        {
                                Triangles = triangles[i],
                                Vertices = vertices[i],
                                Uvs = uvs[i],
                                HeightMap = noiseMapsFlat[i],
                                SideLength = attributes.MapSideLength,
                                LevelOfDetail = _mapGenerator.levelOfDetail
                        };
                        jobs[i] = meshDataJob.Schedule(mapDataJob.Schedule());
                }

                JobHandle.CompleteAll(jobs);
                
                for (var i = 0; i < _newTerrainChunksToGenerate.Count; i++)
                {
                        var heightAdjustedVertices = MeshGenerator.AdjustTrianglesWithCurve(vertices[i].ToArray(),
                                _mapGenerator.meshHeightMultiplier, _mapGenerator.meshHeightCurve);
                        meshData.Add(new MeshData(heightAdjustedVertices, triangles[i].ToArray(), uvs[i].ToArray()));

                }

                jobs.Dispose();
                terrainRegions.Dispose();

                noiseMapsFlat.ForEach(triangle => triangle.Dispose());
                
                triangles.ForEach(triangle => triangle.Dispose());
                vertices.ForEach(triangle => triangle.Dispose());
                uvs.ForEach(triangle => triangle.Dispose());

                return meshData;
        }
        
       

        private void UpdateVisibleChunks()
        {
                foreach (var chunk in _terrainChunksVisibleLastUpdate)
                {
                        chunk.SetVisible(false);
                }
                _terrainChunksVisibleLastUpdate.Clear();
                _newTerrainChunksToGenerate.Clear();
                
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
                                        currentChunk.SetVisible(true);
                                        _terrainChunksVisibleLastUpdate.Add(currentChunk);
                                }
                                else
                                {
                                        var newChunk = new TerrainChunk(viewedChunkCoordinates, _chunkSize, transform, mapMaterial);
                                        _terrainChunkDictionary.Add(viewedChunkCoordinates, newChunk);
                                        _newTerrainChunksToGenerate.Add(newChunk);
                                }
                        }
                }
        }
}