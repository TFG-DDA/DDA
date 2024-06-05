using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class DDAEditorWindow : EditorWindow
{
    [MenuItem("Window/DDA Config")]
    static void Init()
    {
        DDAEditorWindow window = (DDAEditorWindow)GetWindow(typeof(DDAEditorWindow));
    }

    public static void ShowExample()
    {
        DDAEditorWindow wnd = GetWindow<DDAEditorWindow>();
        wnd.titleContent = new GUIContent("DDA Editor");
    }

    public void CreateGUI()
    {
        //// Each editor window contains a root VisualElement object
        //VisualElement root = rootVisualElement;

        //// VisualElements objects can contain other VisualElement following a tree hierarchy.
        //VisualElement label = new Label("Hello World! From C#");
        //root.Add(label);

        //// Import UXML
        //var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/DDAEditorWindow.uxml");
        //VisualElement labelFromUXML = visualTree.Instantiate();
        //root.Add(labelFromUXML);

        //// A stylesheet can be added to a VisualElement.
        //// The style will be applied to the VisualElement and all of its children.
        //var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/DDAEditorWindow.uss");
        //VisualElement labelWithStyle = new Label("Hello World! With Style");
        //labelWithStyle.styleSheets.Add(styleSheet);
        //root.Add(labelWithStyle);
    }

    public void OnGUI()
    {
        var DDAobjects = FindObjectsOfType<DDAConfig>();

        if (DDAobjects.Length > 1)
        {
            EditorGUILayout.LabelField("ERROR: More than one DDA Object found in scene.");
            return;
        }
        else if (DDAobjects.Length < 1)
        {
            EditorGUILayout.LabelField("ERROR: NO DDA Object found in scene.");
            return;
        }

        var editor = Editor.CreateEditor(DDAobjects[0]) as DDAConfigEditor;
        editor.Window();
    }
}