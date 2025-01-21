using UnityEngine;
using UnityEditor;
using System.IO;

public class ParticleTextureGenerator : EditorWindow
{
    private int textureSize = 128;
    private float softness = 0.3f;
    private float noiseScale = 4f;
    private float noiseAmount = 0.3f;
    private string textureName = "ParticleTexture";
    private bool isCircular = true;

    [MenuItem("Tools/Particle Texture Generator")]
    public static void ShowWindow()
    {
        GetWindow<ParticleTextureGenerator>("Particle Texture Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Particle Texture Settings", EditorStyles.boldLabel);

        textureSize = EditorGUILayout.IntField("Texture Size", textureSize);
        softness = EditorGUILayout.Slider("Edge Softness", softness, 0f, 1f);
        isCircular = EditorGUILayout.Toggle("Circular Shape", isCircular);

        if (!isCircular)
        {
            noiseScale = EditorGUILayout.FloatField("Noise Scale", noiseScale);
            noiseAmount = EditorGUILayout.Slider("Noise Amount", noiseAmount, 0f, 1f);
        }

        textureName = EditorGUILayout.TextField("Texture Name", textureName);

        if (GUILayout.Button("Generate Texture"))
        {
            if (isCircular)
                GenerateCircularTexture();
            else
                GenerateBlobTexture();
        }
    }

    private void GenerateCircularTexture()
    {
        Texture2D texture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);
        Vector2 center = new Vector2(textureSize / 2f, textureSize / 2f);
        float radius = textureSize / 2f;

        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                float alpha = 1f - Mathf.Clamp01(distance / (radius * (1f - softness)));
                texture.SetPixel(x, y, new Color(1, 1, 1, alpha));
            }
        }

        SaveTexture(texture);
    }

    private void GenerateBlobTexture()
    {
        Texture2D texture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);
        Vector2 center = new Vector2(textureSize / 2f, textureSize / 2f);
        float radius = textureSize / 2f;

        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);

                // Base circle shape
                float baseAlpha = 1f - Mathf.Clamp01(distance / (radius * (1f - softness)));

                // Add Perlin noise
                float noise = Mathf.PerlinNoise(
                    x * noiseScale / textureSize,
                    y * noiseScale / textureSize
                );

                // Mix base shape with noise
                float alpha = Mathf.Lerp(baseAlpha, baseAlpha * noise, noiseAmount);
                alpha = Mathf.Clamp01(alpha);

                texture.SetPixel(x, y, new Color(1, 1, 1, alpha));
            }
        }

        SaveTexture(texture);
    }

    private void SaveTexture(Texture2D texture)
    {
        texture.Apply();

        // Save the texture to the Assets folder
        string path = $"Assets/{textureName}.png";
        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(path, bytes);
        AssetDatabase.Refresh();

        // Set texture import settings
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Default;
            importer.filterMode = FilterMode.Bilinear;
            importer.alphaIsTransparency = true;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.SaveAndReimport();
        }

        Debug.Log($"Texture saved at: {path}");
    }
}