using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FalloffGenerator {
    public static float[,] GenerateFalloffMap(int size, Vector2 falloffParameters){
        float [,] map = new float[size,size];

        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                float x = i / (float)size * 2 - 1;
                float y = j / (float)size * 2 - 1;

                // float value = Mathf.Max(falloffCurve.Evaluate(Mathf.Abs(x)),falloffCurve.Evaluate(Mathf.Abs(y)));
                float value = Mathf.Max(Mathf.Abs(x),Mathf.Abs(y));
                map[i,j] = Evaluate(value,falloffParameters);
            }
        }
        return map;
    }
    public static float Evaluate(float x, Vector2 parameters){
        float a = parameters.x;
        float b = parameters.y;

        float value = Mathf.Pow(x,a) / (Mathf.Pow(x,a) + Mathf.Pow(b - (b * x),a));

        return value;
    }
}
