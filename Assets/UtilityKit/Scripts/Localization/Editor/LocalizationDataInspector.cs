using UnityEditor;
using UnityEngine;

namespace MCFramework
{
    [CustomEditor(typeof(LocalizationData))]
    [CanEditMultipleObjects]
    public class LocalizationDataInspector : Editor
    {
        SerializedProperty m_LanguageProperty;
        SerializedProperty m_ItemProperty;

        private void OnEnable()
        {
            m_LanguageProperty = serializedObject.FindProperty("language");
            m_ItemProperty = serializedObject.FindProperty("items");
        }

        public override void OnInspectorGUI()
        {
            LocalizationData data = (LocalizationData)target;
            if (data.items == null)
            {
                data.items = new LocalizationItem[1];
                data.items[0] = new LocalizationItem();

                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();
            }

            EditorGUILayout.PropertyField(m_LanguageProperty, new GUIContent("Language"));
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            for (int i = 0; i < data.items.Length; i++)
            {
                var element = m_ItemProperty.GetArrayElementAtIndex(i);
                var key = element.FindPropertyRelative("key");
                var value = element.FindPropertyRelative("value");

                EditorGUILayout.PropertyField(key);
                value.stringValue = EditorGUILayout.TextArea(value.stringValue, GUILayout.Height(55));

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Up"))
                {
                    m_ItemProperty.MoveArrayElement(i, i - 1);
                }

                if (GUILayout.Button("Down"))
                {
                    m_ItemProperty.MoveArrayElement(i, i + 1);
                }

                if (GUILayout.Button("Remove"))
                {
                    int oldSize = m_ItemProperty.arraySize;
                    m_ItemProperty.DeleteArrayElementAtIndex(i);
                    if (m_ItemProperty.arraySize == oldSize)
                    {
                        m_ItemProperty.DeleteArrayElementAtIndex(i);
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();
                EditorGUILayout.Space();
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            if (GUILayout.Button("Add Item"))
            {
                m_ItemProperty.InsertArrayElementAtIndex(data.items.Length);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
