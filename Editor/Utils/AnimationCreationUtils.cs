using UnityEditor;
using UnityEngine;

namespace cc.dingemans.bigibas123.bulkmaterialgenerators.Editor.Utils
{
    public static class AnimationCreationUtils
    {
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