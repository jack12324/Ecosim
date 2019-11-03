using UnityEngine;

public class TerrainChunk
{
    private readonly GameObject _meshObject;
    private Bounds _bounds;

    public MeshRenderer MeshRenderer;
    public MeshFilter MeshFilter;
    public Vector2 Position;
    public TerrainChunk(Vector2 coordinates, int size, Transform parent, Material material)
    {
        Position = coordinates * size;
        _bounds = new Bounds(Position, Vector2.one * size);
        var positionV3 = new Vector3(Position.x, 0, Position.y);

        _meshObject = new GameObject("Terrain Chunk");
        MeshRenderer = _meshObject.AddComponent<MeshRenderer>();
        MeshRenderer.material = material;
        MeshFilter = _meshObject.AddComponent<MeshFilter>();
        
        _meshObject.transform.position = positionV3;
        _meshObject.transform.parent = parent;
        SetVisible(false);
        
    }

    public void UpdateTerrainChunk(Vector2 viewerPosition, float maxViewDistance)
    {
        var viewerDistanceFromNearestEdge = Mathf.Sqrt(_bounds.SqrDistance(viewerPosition));
        var visible = viewerDistanceFromNearestEdge <= maxViewDistance;
        SetVisible(visible);
    }

    public void SetVisible(bool visible)
    {
        _meshObject.SetActive(visible);
    }

    public bool IsVisible()
    {
        return _meshObject.activeSelf;
    }
}

public struct TerrainChunkRenderData
{
    public Texture2D Texture;
    public MeshData Mesh;

    public TerrainChunkRenderData(Texture2D texture, MeshData mesh)
    {
        Texture = texture;
        this.Mesh = mesh;
    }
}