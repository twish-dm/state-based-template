using UnityEditor;
using UnityEditor.UI;

using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(AutoFitGridLayout))]
[CanEditMultipleObjects]
public class AutoFitGridLayoutEditor : Editor
{
    private SerializedProperty padding;
    private SerializedProperty cellSize;
    private SerializedProperty spacing;
    private SerializedProperty startCorner;
    private SerializedProperty startAxis;
    private SerializedProperty childAlignment;

    private SerializedProperty fitAxis;
    private SerializedProperty fitCount;

    protected void OnEnable()
    {
        // Standard GridLayout properties
        padding = serializedObject.FindProperty("m_Padding");
        cellSize = serializedObject.FindProperty("m_CellSize");
        spacing = serializedObject.FindProperty("m_Spacing");
        startCorner = serializedObject.FindProperty("m_StartCorner");
        startAxis = serializedObject.FindProperty("m_StartAxis");
        childAlignment = serializedObject.FindProperty("m_ChildAlignment");

        // Custom properties
        fitAxis = serializedObject.FindProperty("fitAxis");
        fitCount = serializedObject.FindProperty("fitCount");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Custom settings
        EditorGUILayout.LabelField("Auto Fit Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(fitAxis);
        EditorGUILayout.PropertyField(fitCount);

        EditorGUILayout.Space(10);

        // Standard settings
        EditorGUILayout.LabelField("Grid Layout Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(padding, true);
        EditorGUILayout.PropertyField(spacing, true);

        EditorGUILayout.Space(5);

        // Conditional cell size display
        var fitAxisValue = (AutoFitGridLayout.FitAxis)fitAxis.enumValueIndex;
        if (fitAxisValue == AutoFitGridLayout.FitAxis.Width)
        {
            EditorGUILayout.PropertyField(cellSize, new GUIContent("Fixed Cell Height"), true);
        }
        else if (fitAxisValue == AutoFitGridLayout.FitAxis.Height)
        {
            EditorGUILayout.PropertyField(cellSize, new GUIContent("Fixed Cell Width"), true);
        }
        else
        {
            EditorGUILayout.PropertyField(cellSize, true);
        }

        EditorGUILayout.Space(5);

        EditorGUILayout.PropertyField(startCorner);
        EditorGUILayout.PropertyField(startAxis);
        EditorGUILayout.PropertyField(childAlignment);

        // Warning about constraints
        EditorGUILayout.HelpBox(
            "Constraint settings are automatically controlled by the Fit Axis and Fit Count values.",
            MessageType.Info);

        serializedObject.ApplyModifiedProperties();
    }
}