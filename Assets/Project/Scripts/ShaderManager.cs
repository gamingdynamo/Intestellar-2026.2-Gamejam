using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using NaughtyAttributes;

[System.Serializable]
public class PostEffectData
{
    public string featureName;
    public Material material;
}

public class ShaderManager : MonoBehaviour
{
    [SerializeField] private UniversalRendererData rendererData;
    [SerializeField] private List<PostEffectData> postEffects;
    private static readonly int IntensityEffectID = Shader.PropertyToID("_IntensityEffect");

    [Button("Clear All Effects")]
    public void ClearShaderEffects()
    {
        foreach (var effect in postEffects)
        {
            SetEffectIntensity(effect, 0.0f);
        }
    }

    [Button("Set Random Effect (Full Intensity)")]
    public void SetRandomEffectFull()
    {
        SetRandomShaderEffect(1.0f);
    }

    public void SetRandomShaderEffect(float intensity)
    {
        ClearShaderEffects();

        if (postEffects == null || postEffects.Count == 0) return;

        int randomIndex = Random.Range(0, postEffects.Count);
        SetEffectIntensity(postEffects[randomIndex], intensity);
    }

    public void SetEffectIntensity(PostEffectData effect, float intensity)
    {
        if (effect == null || effect.material == null) return;
        
        float clampedIntensity = Mathf.Clamp01(intensity);

        if (clampedIntensity <= 0.001f)
        {
            effect.material.SetFloat(IntensityEffectID, 0.0f);
            SetFeatureActive(effect.featureName, false);
        }
        else
        {
            SetFeatureActive(effect.featureName, true);
            effect.material.SetFloat(IntensityEffectID, clampedIntensity);
        }
    }

    private void SetFeatureActive(string featureName, bool active)
    {
        if (rendererData == null) return;

        foreach (ScriptableRendererFeature feature in rendererData.rendererFeatures)
        {
            if (feature.name != featureName){continue;}
        
            feature.SetActive(active);
            break;

        }
    }
}