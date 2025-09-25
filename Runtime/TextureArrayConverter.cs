using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using VRC.SDKBase;

namespace cc.dingemans.bigibas123.bulkmaterialgenerators.Runtime
{
    [AddComponentMenu("Convert Texturearray shader to multiple-material")]
    [RequireComponent(typeof(Renderer))]
    public class TextureArrayConverter : MonoBehaviour, IEditorOnly
    {
        public Renderer Renderer => gameObject.GetComponent<Renderer>();
        public List<TextureArrayConverterMaterialSlotReference> slots = new();

        public List<TextureArrayConverterMaterialSlotReference> PossibleTargets => Renderer.sharedMaterials
            .Select(((_, i) => new TextureArrayConverterMaterialSlotReference() { renderer = Renderer, slot = i }))
            .Where(msr => ContainsTextureArrayProperty(msr.Material))
            .ToList();

        private bool ContainsTextureArrayProperty(Material material)
        {
            return material.GetTexturePropertyNames()
                .Select(propName => material.shader.FindPropertyIndex(propName))
                .Where(id => material.shader.GetPropertyCount() > id &&
                             material.shader.GetPropertyType(id) == ShaderPropertyType.Texture)
                .Select(id => material.shader.GetPropertyTextureDimension(id))
                .Any(dim => dim == TextureDimension.Tex2DArray);
        }
    }

    [Serializable]
    public class TextureArrayConverterMaterialSlotReference : MaterialSlotTarget
    {
        public string menuPath;
        public string sourceProperty;
        public Shader targetShader;
        public string targetProperty;
        
        public Texture2DArray SourceTextureArray => Material.GetTexture(sourceProperty) as Texture2DArray;

        public List<string> PossbileSourceProperties =>
            Shader != null ? Enumerable.Range(0, Shader.GetPropertyCount())
            .Where(id => Shader.GetPropertyType(id) == ShaderPropertyType.Texture)
            .Where(id => Shader.GetPropertyTextureDimension(id) == TextureDimension.Tex2DArray)
            .Select(id => Shader.GetPropertyName(id)).ToList() : new List<string>();

        public List<string> PossibleTargetProperties => targetShader != null ?
            Enumerable.Range(0, targetShader.GetPropertyCount())
                .Where(id => targetShader.GetPropertyType(id) == ShaderPropertyType.Texture)
                .Where(id => targetShader.GetPropertyTextureDimension(id) == TextureDimension.Tex2D)
                .Select(id => targetShader.GetPropertyName(id)).ToList() : new List<string>();
    }
}