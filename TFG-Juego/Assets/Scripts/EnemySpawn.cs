using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.Tilemaps;
using UnityEngine.U2D.Animation;
using static UnityEngine.GraphicsBuffer;

[Serializable]
public struct IslandConfig
{
    [Range(0f, 100f)]
    public int percentaje;
    public Color color;
    public Tilemap tilemap;
}

public class EnemySpawn : MonoBehaviour
{

    public IslandConfig[] islandConfig;
    int[] num_enemies;

    [SerializeField]
    public int enemies;

    [SerializeField]
    public GameObject enemyObject;
    [SerializeField]
    public GameObject enemyContainer;

    int num_enemy_types;

    // Start is called before the first frame update
    void Start()
    {
        num_enemy_types = GameManager.instance.getNumEnemyAssets();
        int totalSpawned = 0;
        // Asignamos el numero de enemigos que va a cada isla
        num_enemies = new int[islandConfig.Length];
        for (int i = 0; i< islandConfig.Length; i++)
        {
            // La operacion de porcentaje con enteros se va a redondear hacia abajo y va a dejar enemigos sin aparecer
            num_enemies[i] = (islandConfig[i].percentaje * enemies) / 100;

            totalSpawned += num_enemies[i];
        }

        // Colocamos el resto de enemigos que no se han instanciado por el paso de flotante a entero
        for (int i = totalSpawned; i < enemies; ++i)
        {
            int island = UnityEngine.Random.Range(0, islandConfig.Length);
            num_enemies[island]++;
        }

        for (int i = 0; i < islandConfig.Length; i++)
        {
            // Colocamos los enemigos
            setEnemies(i);
        }

        GameManager.instance.SetEnemies(enemies);
    }

    void setEnemies(int i)
    {
        // Sacamos las dimensiones de la isla
        BoundsInt bounds = islandConfig[i].tilemap.cellBounds;

        // Lista de posiciones activas
        List<Vector3Int> positions = new List<Vector3Int>();

        // Recorremos las coordenadas de la isla
        int a = 0;
        for (int x =  bounds.xMin; x < bounds.xMax; x++)
        {
            for(int y = bounds.yMin; y < bounds.yMax; y++)
            {
                // Sacamos su posicion
                Vector3Int pos = new Vector3Int(x, y, (int)islandConfig[i].tilemap.transform.position.z);
                TileBase tile = islandConfig[i].tilemap.GetTile(pos);

                // Si el tile esta pintado, guardamos su posicion
                if (tile != null)
                {
                    positions.Add(pos);
                }

            }
        }

        for (int e = 0; e < num_enemies[i]; e++)
        {
            // Lanzamos un random para elegir la casilla 
            int index = UnityEngine.Random.Range(0, positions.Count - 1);
            //Debug.Log("INDICE" + index);
            //Debug.Log("POS" + positions[index]);

            Vector3 pos = islandConfig[i].tilemap.CellToWorld(positions[index]);
            pos.x -= 0.002f;
            pos.y += 0.07f;

            // Le asignamos una skin 
            int skin_index = UnityEngine.Random.Range(0, num_enemy_types);
            enemyObject.GetComponent<SpriteLibrary>().spriteLibraryAsset = GameManager.instance.getEnemyAsset(skin_index);

            // Instanciamos el enemigo
            Instantiate(enemyObject, pos, Quaternion.identity, enemyContainer.transform);
        }
    
    }
}



#if UNITY_EDITOR
[CustomEditor(typeof(EnemySpawn))]
public class EnemySpawnEditor : Editor
{
    SerializedProperty spawn_Property;

    void OnEnable()
    {
        spawn_Property = serializedObject.FindProperty("islandConfig");
    }

    void OnValidate()
    {

    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(spawn_Property);

        EnemySpawn enemySpawn = (EnemySpawn)target;
        //Metodo de checkeo de cambios en el editor
        EditorGUI.BeginChangeCheck();


        // Cambia el color del Sprite directamente desde el Editor
        for(int i = 0; i < enemySpawn.islandConfig.Length; i++)
        {
            enemySpawn.islandConfig[i].color.a = 1;
            enemySpawn.islandConfig[i].tilemap.color = enemySpawn.islandConfig[i].color;
            enemySpawn.islandConfig[i].tilemap.RefreshAllTiles();
        }

        // Resto de elementos
        enemySpawn.enemies = EditorGUILayout.IntField("Num Enemies", enemySpawn.enemies);
        enemySpawn.enemyObject = EditorGUILayout.ObjectField("Enemy Object", enemySpawn.enemyObject, typeof(GameObject), false) as GameObject;
        enemySpawn.enemyContainer = EditorGUILayout.ObjectField("Container Object", enemySpawn.enemyContainer, typeof(GameObject), true) as GameObject;
        


        //Si ha habido cambios utilizamos setDirty para que unity no cambie los valores de editor y se mantengan para ejecucion
        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(target);

        // Guarda los cambios realizados en el editor
        serializedObject.ApplyModifiedProperties();

    }
}
#endif

