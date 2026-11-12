using StateEngine.Components.UI;
using UnityEditor;
using UnityEngine;

namespace StateEngine.Editor
{
    [CustomEditor(typeof(AutoFitGridLayout)), CanEditMultipleObjects]
    public sealed class AutoFitGridLayoutEditor : UnityEditor.Editor
    {
        private SerializedProperty m_Padding;
        private SerializedProperty m_CellSize;
        private SerializedProperty m_Spacing;
        private SerializedProperty m_StartCorner;
        private SerializedProperty m_StartAxis;
        private SerializedProperty m_ChildAlignment;
        private SerializedProperty m_FitAxis;
        private SerializedProperty m_FitCount;

        private void OnEnable()
        {
            m_Padding = serializedObject.FindProperty("m_Padding");
            m_CellSize = serializedObject.FindProperty("m_CellSize");
            m_Spacing = serializedObject.FindProperty("m_Spacing");
            m_StartCorner = serializedObject.FindProperty("m_StartCorner");
            m_StartAxis = serializedObject.FindProperty("m_StartAxis");
            m_ChildAlignment = serializedObject.FindProperty("m_ChildAlignment");
            m_FitAxis = serializedObject.FindProperty("m_FitAxis");
            m_FitCount = serializedObject.FindProperty("m_FitCount");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Auto Fit", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_FitAxis);

            var fitAxis = (AutoFitGridLayout.FitAxis)m_FitAxis.enumValueIndex;
            using (new EditorGUI.DisabledScope(fitAxis == AutoFitGridLayout.FitAxis.None))
                EditorGUILayout.PropertyField(m_FitCount);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Grid", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_Padding, true);
            EditorGUILayout.PropertyField(m_Spacing, true);
            EditorGUILayout.PropertyField(m_CellSize, GetCellSizeLabel(fitAxis), true);
            EditorGUILayout.PropertyField(m_StartCorner);
            EditorGUILayout.PropertyField(m_StartAxis);
            EditorGUILayout.PropertyField(m_ChildAlignment);

            if (fitAxis != AutoFitGridLayout.FitAxis.None)
                EditorGUILayout.HelpBox("Constraint and the fitted cell dimension are driven by Fit Axis and Fit Count.", MessageType.Info);

            serializedObject.ApplyModifiedProperties();
        }

        private static GUIContent GetCellSizeLabel(AutoFitGridLayout.FitAxis fitAxis)
        {
            switch (fitAxis)
            {
                case AutoFitGridLayout.FitAxis.Width:
                    return new GUIContent("Cell Size", "Width is calculated automatically, only height is used.");
                case AutoFitGridLayout.FitAxis.Height:
                    return new GUIContent("Cell Size", "Height is calculated automatically, only width is used.");
                default:
                    return new GUIContent("Cell Size");
            }
        }
    }
}
