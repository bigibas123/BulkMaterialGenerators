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
        public List<MaterialSlotReference> slots = new();

        public List<MaterialSlotReference> PossibleTargets => Renderer.sharedMaterials
            .Select(((_, i) => new MaterialSlotReference() { renderer = Renderer, slot = i }))
            .Where(msr => ContainsTextureArrayProperty(msr.SourceMaterial))
            .ToList();

        private bool ContainsTextureArrayProperty(Material material)
        {
            return material.GetTexturePropertyNames()
                .Select(propName => material.shader.FindPropertyIndex(propName))
                .Where(id => material.shader.GetPropertyType(id) == ShaderPropertyType.Texture)
                .Select(id => material.shader.GetPropertyTextureDimension(id))
                .Any(dim => dim == TextureDimension.Tex2DArray);
        }
    }

    [Serializable]
    public class MaterialSlotReference
    {
        public bool enabled;
        public Renderer renderer;
        public int slot;
        public string menuPath;
        public string sourceProperty;
        public Shader targetShader;
        public string targetProperty;

        public Shader SourceShader => renderer.sharedMaterials[slot].shader;
        
        public Texture2DArray SourceTextureArray => SourceMaterial.GetTexture(sourceProperty) as Texture2DArray;
        public Material SourceMaterial => renderer != null && renderer.sharedMaterials.Length > slot
            ? renderer.sharedMaterials[slot]
            : null;

        public List<string> PossbileSourceProperties =>
            SourceShader != null ? Enumerable.Range(0, SourceShader.GetPropertyCount())
            .Where(id => SourceShader.GetPropertyType(id) == ShaderPropertyType.Texture)
            .Where(id => SourceShader.GetPropertyTextureDimension(id) == TextureDimension.Tex2DArray)
            .Select(id => SourceShader.GetPropertyName(id)).ToList() : new List<string>();

        public List<string> PossibleTargetProperties => targetShader != null ?
            Enumerable.Range(0, targetShader.GetPropertyCount())
                .Where(id => targetShader.GetPropertyType(id) == ShaderPropertyType.Texture)
                .Where(id => targetShader.GetPropertyTextureDimension(id) == TextureDimension.Tex2D)
                .Select(id => targetShader.GetPropertyName(id)).ToList() : new List<string>();
    }
}