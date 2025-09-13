using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace YooTools.Hierarchy.RequiredDrawer.Editor {
    [CustomPropertyDrawer(typeof(RequiredAttribute))]
    public class RequiredDrawer : PropertyDrawer  {
        private readonly GUIContent _errorIcon = EditorGUIUtility.IconContent("console.erroricon.sml");

		public override VisualElement CreatePropertyGUI(SerializedProperty property) {
			var root = new VisualElement {
				style = {
					flexDirection = FlexDirection.Row, alignItems = Align.Center, paddingTop = 2, paddingBottom = 2
				}
			};

			var propertyField = new PropertyField(property, property.displayName) {
				style = {
					flexGrow = 1f
				}
			};

			root.Add(propertyField);

			var icon = new Image {
				image = _errorIcon.image,
				tooltip = "This field is required and is either missing or empty!",
				style = {
					width = 16, height = 16, marginLeft = 4
				}
			};

			root.Add(icon);

			RefreshFieldVisuals(propertyField, icon, property);

			// Update if value changed
			propertyField.RegisterValueChangeCallback(_ => {
					RefreshFieldVisuals(propertyField, icon, property);
				}
			);
			
			EditorApplication.RepaintHierarchyWindow();

			return root;
		}

		private static void RefreshFieldVisuals(PropertyField propertyField, VisualElement icon, SerializedProperty property) {
			bool isUnassigned = property.propertyType switch {
				SerializedPropertyType.ObjectReference => property.objectReferenceValue == null,
				SerializedPropertyType.ExposedReference => property.exposedReferenceValue == null,
				SerializedPropertyType.AnimationCurve => property.animationCurveValue == null || property.animationCurveValue.length == 0,
				SerializedPropertyType.String => string.IsNullOrEmpty(property.stringValue),
				var _ => false
			};

			// Icon visibility
			icon.style.display = isUnassigned
				? DisplayStyle.Flex
				: DisplayStyle.None;

			// Add red border if field is empty
			if (isUnassigned) {
				propertyField.style.borderTopColor = Color.red;
				propertyField.style.borderBottomColor = Color.red;
				propertyField.style.borderLeftColor = Color.red;
				propertyField.style.borderRightColor = Color.red;
				propertyField.style.borderTopWidth = 1;
				propertyField.style.borderBottomWidth = 1;
				propertyField.style.borderLeftWidth = 1;
				propertyField.style.borderRightWidth = 1;
			} else {
				propertyField.style.borderTopWidth = 0;
				propertyField.style.borderBottomWidth = 0;
				propertyField.style.borderLeftWidth = 0;
				propertyField.style.borderRightWidth = 0;
			}
		}
    }
}