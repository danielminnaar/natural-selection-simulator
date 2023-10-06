using UnityEngine;

public class TerrainTextureGenerator : MonoBehaviour
{
    public Color grassColor = Color.green; // Color representing grass
    public Color dirtColor = Color.yellow; // Color representing dirt
    public float scale = 10f; // Scale of the noise
    private int width;  // Width of the texture
    private int height; // Height of the texture

    void Start()
    {


        // Getting the size of the plane object and adjusting texture resolution
        width = Mathf.RoundToInt(transform.localScale.x * 10); // Unity's default plane size is 10
        height = Mathf.RoundToInt(transform.localScale.z * 10); // Using Z because the plane is oriented on the XZ plane

        // Applying generated texture
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.mainTexture = GenerateTexture();
    }

    Texture2D GenerateTexture()
    {
        Texture2D texture = new Texture2D(width, height);
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                float pX = (float)x / width * scale;
                float pY = (float)y / height * scale;

                float noiseValue = Mathf.PerlinNoise(pX, pY);
                Color color = Color.Lerp(dirtColor, grassColor, noiseValue);
                texture.SetPixel(x, y, color);
            }
        }
        texture.Apply();
        return texture;
    }
}