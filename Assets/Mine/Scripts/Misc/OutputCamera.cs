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

    /// <summary>
    /// Should be called on the final spritesheet if black is set to alpha.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public Texture2D TextureToAlpha(Texture2D source)
    {
        for(int x = 0; x < source.width; x++)
        {
            for(int y = 0; y < source.height; y++)
            {
                Color c = source.GetPixel(x, y);
                c = ConvertColorToAlpha(c);
                source.SetPixel(x, y, c);
            }
        }
        source.Apply();
        return source;
    }

    Color ConvertColorToAlpha(Color inputColor)
    {
        // Calculate alpha based on the luminance of the input color
        float alpha = inputColor.grayscale;

        // Create a new color with the same RGB components as input, and alpha based on the luminance
        Color outputColor = new Color(inputColor.r, inputColor.g, inputColor.b, alpha);

        return outputColor;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
