using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace cc.dingemans.bigibas123.bulkmaterialgenerators.Editor.MaterialVariantGen
{
    [CustomEditor(typeof(Runtime.MaterialVariantGen))]
    [CanEditMultipleObjects]
    public class MaterialVariantGenEditor : UnityEditor.Editor
    {
        SerializedProperty _slotsProp;

        void OnEnable()
        {
            _slotsProp =
                serializedObject.FindProperty(nameof(Runtime.MaterialVariantGen.slots));
        }

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement inspector = new VisualElement();
            foreach (var tgt in targets)
            {
                if (tgt is not Runtime.MaterialVariantGen setting) continue;
                
                
                var targetRenderer = setting.Renderer;
                setting.slots.ForEach(slot => slot.renderer = targetRenderer);

                int posibleCount = setting.PossibleTargets.Count;
                int slotsCount = setting.slots.Count;
                if (posibleCount < slotsCount)
                {
                    setting.slots.RemoveRange(posibleCount,
                        slotsCount - posibleCount);
                }
                else if (posibleCount > slotsCount)
                {
                    foreach (var possibleTarget in setting.PossibleTargets)
                    {
                        bool found = false;

                        foreach (var existingSlot in setting.slots)
                        {
                            if (possibleTarget.renderer == existingSlot.renderer &&
                                possibleTarget.slot == existingSlot.slot)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            setting.slots.Add(possibleTarget);
                        }
                    }
                }
            }

            var slotsUi = new PropertyField(_slotsProp, "Settings");
            //checkBox.RegisterValueChangeCallback(evt => { ReDrawMaterials(behavior); });

            inspector.Add(slotsUi);

            return inspector;
        }
    }
}