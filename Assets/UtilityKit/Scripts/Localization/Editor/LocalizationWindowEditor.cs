using UnityEditor;
using UnityEngine;

namespace UtilityKit
{
    public class LocalizationWindowEditor : EditorWindow
    {
        private LocalizationData m_LocalizationData;
        private SerializedObject serializedObject;
        private SerializedProperty localizationList;
        private SerializedProperty itemProperty;

        private bool showDetails = false;
        private int selectedItemIndex = -1;
        private Vector2 scrollingPosition = Vector2.zero;

        [MenuItem("Window/Localization/Open Database")]
        public static void Init()
        {
            LocalizationWindowEditor window = (LocalizationWindowEditor)GetWindow(typeof(LocalizationWindowEditor));
            window.minSize = new Vector2(800, 600);
            window.titleContent = new GUIContent("Localization Database Editor");
            window.Show();
        }

        private void OnGUI()
        {
            m_LocalizationData = (LocalizationData)EditorGUILayout.ObjectField("Database:", m_LocalizationData, typeof(LocalizationData), false);
            if (m_LocalizationData == null)
                return;

            EditorGUILayout.Space();

            // Update settings
            serializedObject = new SerializedObject(m_LocalizationData);
            localizationList = serializedObject.FindProperty("items");
            serializedObject.Update();
            localizationList.serializedObject.Update();

            // Show editor
            EditorGUILayout.PropertyField(serializedObject.FindProperty("language"));
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            ListView();
            DetailView();
            EditorGUILayout.EndHorizontal();
            
            // Save new setting
            serializedObject.ApplyModifiedProperties();
            localizationList.serializedObject.ApplyModifiedProperties();
        }

        private void ListView()
        {
            scrollingPosition = GUILayout.BeginScrollView(scrollingPosition, GUILayout.Width(250), GUILayout.ExpandHeight(true));
            for (int i = 0; i < m_LocalizationData.items.Length; i++)
            {
                GUI.color = (selectedItemIndex == i) ? Color.grey : GUI.color = Color.white; ;
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(m_LocalizationData.items[i].key))
                {
                    itemProperty = localizationList.GetArrayElementAtIndex(i);
                    selectedItemIndex = i;
                    showDetails = true;
                    GUI.FocusControl(null);
                }
                GUI.color = Color.white;
                GUILayout.EndHorizontal();
                GUILayout.Space(2);
            }
            GUILayout.EndScrollView();
        }

        private void DetailView()
        {
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            if (itemProperty != null && showDetails)
            {
                itemProperty.serializedObject.Update();
                EditorGUILayout.PropertyField(itemProperty.FindPropertyRelative("key"));
                SerializedProperty value = itemProperty.FindPropertyRelative("value");
                value.stringValue = EditorGUILayout.TextArea(value.stringValue, GUILayout.MaxHeight(250));
                itemProperty.serializedObject.ApplyModifiedProperties();
            }
            GUILayout.EndVertical();
        }
    }
}
