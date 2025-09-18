using System.Collections.Generic;
using System.Linq;
using cc.dingemans.bigibas123.bulkmaterialgenerators.Editor.Utils;
using com.vrcfury.api;
using com.vrcfury.api.Actions;
using com.vrcfury.api.Components;
using UnityEditor;
using UnityEngine;

namespace cc.dingemans.bigibas123.bulkmaterialgenerators.Editor.MaterialVariantGen
{
    public static class MaterialVariantGenExtensions
    {
        public static void Process(this Runtime.MaterialVariantGen settings)
        {
            foreach (var slot in settings.slots)
            {
                if (slot.enabled)
                {
                    slot.Process();
                }
            }

            Object.DestroyImmediate(settings);
        }

        public static void Process(this Runtime.MaterialVariantGen.MaterialVariantReplacerSlotTarget setting)
        {
            var resultMaterials = setting.textures.ToMaterials(setting.Material,setting.targetProperty);
           
            var sharedMats = setting.renderer.sharedMaterials;
            sharedMats[setting.slot] = resultMaterials.First();
            setting.renderer.sharedMaterials = sharedMats;
            
            FuryToggle toggle = FuryComponents.CreateToggle(setting.renderer.gameObject);
            toggle.SetMenuPath(setting.menuPath);
            toggle.SetSlider();
            
            var mainActions = toggle.GetActions();
            FuryFlipbookBuilder flipbook = mainActions.AddFlipbookBuilder();
            
            foreach (var material in resultMaterials)
            {
                var page = flipbook.AddPage();
                var actions = page.GetActions();
                var clip = material.CreateMaterialSwap(setting.slot);
                actions.AddAnimationClip(clip);
            }
        }
        
        public static List<Material> ToMaterials(this List<Texture> array, Material sourceMaterial, string destProperty)
        {
            return array.Select((tex, i) =>
            {
                var mat = new Material(sourceMaterial)
                {
                    name = $"{sourceMaterial.name}[{i}]"
                };
                mat.SetTexture(destProperty, tex);
                return mat;
            }).ToList();
        }
    }
}