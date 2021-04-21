using UnityEngine;
using UnityEditor;

public static class SerializedPropertyMazeExtension {

}

[CustomEditor(typeof(MazeView))]
public class MazeViewEditor : Editor {
    SerializedProperty floorPrefabProperty;
    SerializedProperty wallPrefabProperty;

    int width;
    int height;
    int wallCount;

    void OnEnable() {
        floorPrefabProperty = serializedObject.FindProperty("floorPrefab");
        wallPrefabProperty = serializedObject.FindProperty("wallPrefab");

        MazeView view = target as MazeView;
        width = view.maze?.width ?? 10;
        height = view.maze?.height ?? 10;
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        GUILayout.Space(20);

        serializedObject.Update();

        MazeView view = target as MazeView;

        EditorGUI.BeginChangeCheck();
        width = EditorGUILayout.IntSlider("Width", width, 0, 20);
        height = EditorGUILayout.IntSlider("Height", height, 0, 20);
        wallCount = EditorGUILayout.IntSlider("Number of walls", wallCount, 0, (width - 1)*(height - 1));

        if (GUILayout.Button("Generate Maze") || EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(target, $"Generate Maze");

            MazeBuilder builder = new MazeBuilder();
            builder.mazeWidth = width;
            builder.mazeHeight = height;
            builder.wallCount = wallCount;
            view.maze = builder.Build();
        }

        if (GUILayout.Button("Clear Maze")) {
            Undo.RecordObject(target, $"Clear Maze");

            view.maze = null;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
