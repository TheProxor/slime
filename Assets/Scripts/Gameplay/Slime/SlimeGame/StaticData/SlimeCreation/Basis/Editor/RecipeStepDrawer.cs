using System;
using UnityEditor;
using UnityEngine;

namespace TheProxor.MetaGamesSystem.MetaGames.SlimeGame.StaticData.SlimeCreation.Basis.Editor
{
	[CustomPropertyDrawer(typeof(RecipeStep))]
	public class RecipeStepDrawer : PropertyDrawer
	{
		private const string NAME_PATH = "name";
		private const string STATE_TYPE_PATH = "stateType";
		private const string EFFECTS_PATH = "effects";
		private const string CHANGE_BOWL_FILL_PATH = "changeBowlFill";
		private const string BOWL_FILL_ANIMATION_PATH = "bowlFillAnimation";
		private const string ANIMATE_MATERIAL_PATH = "animateMaterial";
		private const string MATERIAL_ANIMATION_PATH = "materialAnimation";
		private const string STEP_SOUND_PATH = "stepSound";

		private static readonly float standardVerticalSpacing =
			EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if (!property.isExpanded)
			{
				return base.GetPropertyHeight(property, label);
			}

			float effectsPropHeight =
				EditorGUI.GetPropertyHeight(property.FindPropertyRelative(EFFECTS_PATH));

			float height = EditorGUIUtility.singleLineHeight * 4
						   + EditorGUIUtility.standardVerticalSpacing * 6
						   + effectsPropHeight;

			if (property.FindPropertyRelative(CHANGE_BOWL_FILL_PATH).boolValue)
			{
				SerializedProperty bowlFillAnimationProp =
					property.FindPropertyRelative(BOWL_FILL_ANIMATION_PATH);
				height += EditorGUI.GetPropertyHeight(bowlFillAnimationProp);
			}

			if (property.FindPropertyRelative(ANIMATE_MATERIAL_PATH).boolValue)
			{
				SerializedProperty materialAnimationProp =
					property.FindPropertyRelative(MATERIAL_ANIMATION_PATH);
				height += EditorGUI.GetPropertyHeight(materialAnimationProp);
			}


			height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative(STEP_SOUND_PATH));

			return height;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			position.height = EditorGUIUtility.singleLineHeight;
			property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label, true);

			if (property.isExpanded)
			{
				using (new EditorGUI.IndentLevelScope())
				{
					Space(ref position);
					DrawProperty(ref position, property);
				}
			}

			EditorGUI.EndProperty();
		}

		private void DrawProperty(ref Rect position,
								  SerializedProperty property)
		{
			DrawStateType(position, property);
			Space(ref position);
			DrawChangeBowlFill(ref position, property);
			DrawAnimateMaterial(ref position, property);

			EditorGUI.PropertyField(position, property.FindPropertyRelative(STEP_SOUND_PATH));
			Space(ref position);
			EditorGUI.PropertyField(position, property.FindPropertyRelative(EFFECTS_PATH));
		}

		private static void DrawStateType(Rect position, SerializedProperty property)
		{
			SerializedProperty stateTypeProp = property.FindPropertyRelative(STATE_TYPE_PATH);

			EditorGUI.BeginChangeCheck();
			{
				EditorGUI.PropertyField(position, stateTypeProp);
			}

			if (EditorGUI.EndChangeCheck())
			{
				property.FindPropertyRelative(NAME_PATH).stringValue =
					Enum.GetNames(typeof(SubStateType))[stateTypeProp.enumValueIndex];
			}
		}

		private void DrawChangeBowlFill(ref Rect position, SerializedProperty property)
		{
			SerializedProperty changeBowlFillProp =
				property.FindPropertyRelative(CHANGE_BOWL_FILL_PATH);

			EditorGUI.PropertyField(position, changeBowlFillProp);

			Space(ref position);

			if (!changeBowlFillProp.boolValue)
			{
				return;
			}

			using (new EditorGUI.IndentLevelScope())
			{
				SerializedProperty animationProp =
					property.FindPropertyRelative(BOWL_FILL_ANIMATION_PATH);
				EditorGUI.PropertyField(position, animationProp, true);
				position.y += EditorGUI.GetPropertyHeight(animationProp)
							  + EditorGUIUtility.standardVerticalSpacing;
			}
		}

		private void DrawAnimateMaterial(ref Rect position, SerializedProperty property)
		{
			SerializedProperty animateMaterialProp =
				property.FindPropertyRelative(ANIMATE_MATERIAL_PATH);

			EditorGUI.PropertyField(position, animateMaterialProp);

			Space(ref position);

			if (!animateMaterialProp.boolValue)
			{
				return;
			}

			using (new EditorGUI.IndentLevelScope())
			{
				SerializedProperty animationProp =
					property.FindPropertyRelative(MATERIAL_ANIMATION_PATH);
				EditorGUI.PropertyField(position, animationProp, true);
				position.y += EditorGUI.GetPropertyHeight(animationProp)
							  + EditorGUIUtility.standardVerticalSpacing;
			}
		}

		private static void Space(ref Rect position)
		{
			position.y += standardVerticalSpacing;
		}
	}
}
