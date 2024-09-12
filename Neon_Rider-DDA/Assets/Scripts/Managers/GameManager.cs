using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Creas un object GameManager vacio (prefab para que sobreviva escenas) con este script.

    public static GameManager instance;
    public Vector2 checkpoint;
    public Tuple<Vector2, SpawnEnemy.Type>[] posSpawn;
    public int deadVal = -1;
    public int actualScene = 1;
    public bool fullScreenToggle = true,
                mando = true,
                gameIsPaused; 
                //toggleDeath = true;
    public float mainVolSlider = 0.2f,
                 SFXVolSlider = 0.2f,
                 musicVolSlider = 0.2f;

    public int deads = 0;

    [SerializeField] GameObject[] prefabsEnemies;

    // En el método Awake comprueba si hay otro GameManger
    // y si no lo hay se inicializa como GameManager. En el caso
    // que hubiera otro se autodestruye
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }      
    }

    public void FullscreenToggleState(bool isFullscreen)
    {
        if (isFullscreen)
            fullScreenToggle = true;
        else
            fullScreenToggle = false;
    }
    public void ControlToggle(bool isMando)
    {
        mando = isMando;
    }
    /*public void DeathToggle(bool isDeath)
    {
        toggleDeath = !isDeath;
    }*/
    public void MainSliderState (float volume)
    {
        mainVolSlider = volume;
    }
    public void MusicSliderState(float volume)
    {
        musicVolSlider = volume;
    }
    public void SFXSliderState(float volume)
    {
        SFXVolSlider = volume;
    }

    public void ChangeScene()
    {
        posSpawn = null;
        checkpoint = new Vector2(0, 0);
        deadVal = -1;
        Time.timeScale = 1;
        //OnSceneChange(SceneManager.GetActiveScene().buildIndex);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void OnLevelWasLoaded(int level)
    {
        if (level != 0 && level != SceneManager.sceneCountInBuildSettings - 1)
            actualScene = level;
        switch (level)
        {
            case (0):
                AudioManager.instance.StopAll();
                AudioManager.instance.Play(AudioManager.ESounds.Menu);
                break;
            case (1):
                AudioManager.instance.StopAll();
                AudioManager.instance.Play(AudioManager.ESounds.Level1Low);
                break;
            case (3):
                AudioManager.instance.Stop(AudioManager.ESounds.Level1Low);
                AudioManager.instance.Stop(AudioManager.ESounds.Menu);
                AudioManager.instance.Play(AudioManager.ESounds.Level1);
                break;
            case (5):
                AudioManager.instance.Stop(AudioManager.ESounds.Level1);
                AudioManager.instance.Stop(AudioManager.ESounds.Menu);
                AudioManager.instance.Play(AudioManager.ESounds.Level2);
                break;
            case (6):
            case (8):
                AudioManager.instance.Stop(AudioManager.ESounds.Level2);
                AudioManager.instance.Stop(AudioManager.ESounds.Menu);
                AudioManager.instance.Play(AudioManager.ESounds.Level2Low);
                break;
            case (7):
                AudioManager.instance.Stop(AudioManager.ESounds.Level2Low);
                AudioManager.instance.Stop(AudioManager.ESounds.Menu);
                AudioManager.instance.Play(AudioManager.ESounds.Level2);
                break;
            case (9):
                AudioManager.instance.Stop(AudioManager.ESounds.Level2Low);
                AudioManager.instance.Stop(AudioManager.ESounds.Menu);
                break;
            case (10):
                AudioManager.instance.Stop(AudioManager.ESounds.Boss);
                AudioManager.instance.Stop(AudioManager.ESounds.Menu);
                AudioManager.instance.Play(AudioManager.ESounds.Credits);
                break;
        }
    }

    public void SpawnEnemies(Tuple<Vector2, SpawnEnemy.Type>[] pos, Transform player, int deadVal)
    {
        foreach (Tuple<Vector2, SpawnEnemy.Type> i in pos)
        {
            switch(i.Item2)
            {
                case SpawnEnemy.Type.MATON:
                    GameObject enemy = Instantiate(prefabsEnemies[0], i.Item1, new Quaternion());
                    enemy.GetComponent<EnemyAttack>().player = player;
                    enemy.GetComponent<EnemyMovement>().player = player;
                    enemy.GetComponent<Enemy_Death>().deadVal = deadVal + 1;
                    break;
            }
        }
    }
}
