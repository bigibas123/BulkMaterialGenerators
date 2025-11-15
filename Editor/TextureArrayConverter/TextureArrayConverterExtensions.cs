using System;
using System.Collections.Generic;
using System.Linq;
using cc.dingemans.bigibas123.bulkmaterialgenerators.Editor.Utils;
using com.vrcfury.api;
using com.vrcfury.api.Actions;
using com.vrcfury.api.Components;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

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

        public static void Process(this Runtime.TextureArrayConverterMaterialSlotReference setting)
        {
            var resultMaterials = setting.SourceTextureArray
                .ToTexture2DList()
                .ToMaterials(setting.Material, setting.sourceProperty, setting.targetShader,
                    setting.targetProperty, setting.flipProperty);

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
            string sourceProperty, Shader destShader, string destProperty, string flipProperty)
        {
            return array.Select((tex, i) =>
            {
                var mat = new Material(sourceMaterial)
                {
                    shader = destShader,
                    name = $"{sourceMaterial.name}[{i}]"
                };
                mat.SetTexture(sourceProperty, null);
                mat.SetTexture(destProperty, tex);
                if (string.IsNullOrWhiteSpace(flipProperty)) return mat;

                // Flip property logic
                var propIndex = destShader.FindPropertyIndex(flipProperty);
                var propId = Shader.PropertyToID(flipProperty);
                var type = destShader.GetPropertyType(propIndex);
                switch (type)
                {
                    case ShaderPropertyType.Int:
                    {
                        int iValue = mat.GetInt(propId);
                        if (iValue > 0)
                        {
                            mat.SetInt(propId, 0);
                        }
                        else
                        {
                            mat.SetInt(propId, 1);
                        }

                        break;
                    }
                    case ShaderPropertyType.Float:
                    case ShaderPropertyType.Range:
                        float fValue = mat.GetFloat(propId);
                        if (fValue > Single.Epsilon)
                        {
                            mat.SetFloat(propId, 0);
                        }
                        else
                        {
                            mat.SetFloat(propId, 1);
                        }

                        break;

                    case ShaderPropertyType.Color:
                    case ShaderPropertyType.Vector:
                    case ShaderPropertyType.Texture:
                    default:
                        throw new ArgumentOutOfRangeException(flipProperty,
                            $"property to flip is of non-flippable type: {nameof(type)}");
                }

                if (mat.enabledKeywords.Select(kw => { return kw.name; }).Contains("MULTI_TEXTURE"))
                {
                    mat.DisableKeyword("MULTI_TEXTURE");
                }

                return mat;
            }).ToList();
        }
    }
}