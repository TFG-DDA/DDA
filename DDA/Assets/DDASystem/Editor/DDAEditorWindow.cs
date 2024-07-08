using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine.SceneManagement;

[System.Serializable]
public class DDAEditorWindow : EditorWindow
{
    private static DDAConfigEditor editor;
    Vector2 scrollPosition = Vector2.zero;
    private static DDAEditorWindow window;

    [MenuItem("Window/DDA Config")]
    public static void Init()
    {
        // Cogemos la ventana y le cambiamos el titulo
        window = (DDAEditorWindow)GetWindow(typeof(DDAEditorWindow));
        window.titleContent.text = "DDA Config";

        // Encontramos los objetos en la escena que tengan el componente DDAConfig
        var DDAobjects = FindObjectsOfType<DDAConfig>();

        // Si hay mas de uno se avisa y no se muestra nada mas
        if (DDAobjects.Length > 1)
        {
            EditorGUILayout.LabelField("ERROR: More than one DDA Config script found in scene.");
            return;
        }
        // Si no hay ninguno tambien
        else if (DDAobjects.Length < 1)
        {
            EditorGUILayout.LabelField("ERROR: NO DDA Config script found in scene.");
            return;
        }

        // Si hay exactamente un objeto con DDAConfig se crea el editor custom
        editor = Editor.CreateEditor(DDAobjects[0]) as DDAConfigEditor;
    }

    // Mientras se muestre la ventana
    public void OnGUI()
    {
        // Se sigue comprobando en todo momento que haya exactamente un objeto DDAConfig
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

        // Se crea scroll por si hace falta que se use para poder ver todos los elementos
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);
        // Se muestra el editor
        if (editor != null)
            editor.Editor(window);

        GUILayout.EndScrollView();
    }
}