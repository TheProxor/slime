using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;


namespace TheProxor.Utils
{
	[CustomPropertyDrawer(typeof(SerializableDictionary), true)]
	public class SerializableDictionaryDrawer : PropertyDrawer
	{
		#region Fields

		private const float INDENT_ITEM = 12f;
		private const float BUTTONS_HEIGHT = 52f;

		private static GUIContent elementIndex;

		private SerializableDictionary sourceDictionary;

		private ReorderableList list;
		private Func<Rect> VisibleRect;
		private Dictionary<int, int> keysFirstIndexes;
		private float contentHeight;

		private bool isSingleLineView = true;
		private readonly Event e = Event.current;

		#endregion



		#region Methods

		private void ContextMenu(Rect rect)
		{
			if (e.type != EventType.MouseDown || e.button != 1 || !rect.Contains(e.mousePosition))
				return;

			e.Use();

			GenericMenu context = new GenericMenu();
			context.AddItem(new GUIContent("Toggle view"), false, ToggleView);
			context.ShowAsContext();
		}


		private void ToggleView()
		{
			isSingleLineView = !sourceDictionary.IsSingleLineView;
			sourceDictionary.IsSingleLineView = isSingleLineView;
		}


		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			#if UNITY_2020_2_OR_NEWER

			ContextMenu(position);

			if (sourceDictionary == null)
			{
				sourceDictionary = property.GetValue<SerializableDictionary>(true);
				isSingleLineView = sourceDictionary.IsSingleLineView;
			}

			keysFirstIndexes = sourceDictionary.GetKeysFirstIndexes();

			if (list == null)
			{
				SerializedProperty listProp = property.FindPropertyRelative("list");
				list = new ReorderableList(property.serializedObject, listProp, true, false, true, true);
				list.drawElementCallback = DrawListItems;
				list.elementHeightCallback = GetItemHeight;
			}

			Rect firstLine = position;
			firstLine.height = EditorGUIUtility.singleLineHeight;
			EditorGUI.PropertyField(firstLine, property, label);

			if (property.isExpanded)
			{
				position.y += firstLine.height;

				if (VisibleRect == null)
				{
					Type tyGUIClip = System.Type.GetType("UnityEngine.GUIClip,UnityEngine");
					if (tyGUIClip != null)
					{
						PropertyInfo piVisibleRect = tyGUIClip.GetProperty("visibleRect", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
						if (piVisibleRect != null)
						{
							MethodInfo getMethod = piVisibleRect.GetGetMethod(true) ?? piVisibleRect.GetGetMethod(false);
							VisibleRect = (Func<Rect>)Delegate.CreateDelegate(typeof(Func<Rect>), getMethod);
						}
					}
				}

				var vRect = VisibleRect();
				vRect.y -= position.y;

				if (elementIndex == null)
				{
					elementIndex = new GUIContent();
				}
				list.DoList(position, vRect);
			}
			#endif

			EditorGUI.EndProperty();
		}


		private void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
		{
			SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);
			var keyProp = element.FindPropertyRelative("Key");
			var valueProp = element.FindPropertyRelative("Value");

			var isKeySimpleType = IsSimpleType(keyProp);
			var isValueSimpleType = IsSimpleType(valueProp);
			var isSingleLineView = this.isSingleLineView || isKeySimpleType && isValueSimpleType;

			var firstIndex = keysFirstIndexes[index];
			var isDuplicate = firstIndex != index;
			SerializedProperty exist = isDuplicate
				? list.serializedProperty.GetArrayElementAtIndex(firstIndex)
				: null;

			EditorGUI.BeginProperty(rect, elementIndex, element);

			if (isDuplicate)
			{
				Rect warnRect = rect;
				warnRect.height = EditorGUIUtility.singleLineHeight;
				EditorGUI.HelpBox(warnRect, $"Key exists in {exist.displayName}", MessageType.Warning);
				rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing / 2f;
				rect.height -= EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			}

			if (isSingleLineView)
			{
				var prevLabelWidth = EditorGUIUtility.labelWidth;

				EditorGUIUtility.labelWidth = 75;

				Rect rect0 = rect;

				float halfWidth = rect0.width / 2f;
				rect0.width = halfWidth;
				rect0.y += 1f;
				rect0.height -= 2f;

				EditorGUIUtility.labelWidth = 30;

				DrawPropertyField(rect0, keyProp, true, !isKeySimpleType);

				rect0.x += halfWidth + 4f;

				DrawPropertyField(rect0, valueProp, true, !isValueSimpleType);

				EditorGUIUtility.labelWidth = prevLabelWidth;
			}
			else
			{
				DrawPropertyField(rect, element, true);
			}

			EditorGUI.EndProperty();
		}


		private static void DrawPropertyField(Rect rect, SerializedProperty element, bool includeChildren, bool indent = true)
		{
			if (indent)
			{
				rect.width -= INDENT_ITEM;
				rect.x += INDENT_ITEM;
			}

			if (element == null)
				EditorGUI.LabelField(rect, "Null");
			else
				EditorGUI.PropertyField(rect, element, true);
		}


		private float GetItemHeight(int index)
		{
			if (list.serializedProperty.arraySize == 0)
			{
				return 0f;
			}

			if (index == 0)
			{
				contentHeight = 0f;
			}

			SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);
			var keyProp = element.FindPropertyRelative("Key");
			var valueProp = element.FindPropertyRelative("Value");

			var height = EditorGUIUtility.singleLineHeight;


			float GetHeight(SerializedProperty property) => property == null
				? EditorGUIUtility.singleLineHeight
				: EditorGUI.GetPropertyHeight(property);

			if (keyProp != null || valueProp != null)
			{
				var isKeySimpleType = IsSimpleType(keyProp);
				var isValueSimpleType = IsSimpleType(valueProp);
				var isSingleLineView = this.isSingleLineView || isKeySimpleType && isValueSimpleType;


				if (isSingleLineView)
				{
					height = Mathf.Max(
						GetHeight(keyProp),
						GetHeight(valueProp)
					);
				}
				else
				{
					height = EditorGUI.GetPropertyHeight(element);
				}
			}


			if (keysFirstIndexes[index] != index)
				height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

			contentHeight += height;

			return height;
		}


		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			#if UNITY_2020_2_OR_NEWER
			if (property.isExpanded)
			{
				var listProp = property.FindPropertyRelative("list");

				if (listProp.arraySize == 0)
					contentHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

				return contentHeight
					   + EditorGUIUtility.standardVerticalSpacing * (listProp.arraySize - 1)
					   + BUTTONS_HEIGHT;
			}

			return EditorGUIUtility.singleLineHeight;

			#else
			return 0f;
			#endif
		}


		private bool IsSimpleType(SerializedProperty property)
		{
			if (property == null)
				return true;

			switch (property.propertyType)
			{
				case SerializedPropertyType.Vector4:
				case SerializedPropertyType.Quaternion:
				case SerializedPropertyType.Generic:
					return false;
				default:
					return true;
			}
		}

		#endregion
	}
}
