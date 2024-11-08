#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditor.UI;

    namespace LucidFactory.UI.Editor
    {
        [CustomEditor(typeof(LF_CurvedLayout), true)]
        [CanEditMultipleObjects]
        public class CurvedLayoutEditor : HorizontalOrVerticalLayoutGroupEditor
        {
            private SerializedProperty m_CurveStrength;
            private SerializedProperty m_maxInfluenceAt;
            private SerializedProperty m_MaxTilt;

            protected override void OnEnable()
            {
                base.OnEnable();
                m_CurveStrength = serializedObject.FindProperty("curveStrength");
                m_MaxTilt = serializedObject.FindProperty("maxTilt");
                m_maxInfluenceAt = serializedObject.FindProperty("maxInfluenceAt");
            }

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                serializedObject.Update();
                EditorGUILayout.PrefixLabel("Curve settings");
                EditorGUILayout.PropertyField(m_CurveStrength, true);
                EditorGUILayout.PropertyField(m_maxInfluenceAt, true);
                EditorGUILayout.PropertyField(m_MaxTilt, true);
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
#endif
