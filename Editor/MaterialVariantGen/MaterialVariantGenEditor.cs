using System.Linq;
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
                var targetRenderer = setting.GetComponent<Renderer>();
                foreach (var slot in setting.slots.Where(slot => slot.renderer == null)){
                    slot.renderer = targetRenderer;
                }
            }

            var slotsUi = new PropertyField(_slotsProp, "Settings");
            //checkBox.RegisterValueChangeCallback(evt => { ReDrawMaterials(behavior); });

            inspector.Add(slotsUi);

            return inspector;
        }
    }
}