using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using VRC.SDKBase;

namespace cc.dingemans.bigibas123.bulkmaterialgenerators.Runtime
{
    [AddComponentMenu("Generate Materials from list of textures")]
    [RequireComponent(typeof(Renderer))]
    public class MaterialVariantGen : MonoBehaviour, IEditorOnly
    {
        public Renderer Renderer => gameObject.GetComponent<Renderer>();
        public List<MaterialVariantReplacerSlotTarget> slots = new();
        
        public List<MaterialVariantReplacerSlotTarget> PossibleTargets => Renderer.sharedMaterials
            .Select(((_, i) => new MaterialVariantReplacerSlotTarget() { renderer = Renderer, slot = i }))
            .Where(msr => msr.PossibleTargetProperties.Count > 0)
            .ToList();

        [Serializable]
        public class MaterialVariantReplacerSlotTarget : MaterialSlotTarget
        {
            public string menuPath;
            public string targetProperty;
            public List<Texture> textures = new List<Texture>();
            
            // Get the dimension of the textures list, any means empty, null means mixed
            public TextureDimension? Dimension =>
                textures.Aggregate(TextureDimension.Any,
                    (aggregate, texture) =>
                    {
                        return aggregate == TextureDimension.Any
                            ?
                            texture == null ? TextureDimension.Any : texture.dimension
                            : texture == null
                                ? aggregate
                                : texture.dimension == aggregate
                                    ? aggregate
                                    : null;
                    },
                    (TextureDimension? dim) => dim);

            public List<string> PossibleTargetProperties => Shader != null
                ? Enumerable.Range(0, Shader.GetPropertyCount())
                    .Where(id => Shader.GetPropertyType(id) == ShaderPropertyType.Texture)
                    .Where(id =>
                    {
                        var textureDimension = Dimension;
                        var shaderDim = Shader.GetPropertyTextureDimension(id);
                        return textureDimension == TextureDimension.Any || textureDimension == shaderDim;
                    })
                    .Select(id => Shader.GetPropertyName(id)).ToList()
                : new List<string>();
            
        }
    }
}