using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

#if UNITY_EDITOR || UNITY_EDITOR_64 || UNITY_EDITOR_WIN
public class EditWindow : EditorWindow
{
    [MenuItem("Custom/DataParser")]
    private static void Init()
    {
        EditorWindow wnd = GetWindow<EditWindow>();
        wnd.titleContent = new GUIContent("DataParser");
    }
    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("[.csv] �� [.bin]"))
        {
            DataTable.LoadCSVTable();         //CSV ���� �о
            DataTable.WriteBinaryFiles();     //Binary ���Ϸ� ����
        }
        EditorGUILayout.EndHorizontal();
    }
}
#endif
