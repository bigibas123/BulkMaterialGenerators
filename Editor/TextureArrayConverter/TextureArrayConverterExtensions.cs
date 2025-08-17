using System.Collections.Generic;
using System.Linq;
using com.vrcfury.api;
using com.vrcfury.api.Actions;
using com.vrcfury.api.Components;
using UnityEditor;
using UnityEngine;

namespace cc.dingemans.bigibas123.bulkmaterialgenerators.Editor.TextureArrayConverter
{
    public static class TextureArrayConverterExtensions
    {
        public static void Process(this Runtime.TextureArrayConverter settings)
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

        public static void Process(this Runtime.MaterialSlotReference setting)
        {
            var resultMaterials = setting.SourceTextureArray
                .ToTexture2DList()
                .ToMaterials(setting.SourceMaterial, setting.sourceProperty, setting.targetShader,
                    setting.targetProperty);

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

        public static List<Texture2D> ToTexture2DList(this Texture2DArray array)
        {
            List<Texture2D> layers = new List<Texture2D>(array.depth);
            for (int i = 0; i < array.depth; i++)
            {
                layers.Insert(i,
                    new Texture2D(array.width, array.height, array.format, array.mipmapCount, !array.isDataSRGB, true));
                layers[i].name = $"{array.name}[{i}]";
                layers[i].alphaIsTransparency = true;
                layers[i].wrapMode = array.wrapMode;
                layers[i].wrapModeU = array.wrapModeU;
                layers[i].wrapModeV = array.wrapModeV;
                layers[i].wrapModeW = array.wrapModeW;
                layers[i].filterMode = array.filterMode;
                layers[i].anisoLevel = array.anisoLevel;
                layers[i].mipMapBias = array.mipMapBias;
                
                for (int mip = 0; mip < array.mipmapCount; mip++)
                {
                    Graphics.CopyTexture(array, i, mip, layers[i], 0, mip);
                }
            }
            return layers;
        }


        public static List<Material> ToMaterials(this List<Texture2D> array, Material sourceMaterial,
            string sourceProperty, Shader destShader, string destProperty)
        {
            return array.Select((tex, i) =>
            {
                var mat = new Material(sourceMaterial)
                {
                    shader = destShader,
                    name = $"{sourceMaterial.name}[{i}]"
                };
                mat.SetTexture(destProperty, tex);
                mat.SetTexture(sourceProperty, tex);
                return mat;
            }).ToList();
        }

        public static AnimationClip CreateMaterialSwap(this Material material, int slot)
        {
            AnimationClip clip = new AnimationClip();
            clip.frameRate = 30;

            // The property path for the first material slot
            string propertyPath = $"m_Materials.Array.data[{slot}]";

            // Create an ObjectReferenceCurveBinding for material swap
            var binding = new EditorCurveBinding
            {
                type = typeof(Renderer),
                path = "",
                propertyName = propertyPath
            };

            // Define keyframes: at t=0 use materialA, at t=1s use materialB
            ObjectReferenceKeyframe[] keyframes = new ObjectReferenceKeyframe[1];

            keyframes[0] = new ObjectReferenceKeyframe
            {
                time = 0f,
                value = material
            };

            // Assign the keyframes to the clip
            AnimationUtility.SetObjectReferenceCurve(clip, binding, keyframes);
            return clip;
        }
    }
}