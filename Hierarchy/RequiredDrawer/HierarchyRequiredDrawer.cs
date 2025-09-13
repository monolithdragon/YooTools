using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace YooTools.Hierarchy.RequiredDrawer {
	[InitializeOnLoad]
    public static class HierarchyRequiredDrawer {
        private static readonly GUIContent RequiredIcon = EditorGUIUtility.IconContent("console.erroricon.sml", "One or more required fields are missing or empty!");
		private static readonly Dictionary<Type, FieldInfo[]> CachedFieldInfo = new();

		static HierarchyRequiredDrawer() => EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemOnGUI;

		private static void OnHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect) {
			if (EditorUtility.InstanceIDToObject(instanceID) is not GameObject gameObject) {
				return;
			}
			
			var components = gameObject.GetComponents<Component>();

			foreach (var component in components) {
				if (component == null) {
					continue;
				}

				var fields = GetCachedFieldsWithRequireAttribute(component.GetType());
				
				if (fields.Any(field => IsFieldUnassigned(field.GetValue(component)))) {
					// Icon of the hierarchy field is in the right place
					var iconRect = new Rect(selectionRect.xMax - 20f, selectionRect.y + 1f, 16f, 16f);
					GUI.Label(iconRect, RequiredIcon);

					break;
				}
			}
		}

		private static bool IsFieldUnassigned(object? fieldValue) {
			switch (fieldValue) {
				case null:
				case UnityEngine.Object obj when obj == null:
				case string stringValue when string.IsNullOrEmpty(stringValue):
					return true;
				case IEnumerable enumerable: {
					foreach (object? item in enumerable) {
						if (item == null || item.Equals(null)) return true;
					}

					break;
				}
			}

			return false;
		}

		private static FieldInfo[] GetCachedFieldsWithRequireAttribute(Type componentType) {
			if (!CachedFieldInfo.TryGetValue(componentType, out var fields)) {
				fields = componentType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				var requiredFields = new List<FieldInfo>();

				foreach (var field in fields) {
					bool isSerialized = field.IsPublic || field.IsDefined(typeof(SerializeField), false);
					bool isRequired = field.IsDefined(typeof(RequiredAttribute), false);

					if (isSerialized && isRequired) {
						requiredFields.Add(field);
					}
				}

				fields = requiredFields.ToArray();
				CachedFieldInfo.Add(componentType, fields);
			}

			return fields;
		}
    }
}