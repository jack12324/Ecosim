using System.Linq;
using ChanceNET;
using Generators;
using NUnit.Framework;
using UnityEngine;

namespace Tests.EditModeTests
{
    public class MeshGeneratorTests
    {
        private readonly Chance _chance;
        private readonly float[,] _heightMap;
        private readonly int _mapWidth;
        private readonly int _mapHeight;

        public MeshGeneratorTests()
        {
            _chance = new Chance();
            _mapWidth = _chance.Integer(2, 50);
            _mapHeight = _chance.Integer(2, 50);
            _heightMap = GenerateTestHeightMap(_mapWidth, _mapHeight);
        }
        
        [Test]
        public void GivenHeightMap_WhenCallingGenerateTerrainMesh_ReturnsMeshDataWithCorrectDataSizes()
        {
            var curve = AnimationCurve.Linear(0, 0, 1, 1);
            var result = MeshGenerator.GenerateTerrainMesh(_heightMap, _chance.Integer(), curve, 0);
            var expectedNumberOfTriangles = (_mapWidth - 1) * (_mapHeight - 1) * 6;
            var expectedNumberOfVertices = _mapWidth * _mapHeight;
            var expectedNumberOfUVs = _mapWidth * _mapHeight;
            
            Assert.AreEqual(expectedNumberOfTriangles, result.Triangles.Length);
            Assert.AreEqual(expectedNumberOfVertices, result.Vertices.Length);
            Assert.AreEqual(expectedNumberOfUVs, result.Uvs.Length);
        } 
        
        [Test]
        public void GivenHeightMap_WhenCallingGenerateTerrainMesh_ReturnsMeshDataWithCorrectData()
        {
            var heightMap = new float[,] {  {1, 1, 1}, 
                                            {1, 1, 1}, 
                                            {1, 1, 1} };

            var curve = AnimationCurve.Linear(0, 0, 1, 1);
            var height = _chance.Integer();
            var result = MeshGenerator.GenerateTerrainMesh(heightMap, height, curve, 0);
            var expectedTriangles = new int[] {0, 4, 3, 4, 0, 1, 1, 5, 4, 5, 1, 2, 3, 7, 6, 7, 3, 4, 4, 8, 7, 8, 4, 5};
            var expectedVertices = new Vector3[]
            {
                new Vector3(-1, height, 1), new Vector3(0, height, 1), new Vector3(1, height, 1),
                new Vector3(-1, height, 0), new Vector3(0, height, 0), new Vector3(1, height, 0),
                new Vector3(-1, height, -1), new Vector3(0, height, -1), new Vector3(1, height, -1)
            };
            var expectedUVs = new Vector2[]
            {
                new Vector2(0, 0), new Vector2(1 / 3f, 0), new Vector2(2 / 3f, 0),
                new Vector2(0, 1 / 3f), new Vector2(1 / 3f, 1 / 3f), new Vector2(2 / 3f, 1 / 3f),
                new Vector2(0, 2 / 3f), new Vector2(1 / 3f, 2 / 3f), new Vector2(2 / 3f, 2 / 3f),
            };
            
            Assert.AreEqual(expectedTriangles, result.Triangles);
            Assert.AreEqual(expectedVertices, result.Vertices);
            Assert.AreEqual(expectedUVs, result.Uvs);
        }
        
        [Test]
        public void GivenHeightMap_WhenCallingGenerateTerrainMeshWithCurve_ReturnsMeshDataWithCorrectHeights()
        {
            var heightMap = new float[,] {  {.1f, .2f, .3f}, 
                                            {.4f, .55f, .6f}, 
                                            {.7f, .8f, .9f} };

            var curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0, 0, 0, 0));
            curve.AddKey(new Keyframe(.49f, 0, 0, 50));
            curve.AddKey(new Keyframe(.51f, 1, 50, 0));
            curve.AddKey(new Keyframe(1, 1, 0, 0));
            var height = _chance.Integer();
            var result = MeshGenerator.GenerateTerrainMesh(heightMap, height, curve, 0);

            var expectedHeights = new float[]
            {
                     0,      0,      0,
                     0, height, height,
                height, height, height
            };
            var actualHeights = result.Vertices.Select(vector3 => vector3.y).ToArray();

            Assert.AreEqual(expectedHeights, actualHeights);
        }
        
        [Test]
        public void GivenHeightMap_WhenCallingGenerateTerrainMeshWithLessDetail_ReturnsMeshDataWithCorrectNumberOfVertices()
        {
            var heightMap = new float[241, 241];
            for (var row = 0; row < heightMap.GetLength(0); row++)
            {
                for (var col = 0; col < heightMap.GetLength(1); col++)
                {
                    heightMap[row, col] = 1;
                }
            }

            var curve = AnimationCurve.Linear(0, 0, 1, 1);
            var height = _chance.Integer();

            for (var levelOfDetail = 0; levelOfDetail <= 6; levelOfDetail++)
            {
                var detailAdjusted = levelOfDetail == 0 ? 1 : levelOfDetail * 2;
                var result = MeshGenerator.GenerateTerrainMesh(heightMap, height, curve, levelOfDetail);
                var expectedVerticesLength = ((heightMap.GetLength(1) - 1) / detailAdjusted + 1) * heightMap.GetLength(0);
                Assert.AreEqual(expectedVerticesLength, result.Vertices.Length);
            }
        }
        
        private float[,] GenerateTestHeightMap(int mapWidth, int mapHeight)
        {
            
            var noiseMap = new float[mapHeight, mapWidth];

            for (var row = 0; row < mapHeight; row++)
            {
                for (var col = 0; col < mapWidth; col++)
                {
                    noiseMap[row, col] = (float)_chance.Double();
                }
            }

            return noiseMap;
        }
    }
}