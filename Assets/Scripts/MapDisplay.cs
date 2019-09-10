using UnityEngine;

public interface IMapDisplayContainer
{
    void DrawNoiseMap(float[,] noiseMap);
}
public class MapDisplay : MonoBehaviour, IMapDisplayContainer
{
    
    public Renderer textureRenderer;
    private IMapDisplayController _mapDisplayController;

    public void Construct(IMapDisplayController mapDisplayController, Renderer mapTextureRenderer)
    {
        this.textureRenderer = mapTextureRenderer;
        _mapDisplayController = mapDisplayController;
    }

    public void DrawNoiseMap(float[,] noiseMap)
    {
        CreateController();
        
        var texture2D = _mapDisplayController.GenerateTexture(noiseMap);
        
        textureRenderer.sharedMaterial.mainTexture = texture2D;
        textureRenderer.transform.localScale = new Vector3(texture2D.width, 1, texture2D.height);
    }

    private void CreateController()
    {
        if (_mapDisplayController is null)
        {
            _mapDisplayController = new MapDisplayController();
        }
    }
}