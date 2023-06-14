using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;


namespace I2.Loc
{
	public class FontTool : EditorWindow
	{
		private string unicodeHexString = string.Empty;
		private ReorderableList reorderableList;
		private List<int> indexes;
		private List<string> languages;
		private bool fullCharMode;


		[MenuItem("Tools/I2 Localization/Font Tool")]
		private static void Setup()
		{
			InitializeWindow();
		}


		private static void InitializeWindow()
		{
			FontTool window = CreateWindow<FontTool>();

			window.titleContent.text = nameof(FontTool);

			int width = Screen.currentResolution.width / 6;
			int height = Screen.currentResolution.height / 4;

			int x = Screen.currentResolution.width / 2 - width / 2;
			int y = Screen.currentResolution.height / 2 - height / 2;

			window.position = new Rect(x, y, width, height);
			window.Show();

		}


		private void OnEnable()
		{
			LocalizationManager.UpdateSources();
			languages = LocalizationManager.GetAllLanguages();
			indexes = new List<int>(languages.Count);

			reorderableList = new ReorderableList(indexes, typeof(int));
			reorderableList.onAddCallback = OnAddCallback;
			reorderableList.drawElementCallback = DrawElementCallback;
			reorderableList.drawHeaderCallback = DrawHeaderCallback;
		}


		private void DrawHeaderCallback(Rect rect)
		{
			EditorGUI.LabelField(rect, "Languages");
		}


		private void OnAddCallback(ReorderableList list)
		{
			indexes.Add(-1);
		}


		private void DrawElementCallback(Rect rect, int index, bool isactive, bool isfocused)
		{
			indexes[index] = EditorGUI.Popup(rect, "Language", indexes[index], languages.ToArray());
		}


		private void OnGUI()
		{
			GUILayout.BeginVertical(EditorStyles.helpBox);
			{
				GUILayout.Space(5);

				reorderableList.DoLayoutList();

				GUILayout.Space(5);

				fullCharMode = EditorGUILayout.Toggle("Full Char Mode (Lower&Upper)", fullCharMode);

				GUILayout.Space(5);

				if (GUILayout.Button("Start"))
				{
					ExtractUnicodeHex();
				}

				GUILayout.Space(15);

				if (unicodeHexString.Length > 0)
				{
					GUILayout.TextArea(unicodeHexString);
				}
			}
			GUILayout.EndVertical();
		}


		private void ExtractUnicodeHex()
		{
			HashSet<int> codes = new HashSet<int>();

			foreach (var index in indexes)
			{
				foreach (var source in LocalizationManager.Sources)
				{
					foreach (var term in source.mTerms)
					{
						string text = term.Languages[index];

						for (int i = 0; i < text.Length; i++)
						{
							if (fullCharMode)
							{
								char lowerCase = Char.ToLower(text[i]);
								char upperCase = Char.ToUpper(text[i]);

								if (!codes.Contains(lowerCase))
									codes.Add(lowerCase);

								if (!codes.Contains(upperCase))
									codes.Add(upperCase);

								continue;
							}

							if (!codes.Contains(text[i]))
								codes.Add(text[i]);
						}
					}
				}
			}

			unicodeHexString = GetHexString(codes);
		}


		private string GetHexString(HashSet<int> codes)
		{
			string result = string.Empty;

			foreach (var code in codes)
			{
				result += code.ToString("X4") + ",";
			}

			result = result.Remove(result.Length - 1, 1);

			return result;
		}
	}
}
