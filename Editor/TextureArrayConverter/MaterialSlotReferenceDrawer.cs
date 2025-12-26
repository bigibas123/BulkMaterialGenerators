#if UNITY_EDITOR
using cc.dingemans.bigibas123.bulkmaterialgenerators.Runtime;
using UnityEditor;
using UnityEngine;

namespace cc.dingemans.bigibas123.bulkmaterialgenerators.Editor.TextureArrayConverter
{
    [CustomPropertyDrawer(typeof(TextureArrayConverterMaterialSlotReference))]
    public class MaterialSlotReferenceDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var curValue = property.boxedValue as TextureArrayConverterMaterialSlotReference;
            float multiplier = 1;

            if (curValue != null)
            {
                multiplier += 5;
                if (curValue.Shader)
                {
                    ++multiplier;
                }

                if (curValue.targetShader)
                {
                    multiplier += 2;
                }
            }

            return EditorGUIUtility.singleLineHeight * multiplier;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var curValue = property.boxedValue as TextureArrayConverterMaterialSlotReference;
            if (curValue != null)
            {
                var renderer = curValue.renderer;
                var slot = curValue.slot;
                var menuPath = curValue.menuPath;
                var sourceProperty = curValue.sourceProperty;

                var targetShader = curValue.targetShader;
                var targetProperty = curValue.targetProperty;

                var flipProperty = curValue.flipProperty;

                string name = "Nothing selected";
                if (curValue.Material)
                {
                    name = curValue.Material.name;
                }else if (curValue.renderer)
                {
                    name = curValue.renderer?.name;
                }
            
                label.text = $"Slot: {name}";
                label = EditorGUI.BeginProperty(position, label, property);
                {
                    position.height = EditorGUIUtility.singleLineHeight;
                    EditorGUI.LabelField(position, label);
                    position.y += EditorGUIUtility.singleLineHeight;
                    EditorGUI.BeginChangeCheck();
                    

                    renderer = EditorGUI.ObjectField(position, new GUIContent("Target Renderer"), renderer,
                        typeof(Renderer), false) as Renderer;
                    position.y += EditorGUIUtility.singleLineHeight;

                    slot = EditorGUI.IntSlider(position, "Slot", slot, 0,
                        (curValue?.renderer?.sharedMaterials.Length - 1 ?? 0));
                    position.y += EditorGUIUtility.singleLineHeight;

                    menuPath = EditorGUI.TextField(position, "Menu Path", menuPath);
                    position.y += EditorGUIUtility.singleLineHeight;

                    if (curValue.Shader)
                    {
                        var possbileSourceProperties = curValue.PossbileSourceProperties;
                        possbileSourceProperties.Insert(0, "Select...");

                        int selected = possbileSourceProperties.IndexOf(sourceProperty);
                        selected = (selected == -1 ? 0 : selected);

                        selected = EditorGUI.Popup(position, "Source shader property:", selected,
                            possbileSourceProperties.ToArray());
                        position.y += EditorGUIUtility.singleLineHeight;

                        sourceProperty = selected == 0 ? null : possbileSourceProperties[selected];
                    }

                    targetShader = EditorGUI.ObjectField(position,
                        new GUIContent("Target Shader"), targetShader, typeof(Shader), false) as Shader;
                    position.y += EditorGUIUtility.singleLineHeight;

                    if (targetShader)
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

                    if (targetShader)
                    {
                        var possibleFlipProperties = curValue.PossbileFlippableProperties;
                        possibleFlipProperties.Insert(0, "Select...");
                        int selected = possibleFlipProperties.IndexOf(flipProperty);
                        selected = (selected == -1 ? 0 : selected);
                        selected = EditorGUI.Popup(position, "Target shader property to flip:", selected,
                            possibleFlipProperties.ToArray());
                        position.y += EditorGUIUtility.singleLineHeight;

                        flipProperty = selected == 0 ? null : possibleFlipProperties[selected];
                    }


                    if (EditorGUI.EndChangeCheck())
                    {
                        var boxValue = property.boxedValue as TextureArrayConverterMaterialSlotReference;
                        boxValue.renderer = renderer;
                        boxValue.slot = slot;
                        boxValue.menuPath = menuPath;
                        boxValue.sourceProperty = sourceProperty;
                        boxValue.targetShader = targetShader;
                        boxValue.targetProperty = targetProperty;
                        boxValue.flipProperty = flipProperty;
                    }

                    EditorGUI.EndProperty();
                }
            }
        }
    }
}
#endif