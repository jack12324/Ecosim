using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Generators
{
    public static class MeshGenerator
    {
        public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier,
            AnimationCurve meshHeightCurve, int levelOfDetail)
        {
            var width = heightMap.GetLength(1);
            var height = heightMap.GetLength(0);
            var topLeftRow = (height - 1) / 2f;
            var topLeftCol = (width - 1) / -2f;

            var meshSimplificationIncrement = levelOfDetail == 0 ? 1 : levelOfDetail * 2;
            var verticesPerLine = (width - 1) / meshSimplificationIncrement + 1;

            var meshData = new MeshData(verticesPerLine, height);
            var vertexIndex = 0;

            for (var row = 0; row < height; row += meshSimplificationIncrement)
            {
                for (var col = 0; col < width; col += meshSimplificationIncrement)
                {
                    meshData.Vertices[vertexIndex] = new Vector3(topLeftCol + col, heightMultiplier * meshHeightCurve.Evaluate(heightMap[row,col]), topLeftRow - row);
                    meshData.Uvs[vertexIndex] = new Vector2(col / (float)width, row / (float)height);

                    if (row < height - 1 && col < width - 1)
                    {
                        meshData.AddTriangle(vertexIndex, vertexIndex + verticesPerLine + 1, vertexIndex + verticesPerLine);
                        meshData.AddTriangle(vertexIndex + verticesPerLine + 1, vertexIndex, vertexIndex + 1);
                    }
                    
                    ++vertexIndex;
                }
            }

            return meshData;
        }
    }
}

public class MeshData
{
    public Vector3[] Vertices{ get; }
    public  int[] Triangles { get; }
    public Vector2[] Uvs { get; }

    private int _triangleIndex;
    
    public MeshData(int meshWidth, int meshHeight)
    {
        Vertices = new Vector3[meshWidth * meshHeight];
        Triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
        Uvs = new Vector2[meshWidth * meshHeight];
    }

    public void AddTriangle(int a, int b, int c)
    {
        Triangles[_triangleIndex] = a;
        Triangles[_triangleIndex + 1] = b;
        Triangles[_triangleIndex + 2] = c;
        _triangleIndex += 3;
    }

    public Mesh CreateMesh()
    {
        var mesh = new Mesh {vertices = Vertices, triangles = Triangles, uv = Uvs};
        mesh.RecalculateNormals();
        return mesh;
    }
}