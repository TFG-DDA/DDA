using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine.SceneManagement;

public class DDAEditorWindow : EditorWindow
{
    private static DDAConfigEditor editor;

    [MenuItem("Window/DDA Config")]
    public static void Init()
    {
        DDAEditorWindow window = (DDAEditorWindow)GetWindow(typeof(DDAEditorWindow));
        window.titleContent.text = "DDA Config";

        var DDAobjects = FindObjectsOfType<DDAConfig>();

        if (DDAobjects.Length > 1)
        {
            EditorGUILayout.LabelField("ERROR: More than one DDA Config script found in scene.");
            return;
        }
        else if (DDAobjects.Length < 1)
        {
            EditorGUILayout.LabelField("ERROR: NO DDA Config script found in scene.");
            return;
        }

        editor = Editor.CreateEditor(DDAobjects[0]) as DDAConfigEditor;
    }


    public void OnGUI()
    {
        var DDAobjects = FindObjectsOfType<DDAConfig>();
        if (DDAobjects.Length > 1)
        {
            EditorGUILayout.LabelField("ERROR: More than one DDA Config script found in scene.");
            return;
        }
        else if (DDAobjects.Length < 1)
        {
            EditorGUILayout.LabelField("ERROR: NO DDA Config script found in scene.");
            return;
        }

        if (editor != null)
            editor.Editor();
    }
}