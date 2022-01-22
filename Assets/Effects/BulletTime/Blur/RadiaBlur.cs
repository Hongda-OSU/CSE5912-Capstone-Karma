using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode] 
[RequireComponent(typeof(Camera))] 
public class RadiaBlur : MonoBehaviour
{

    public Shader RadiaBlurShader;
    public Material RadiaBlurMaterial;

    [Range(1, 100)]
    public float Level = 10;

    [Range(0, 1)]
    public float BufferRadius = 0.5f;

    [Range(0, 1)]
    public float CenterX = 0.5f;

    [Range(0, 1)]
    public float CenterY = 0.5f;

  

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (RadiaBlurMaterial != null)
        {
            RadiaBlurMaterial.SetFloat("_Level", Level);
            RadiaBlurMaterial.SetFloat("_CenterX", CenterX);
            RadiaBlurMaterial.SetFloat("_CenterY", CenterY);
            RadiaBlurMaterial.SetFloat("_BufferRadius", BufferRadius);

            Graphics.Blit(src, dest, RadiaBlurMaterial);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}

