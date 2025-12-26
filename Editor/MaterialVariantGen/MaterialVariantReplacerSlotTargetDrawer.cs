#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace cc.dingemans.bigibas123.bulkmaterialgenerators.Editor.MaterialVariantGen
{
    [CustomPropertyDrawer(typeof(Runtime.MaterialVariantGen.MaterialVariantReplacerSlotTarget))]
    public class MaterialVariantReplacerSlotTargetDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var curValue = property.boxedValue as Runtime.MaterialVariantGen.MaterialVariantReplacerSlotTarget;
            var multiplier = 0;
            if (curValue == null)
            {
                multiplier = 0;
            }

            else if (!curValue.renderer)
            {
                multiplier = 1;
            }
            else
            {
                multiplier = 5;
            }

            return multiplier * EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var curValue = property.boxedValue as Runtime.MaterialVariantGen.MaterialVariantReplacerSlotTarget;
            if (curValue == null) return;

            string name = "Nothing selected";
            if (curValue.Material)
            {
                name = curValue.Material.name;
            }
            else if (curValue.renderer)
            {
                name = curValue.renderer?.name;
            }

            label.text = $"Slot: {name}";
            label = EditorGUI.BeginProperty(position, label, property);
            {
                position.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField(position, label);
                position.y += EditorGUIUtility.singleLineHeight;
                if (!curValue.renderer) return;

                var renderer = curValue.renderer;
                var slot = curValue.slot;
                var menuPath = curValue.menuPath;
                var textures = curValue.textures;
                var targetProperty = curValue.targetProperty;

                EditorGUI.BeginChangeCheck();

                renderer = EditorGUI.ObjectField(position, new GUIContent("Target Renderer"), renderer,
                    typeof(Renderer), false) as Renderer;
                position.y += EditorGUIUtility.singleLineHeight;

                slot = EditorGUI.IntSlider(position, "Slot", slot, 0, curValue.renderer.sharedMaterials.Length - 1);
                position.y += EditorGUIUtility.singleLineHeight;

                menuPath = EditorGUI.TextField(position, "Menu Path", menuPath);
                position.y += EditorGUIUtility.singleLineHeight;


                if (curValue.Material?.shader)
                {
                    var possibleDestProperties = curValue.PossibleTargetProperties;
                    possibleDestProperties.Insert(0, "Select...");
                    int selected = possibleDestProperties.IndexOf(targetProperty);
                    selected = (selected == -1 ? 0 : selected);
                    selected = EditorGUI.Popup(position, "Target shader property:", selected,
                        possibleDestProperties.ToArray());
                    position.y += EditorGUIUtility.singleLineHeight;

                    targetProperty = selected == 0 ? null : possibleDestProperties[selected];
                }

                var texturesProperty =
                    property.FindPropertyRelative(nameof(Runtime.MaterialVariantGen.MaterialVariantReplacerSlotTarget
                        .textures));
                EditorGUILayout.PropertyField(texturesProperty, new GUIContent("Textures"));
                property.serializedObject.ApplyModifiedProperties();


                if (EditorGUI.EndChangeCheck())
                {
                    var boxValue =
                        (property.boxedValue as Runtime.MaterialVariantGen.MaterialVariantReplacerSlotTarget);
                    boxValue.renderer = renderer;
                    boxValue.slot = slot;
                    boxValue.menuPath = menuPath;
                    boxValue.targetProperty = targetProperty;
                    property.boxedValue = boxValue;
                }
            }
        }
    }
}
#endif