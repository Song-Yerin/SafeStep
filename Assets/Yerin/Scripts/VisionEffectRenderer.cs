// VisionEffectRenderer.cs - µø¿œ
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class VisionEffectRenderer : MonoBehaviour
{
    public Material effectMaterial;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (effectMaterial != null)
        {
            Graphics.Blit(source, destination, effectMaterial);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
}