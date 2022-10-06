using UnityEngine;

public class Noise : MonoBehaviour
{
    public enum NormalizeMode{
        Local,
        Global
    }

    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, NoiseSettings settings, Vector2 sampleCenter){

        float[,] NoiseMap = new float[mapWidth,mapHeight];
        // System.Random prng = new System.Random(settings.seed);
        System.Random prng = new System.Random( settings.seed);
        Vector2[] octaveOffsets = new Vector2[settings.octaves];

        float maxPossibleHeight = 0;
        float amplitude = 1;
        float frequency = 1;

        for (int i = 0; i < settings.octaves; i++)
        {
            float offsetX = prng.Next(-10000,10000) + settings.offset.x + sampleCenter.x;
            float offsetY = prng.Next(-10000,10000) - settings.offset.y - sampleCenter.y;
            octaveOffsets[i] = new Vector2(offsetX,offsetY);

            maxPossibleHeight += amplitude;
            amplitude+=settings.persistence;
        }


        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth/2f;
        float halfHeight = mapHeight/2f;


        for (int y = 0; y < mapHeight; y++){
            for (int x = 0; x < mapWidth; x++){

                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < settings.octaves; i++)
                {
                    float sampleX = (x-halfWidth + octaveOffsets[i].x) / settings.scale * frequency;
                    float sampleY = (y-halfHeight + octaveOffsets[i].y) / settings.scale * frequency;
                    float perlinValue = Mathf.PerlinNoise(sampleX,sampleY) * 2 - 1;
                    noiseHeight += perlinValue*amplitude;

                    amplitude *= settings.persistence;
                    frequency *= settings.lacunarity;
                }
                if(noiseHeight>maxLocalNoiseHeight) maxLocalNoiseHeight = noiseHeight;
                if(noiseHeight<minLocalNoiseHeight) minLocalNoiseHeight = noiseHeight;
                NoiseMap[x,y] = noiseHeight;
                
                
                if(settings.normalizeMode == NormalizeMode.Global){
                    float normalizedHeight = (NoiseMap[x,y] + 1) / (2f * maxPossibleHeight / 3.75f);
                    NoiseMap[x,y] = Mathf.Clamp(normalizedHeight,0,int.MaxValue);
                }
            }
        }

        if(settings.normalizeMode == NormalizeMode.Local){
            for (int y = 0; y < mapHeight; y++){
                for (int x = 0; x < mapWidth; x++){
                        NoiseMap[x,y] = Mathf.InverseLerp(minLocalNoiseHeight,maxLocalNoiseHeight,NoiseMap[x,y]);
                }
            }
        }
        return NoiseMap;
    }
}

[System.Serializable]
public class NoiseSettings
{
    
    public Noise.NormalizeMode normalizeMode;
    public float scale = 60;

    public int octaves = 6;

    [Range(0, 1)] public float persistence = 0.6f;
    public float lacunarity = 2;
    
    public int seed;
    public Vector2 offset;

    public void ValidateValues()
    {
        scale = Mathf.Max(scale,0.01f);
        octaves = Mathf.Max(octaves, 1);
        lacunarity = Mathf.Max(lacunarity, 1);
        persistence = Mathf.Clamp01(persistence);
    }
}
