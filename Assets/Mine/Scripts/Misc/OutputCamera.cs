using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutputCamera : MonoBehaviour
{
    private Camera cam;
    public static RenderTexture currentTexture;
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        RedefineRenderTexture(256);
    }

    public void RedefineRenderTexture(int squareSize)
    {
        currentTexture = new RenderTexture(squareSize, squareSize, 32, RenderTextureFormat.ARGB32);
        cam.targetTexture = currentTexture;
    }

    public void SetSize(float orthoSize)
    {
        cam.orthographicSize = orthoSize;
    }

    /// <summary>
    /// Get the render texture converted to Texture2D
    /// </summary>
    /// <returns></returns>
    public Texture2D ConvertedTex()
    {

        Texture2D convertedTexture = new Texture2D(currentTexture.width, currentTexture.height, TextureFormat.RGBA32, false);
        RenderTexture.active = currentTexture;

        convertedTexture.ReadPixels(new Rect(0, 0, currentTexture.width, currentTexture.height), 0, 0);
        return convertedTexture;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
