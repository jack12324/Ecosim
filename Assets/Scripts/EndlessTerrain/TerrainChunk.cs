using UnityEngine;

public class TerrainChunk
{
    private readonly GameObject _meshObject;
    private Bounds _bounds;

    public MeshRenderer _meshRenderer;
    public MeshFilter _meshFilter;
    public TerrainChunk(Vector2 coordinates, int size, Transform parent, Material material)
    {
        var position = coordinates * size;
        _bounds = new Bounds(position, Vector2.one * size);
        var positionV3 = new Vector3(position.x, 0, position.y);

        _meshObject = new GameObject("Terrain Chunk");
        _meshRenderer = _meshObject.AddComponent<MeshRenderer>();
        _meshRenderer.material = material;
        _meshFilter = _meshObject.AddComponent<MeshFilter>();
        
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