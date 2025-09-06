using System;
using UnityEngine;

namespace cc.dingemans.bigibas123.bulkmaterialgenerators.Runtime
{
    [Serializable]
    public class MaterialSlotTarget
    {
        public bool enabled;
        public Renderer renderer;
        public int slot;
        
        
        public Material Material => renderer != null && renderer.sharedMaterials.Length > slot
            ? renderer.sharedMaterials[slot]
            : null;
            
        public Shader Shader => Material.shader;
        

    }
}