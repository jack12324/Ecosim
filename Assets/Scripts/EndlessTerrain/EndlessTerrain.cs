using System.Collections.Generic;
using System.Linq;
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
        private readonly List<TerrainChunk> _newTerrainChunksToGenerate = new List<TerrainChunk>();
        private readonly List<TerrainChunk> _chunksVisibleLastUpdate = new List<TerrainChunk>();

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

                _newTerrainChunksToGenerate.Clear();
                UpdateVisibleChunks();

                if (!_newTerrainChunksToGenerate.IsNullOrEmpty())
                {
                        var meshData = GenerateNewMapDataForChunks(_newTerrainChunksToGenerate.Select(chunk=> chunk.Position).ToList());

                        for (var i = 0; i < _newTerrainChunksToGenerate.Count; i++)
                        {
                                _newTerrainChunksToGenerate[i].MeshFilter.mesh = meshData[i].Mesh.CreateMesh();
                                _newTerrainChunksToGenerate[i].MeshRenderer.material.mainTexture = meshData[i].Texture;
                        }
                        
                }

                viewer.transform.Translate(Vector3.forward * Time.deltaTime * 100);
        }

        private List<TerrainChunkRenderData> GenerateNewMapDataForChunks(List<Vector2> chunkCenters)
        {
                var jobs = new NativeArray<JobHandle>(_newTerrainChunksToGenerate.Count * 2, Allocator.TempJob);

                var meshData = new List<TerrainChunkRenderData>();
                var sysRand = new Random(_mapGenerator.seed);

                var attributes = new MapAttributes(_chunkSize + 1,
                        _mapGenerator.noiseScale, _mapGenerator.octaves, _mapGenerator.persistence, 
                        _mapGenerator.lacunarity, _mapGenerator.offset, sysRand.Next());

                var terrainRegions = new NativeArray<TerrainType>(_mapGenerator.regions.Length, Allocator.TempJob);
                terrainRegions.CopyFrom(_mapGenerator.regions.OrderBy(region => region.maxHeight).ToArray());

                //map data job outputs
                var noiseMapsFlat = new List<NativeArray<float>>();

                //mesh data job outputs
                var triangles = new List<NativeArray<int>>();
                var vertices = new List<NativeArray<Vector3>>();
                var uvs = new List<NativeArray<Vector2>>();
                
                //color map outputs
                var colorMaps = new List<NativeArray<Color>>();

                for (var i = 0; i < _newTerrainChunksToGenerate.Count; i++)
                {
                        noiseMapsFlat.Add(new NativeArray<float>(attributes.MapSideLength * attributes.MapSideLength, Allocator.TempJob));
                        
                        triangles.Add(new NativeArray<int>((_chunkSize) * (_chunkSize) * 6, Allocator.TempJob));
                        vertices.Add(new NativeArray<Vector3>(attributes.MapSideLength * attributes.MapSideLength, Allocator.TempJob));
                        uvs.Add(new NativeArray<Vector2>(attributes.MapSideLength * attributes.MapSideLength, Allocator.TempJob));
                        
                        colorMaps.Add(new NativeArray<Color>(attributes.MapSideLength * attributes.MapSideLength, Allocator.TempJob));
                        var o = new Offset(chunkCenters[i].x/attributes.NoiseScale , chunkCenters[i].y/attributes.NoiseScale);

                        var mapDataJob = new Noise.GenerateNoiseMapJob()
                        {
                                sideLength = attributes.MapSideLength,
                                lacunarity = attributes.Lacunarity,
                                NoiseMap = noiseMapsFlat[i],
                                noiseScale = attributes.NoiseScale,
                                Octaves = attributes.Octaves,
                                offset = o,
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
                        var colorMapJob = new TextureGenerator.ColorMapFromTerrainsJob
                        {
                                ColorMap = colorMaps[i],
                                NoiseMapFlat = noiseMapsFlat[i],
                                SortedRegions = terrainRegions
                        };
                        JobHandle mapDataHandle = mapDataJob.Schedule();
                        jobs[i*2] = meshDataJob.Schedule(mapDataHandle);
                        jobs[i * 2 + 1] = colorMapJob.Schedule(mapDataHandle);
                }

                JobHandle.CompleteAll(jobs);
                
                for (var i = 0; i < _newTerrainChunksToGenerate.Count; i++)
                {
                        var heightAdjustedVertices = MeshGenerator.AdjustTrianglesWithCurve(vertices[i].ToArray(),
                                _mapGenerator.meshHeightMultiplier, _mapGenerator.meshHeightCurve);
                        meshData.Add(new TerrainChunkRenderData(
                                TextureGenerator.TextureFromColorMap(colorMaps[i].ToArray(), attributes.MapSideLength,
                                        attributes.MapSideLength),
                                new MeshData(heightAdjustedVertices, triangles[i].ToArray(), uvs[i].ToArray())
                                ));

                }

                jobs.Dispose();
                terrainRegions.Dispose();

                noiseMapsFlat.ForEach(element => element.Dispose());
                
                triangles.ForEach(element => element.Dispose());
                vertices.ForEach(element => element.Dispose());
                uvs.ForEach(element => element.Dispose());
                
                colorMaps.ForEach(element=> element.Dispose());

                return meshData;
        }
        
       

        private void UpdateVisibleChunks()
        {
                _chunksVisibleLastUpdate.ForEach(chunk => chunk.SetVisible(false));
                _chunksVisibleLastUpdate.Clear();

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
                                        _chunksVisibleLastUpdate.Add(currentChunk);
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