using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

public enum TransitionTypes { TOMENU, TOCARDS, TOLEVEL }

[System.Serializable]
public class EnemyGun
{
    public WeaponScriptable weapon;
    public int probability;
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    bool paused = false;
    private bool connected = false;

    int playedLevels = 0;
    int playedRuns = 0;
    public int maxLevel = 5;
    private int lastScore = 0;

    float transitionTime = -1;
    Image transitionFade;

    float timeToFade = 1;

    Transform deathCanvas;

    int initialEnemies;
    int numEnemies;

    [SerializeField]
    GameObject stairs;

    int shotCounter = 0;
    int infiniteShotCounter = 0;

    int hitCounter = 0;
    int infiniteHitCounter = 0;

    // Tres tipos de calidad de cartas
    [SerializeField]
    Card[] cardsQ1;

    [SerializeField]
    Card[] cardsQ2;

    [SerializeField]
    Card[] cardsQ3;

    [SerializeField]
    Card[] cardsPrimera;

    [SerializeField]
    [Range(0f, 1f)]
    float slowAmount;

    [SerializeField]
    float slowDuration;

    float slowTimer;
    bool slowed = false;

    float invincibleTimer;

    // Queue for non-available levels
    private Queue recentLevels;

    // VIDA
    [SerializeField]
    public int PLAYER_LIFE = 10;
    [SerializeField]
    public int MAX_PLAYER_LIFE = 10;

    private bool playerInvincible = false;

    private bool playerIgnoreBullets = false;
    // MUNICION
    public WeaponScriptable[] weapons;
    Dictionary<WeaponScriptable, int> weaponAmmos;
    Dictionary<WeaponScriptable, int> maxAmmos;

    public EnemyGun[] enemyWeapons;

    // Variables para la carga de escena asincrona
    private AsyncOperation loadAsync;
    private bool cardApplied = false;

    // Para skins de enemigos
    [SerializeField]
    SpriteLibraryAsset[] enemySpriteLibraryAssetsList;

    // Referencia a los sonidos
    SoundResources soundResources;

    // Musica de gameplay
    FMODUnity.StudioEventEmitter musicEmitter;

    // Numero de enemigos en estado de ataque
    int numAttackingEnemies = 0;

    TransitionTypes nextTransition;

    bool checkTimeForArrow;
    [SerializeField]
    float timeToShowArrow;
    [SerializeField, Range(0f, 100f)]
    float percentToShowArrow;
    float arrowTimer;

    // Valor del cuestionario
    int form_Value;
    bool showForm = false;

    // Niveles dificiles con menos coberturas
    bool HARD_LEVELS = false;
    bool sendhard = false;

    // Vida perdida en el nivel
    public int lostHealth = 0;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // Random para elegir si la sesion es con niveles dificiles
            int hard = Random.Range(0, 2);
            if (hard > 0) HARD_LEVELS = true;
        }
        else
        {
            Destroy(gameObject);
        }

        weaponAmmos = new Dictionary<WeaponScriptable, int>();
        maxAmmos = new Dictionary<WeaponScriptable, int>();
        foreach (WeaponScriptable weapon in weapons)
        {
            weaponAmmos.Add(weapon, weapon.initialAmmo);
            maxAmmos.Add(weapon, weapon.maxAmmo);
        }
        StartCoroutine(CheckForControllers());

    }

    private void Start()
    {
        if (!sendhard)
        {
            Tracker.Instance.AddEvent(new LevelDifficultyEvent(HARD_LEVELS));
            sendhard = true;
        }

        soundResources = new SoundResources();
        recentLevels = new Queue();
        musicEmitter = GetComponent<StudioEventEmitter>();
        //DynamicGameplayMusic();
        //deathCanvas = transform.GetChild(0);
        //GetComponent<FMODUnity.StudioEventEmitter>().Play();

        //foreach(var card in cards)
        //{
        //    card.Apply();
        //}
    }

    // Update is called once per frame
    void Update()
    {
        if (transitionTime > -1 && transitionFade != null)
        {
            transitionTime += Time.deltaTime;
            float a = transitionTime / timeToFade;
            transitionFade.color = new Color(transitionFade.color.r, transitionFade.color.g, transitionFade.color.b, a);
            if (transitionTime >= timeToFade)
            {
                switch (nextTransition)
                {
                    case TransitionTypes.TOMENU:
                        GoToMenu();
                        break;
                    case TransitionTypes.TOCARDS:
                        GoToCards();
                        break;
                    case TransitionTypes.TOLEVEL:
                        GoToLevel();
                        break;
                }
            }
        }

        if (slowed)
        {
            slowTimer -= Time.deltaTime;
            if (slowTimer <= 0.0f)
            {
                Time.timeScale = 1.0f;
                slowed = false;
                playerInvincible = false;
            }
        }
        if (playerIgnoreBullets)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer <= 0.0f)
            {
                playerIgnoreBullets = false;
                PlayerInstance.instance.setBulletCollider(true);
            }
        }
        if (checkTimeForArrow)
        {
            arrowTimer += Time.deltaTime;
            if (arrowTimer >= timeToShowArrow)
            {
                PlayerInstance.instance.ToggleArrow(true);
                checkTimeForArrow = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.I)) Tracker.Instance.AddEvent(new LostHealthEvent(1));
        if (Input.GetKeyDown(KeyCode.J)) Tracker.Instance.AddEvent(new EndLevelEvent(10, "hols"));
    }

    public void changeScene(string sc)
    {

        SceneManager.LoadScene(sc);

    }


    void LevelWeaponProbs()
    {
        switch (playedLevels)
        {
            case 0:
                enemyWeapons[0].probability = 100 + DDA.instance.config.actVariables.pistolProb;
                enemyWeapons[1].probability = 0 + DDA.instance.config.actVariables.akProb;
                enemyWeapons[2].probability = 0 + DDA.instance.config.actVariables.shotgunProb;
                break;
            case 1:
                enemyWeapons[0].probability = 94 + DDA.instance.config.actVariables.pistolProb;
                enemyWeapons[1].probability = 3 + DDA.instance.config.actVariables.akProb;
                enemyWeapons[2].probability = 3 + DDA.instance.config.actVariables.shotgunProb;
                break;
            case 2:
                enemyWeapons[0].probability = 88 + DDA.instance.config.actVariables.pistolProb;
                enemyWeapons[1].probability = 6 + DDA.instance.config.actVariables.akProb;
                enemyWeapons[2].probability = 6 + DDA.instance.config.actVariables.shotgunProb;
                break;
            case 3:
                enemyWeapons[0].probability = 82 + DDA.instance.config.actVariables.pistolProb;
                enemyWeapons[1].probability = 9 + DDA.instance.config.actVariables.akProb;
                enemyWeapons[2].probability = 9 + DDA.instance.config.actVariables.shotgunProb;
                break;
            case 4:
                enemyWeapons[0].probability = 76 + DDA.instance.config.actVariables.pistolProb;
                enemyWeapons[1].probability = 12 + DDA.instance.config.actVariables.akProb;
                enemyWeapons[2].probability = 12 + DDA.instance.config.actVariables.shotgunProb;
                break;
            case 5:
                enemyWeapons[0].probability = 70 + DDA.instance.config.actVariables.pistolProb;
                enemyWeapons[1].probability = 15 + DDA.instance.config.actVariables.akProb;
                enemyWeapons[2].probability = 15 + DDA.instance.config.actVariables.shotgunProb;
                break;
            case 6:
                enemyWeapons[0].probability = 64 + DDA.instance.config.actVariables.pistolProb;
                enemyWeapons[1].probability = 18 + DDA.instance.config.actVariables.akProb;
                enemyWeapons[2].probability = 18 + DDA.instance.config.actVariables.shotgunProb;
                break;
            case 7:
                enemyWeapons[0].probability = 58 + DDA.instance.config.actVariables.pistolProb;
                enemyWeapons[1].probability = 21 + DDA.instance.config.actVariables.akProb;
                enemyWeapons[2].probability = 21 + DDA.instance.config.actVariables.shotgunProb;
                break;
            case 8:
                enemyWeapons[0].probability = 52 + DDA.instance.config.actVariables.pistolProb;
                enemyWeapons[1].probability = 24 + DDA.instance.config.actVariables.akProb;
                enemyWeapons[2].probability = 24 + DDA.instance.config.actVariables.shotgunProb;
                break;
            case 9:
                enemyWeapons[0].probability = 46 + DDA.instance.config.actVariables.pistolProb;
                enemyWeapons[1].probability = 27 + DDA.instance.config.actVariables.akProb;
                enemyWeapons[2].probability = 27 + DDA.instance.config.actVariables.shotgunProb;
                break;
            case 10:
                enemyWeapons[0].probability = 40 + DDA.instance.config.actVariables.pistolProb;
                enemyWeapons[1].probability = 30 + DDA.instance.config.actVariables.akProb;
                enemyWeapons[2].probability = 30 + DDA.instance.config.actVariables.shotgunProb;
                break;
            case 11:
                enemyWeapons[0].probability = 34 + DDA.instance.config.actVariables.pistolProb;
                enemyWeapons[1].probability = 33 + DDA.instance.config.actVariables.akProb;
                enemyWeapons[2].probability = 33 + DDA.instance.config.actVariables.shotgunProb;
                break;
            case 12:
                enemyWeapons[0].probability = 28 + DDA.instance.config.actVariables.pistolProb;
                enemyWeapons[1].probability = 36 + DDA.instance.config.actVariables.akProb;
                enemyWeapons[2].probability = 36 + DDA.instance.config.actVariables.shotgunProb;
                break;
            case 13:
                enemyWeapons[0].probability = 22 + DDA.instance.config.actVariables.pistolProb;
                enemyWeapons[1].probability = 39 + DDA.instance.config.actVariables.akProb;
                enemyWeapons[2].probability = 39 + DDA.instance.config.actVariables.shotgunProb;
                break;
            case 14:
                enemyWeapons[0].probability = 16 + DDA.instance.config.actVariables.pistolProb;
                enemyWeapons[1].probability = 42 + DDA.instance.config.actVariables.akProb;
                enemyWeapons[2].probability = 42 + DDA.instance.config.actVariables.shotgunProb;
                break;
            case 15:
                enemyWeapons[0].probability = 0 + DDA.instance.config.actVariables.pistolProb;
                enemyWeapons[1].probability = 50 + DDA.instance.config.actVariables.akProb;
                enemyWeapons[2].probability = 50 + DDA.instance.config.actVariables.shotgunProb;
                break;
            case 20:
                enemyWeapons[0].probability = 0 + DDA.instance.config.actVariables.pistolProb;
                enemyWeapons[1].probability = 0 + DDA.instance.config.actVariables.akProb;
                enemyWeapons[2].probability = 100 + DDA.instance.config.actVariables.shotgunProb;
                break;


        }
    }


    public void loadNextLevel()
    {
        // Si cargamos el primer nivel comenzamos la musica
        if (playedLevels == 0)
        {
            musicEmitter.Play(); // ACUERDATE DE PONER ESTO EN WINDOWS
        }
        numAttackingEnemies = 0;
        DynamicGameplayMusic(0);

        //enemyWeapons[0].probability = 100 - (playedLevels * (10 / 3) * 2);
        //enemyWeapons[1].probability = (playedLevels * (10 / 3));
        //enemyWeapons[2].probability = (playedLevels * (10 / 3));

        // perdon
        LevelWeaponProbs();

        playedLevels++;

        int lvl = Random.Range(0, maxLevel);

        // If more than 70% of levels have been played, dequeue
        if (recentLevels.Count > maxLevel * 0.7f)
        {
            recentLevels.Dequeue();
        }

        // Check if the level has been played recently to try a new one
        while (recentLevels.Contains(lvl))
        {
            //Debug.Log("Bucle GameMngr324");
            lvl = Random.Range(0, maxLevel);
        }

        recentLevels.Enqueue(lvl);

        if (DDA.instance.config.actVariables.hardMap)
        {
            Tracker.Instance.AddEvent(new StartLevelEvent(playedLevels, "Level" + (lvl + 1) + "_Hard"));
            StartCoroutine(LoadLevelAsync("Level" + (lvl + 1) + "_Hard"));
        }
        else
        {
            Tracker.Instance.AddEvent(new StartLevelEvent(playedLevels, "Level" + (lvl + 1)));
            StartCoroutine(LoadLevelAsync("Level" + (lvl + 1)));
        }


    }

    private IEnumerator LoadLevelAsync(string lvl)
    {
        yield return new WaitForSeconds(0.1f);
        // Carga escena de manera as�ncrona y no se muestra hasta que pase la actual

        loadAsync = SceneManager.LoadSceneAsync(lvl);
        loadAsync.allowSceneActivation = false;
        while (!loadAsync.isDone)
        {
            //Debug.Log("Bucle GameMngr343");
            if (cardApplied && loadAsync.progress >= 0.9f)
            {
                Cursor.visible = false;
                cardApplied = false;
                loadAsync.allowSceneActivation = true;

            }
            yield return null;
        }
    }

    public void StartTransition(TransitionTypes type)
    {
        nextTransition = type;
        transitionTime = 0;
        if (type != TransitionTypes.TOLEVEL)
            transitionFade = UIManager.instance.transform.GetChild(4).GetComponent<Image>();
    }

    public void SetFadeObject(Image obj)
    {
        transitionFade = obj;
    }

    public void reloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        for (int i = 0; i < deathCanvas.childCount; ++i)
            deathCanvas.GetChild(i).gameObject.SetActive(false);
        transitionTime = -1;
    }

    public void togglePause()
    {
        paused = !paused;
        if (paused)
        {
            PlayerInstance.instance.GetFireWeapon().EndAttack();
            Time.timeScale = 0;
            Tracker.Instance.AddEvent(new StartPauseEvent());
        }
        else
        {
            Time.timeScale = 1;
            Tracker.Instance.AddEvent(new EndPauseEvent());
        }
    }

    public bool IsPaused() { return paused; }

    public void SetEnemies(int nEne) { numEnemies = nEne; initialEnemies = nEne; }

    public void EnemyDie(Transform tr)
    {
        numEnemies--;
        if (!checkTimeForArrow && !PlayerInstance.instance.ShowingArrow() && (float)numEnemies / initialEnemies * 100.0f <= percentToShowArrow)
            checkTimeForArrow = true;

        if (numEnemies == 0)
            Instantiate(stairs, tr.position, Quaternion.identity);
        else if (checkTimeForArrow)
            ResetArrowTimer();
    }

    public void ResetArrowTimer()
    {
        arrowTimer = 0.0f;
    }

    public void quitGame()
    {
#if UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // Provisional
    public Card[] getCard()
    {
        Card[] cards = new Card[2];
        if (playedLevels == 2)
        {
            int randomIndex0 = Random.Range(0, cardsPrimera.Length);
            cards[0] = cardsPrimera[randomIndex0];
            randomIndex0 = Random.Range(0, cardsQ2.Length);
            while (cardsQ2[randomIndex0].id < weapons.Length) randomIndex0 = Random.Range(0, cardsQ2.Length);
            cards[1] = cardsQ2[randomIndex0];
        }
        else
        {
            int randomQuality = 0;
            float percentage = Random.Range(0f, 1f);

            // 60% Q1 / 30% Q2 / 10% Q3
            if (percentage < DDA.instance.config.actVariables.Q1Prob)
                randomQuality = 1;
            else if (percentage < DDA.instance.config.actVariables.Q2Prob)
                randomQuality = 2;
            else
                randomQuality = 3;

            int playerWeapon = System.Array.IndexOf(weapons, PlayerInstance.instance.GetWeapon());

            switch (randomQuality)
            {
                case 2:
                    int randomIndex2 = Random.Range(0, cardsQ2.Length);
                    while (cardsQ2[randomIndex2].id == playerWeapon) randomIndex2 = Random.Range(0, cardsQ2.Length);
                    cards[0] = cardsQ2[randomIndex2];
                    randomIndex2 = Random.Range(0, cardsQ2.Length);
                    // Si ya le ha tocado un arma que no le toque otra
                    if (cards[0].id < weapons.Length)
                    {
                        while (cardsQ2[randomIndex2].id < weapons.Length) randomIndex2 = Random.Range(0, cardsQ2.Length);
                    }
                    else
                    {
                        while (cardsQ2[randomIndex2] == cards[0] || cardsQ2[randomIndex2].id == playerWeapon) randomIndex2 = Random.Range(0, cardsQ2.Length);
                    }
                    cards[1] = cardsQ2[randomIndex2];
                    break;
                case 3:
                    int randomIndex3 = Random.Range(0, cardsQ3.Length);
                    while (cardsQ3[randomIndex3].id == playerWeapon) randomIndex3 = Random.Range(0, cardsQ3.Length);
                    cards[0] = cardsQ3[randomIndex3];
                    randomIndex3 = Random.Range(0, cardsQ3.Length);
                    if (cards[0].id < weapons.Length)
                    {
                        while (cardsQ3[randomIndex3].id < weapons.Length) randomIndex3 = Random.Range(0, cardsQ3.Length);
                    }
                    else
                    {
                        while (cardsQ3[randomIndex3] == cards[0] || cardsQ3[randomIndex3].id == playerWeapon) randomIndex3 = Random.Range(0, cardsQ3.Length);
                    }
                    cards[1] = cardsQ3[randomIndex3];
                    break;
                default:
                    int randomIndex = Random.Range(0, cardsQ1.Length);
                    while (cardsQ1[randomIndex].id == playerWeapon) randomIndex = Random.Range(0, cardsQ1.Length);
                    cards[0] = cardsQ1[randomIndex];
                    randomIndex = Random.Range(0, cardsQ1.Length);
                    if (cards[0].id < weapons.Length)
                    {
                        while (cardsQ1[randomIndex].id < weapons.Length) randomIndex = Random.Range(0, cardsQ1.Length);
                    }
                    else
                    {
                        while (cardsQ1[randomIndex] == cards[0] || cardsQ1[randomIndex].id == playerWeapon) randomIndex = Random.Range(0, cardsQ1.Length);
                    }
                    cards[1] = cardsQ1[randomIndex];
                    break;
            }
        }

        return cards;
    }

    public void applyCard(Card data)
    {
        data.Apply();
        UIManager.instance.reloadUI();
        //StartTransition(TransitionTypes.TOLEVEL);
        //cardApplied = true;
    }

    // Se le pasan los puntos de vida que va a modificar, si se suma = true, si resta = false
    public void setLife(int points)
    {
        PLAYER_LIFE += points;
        if (PLAYER_LIFE > MAX_PLAYER_LIFE) PLAYER_LIFE = MAX_PLAYER_LIFE;
        if (points > 0)
            UIManager.instance.setLife(PLAYER_LIFE, MAX_PLAYER_LIFE, true);
        else
            UIManager.instance.setLife(PLAYER_LIFE, MAX_PLAYER_LIFE, false);
    }
    // Seteamos la nueva vida maxima (points) , si ademas queremos a�adir vida actual, pasamos el segundo parametro
    public void setMaxLife(int points, int actual_points)
    {
        MAX_PLAYER_LIFE += points;
        //PLAYER_LIFE += actual_points;
    }

    public int getLife() { return PLAYER_LIFE; }
    public int getMaxLife() { return MAX_PLAYER_LIFE; }

    public bool isPlayerInvincible() { return playerInvincible; }

    public void updateUIAmmos()
    {
        WeaponScriptable w = PlayerInstance.instance.GetWeapon();
        UIManager.instance.setAmmo(weaponAmmos[w], maxAmmos[w], true);
    }

    public void maxAmmoWeapons()
    {
        for (int i = 0; i < weaponAmmos.Count; i++)
        {
            var w = weaponAmmos.ElementAt(i);
            weaponAmmos[w.Key] = maxAmmos[w.Key];
        }
    }

    public void setAmmo(WeaponScriptable weapon, int ammo, bool add)
    {
        int a = weaponAmmos[weapon];
        a += ammo;

        if (a > maxAmmos[weapon])
            a = maxAmmos[weapon];
        else if (a < 0)
            a = 0;

        weaponAmmos[weapon] = a;
        UIManager.instance.setAmmo(weaponAmmos[weapon], maxAmmos[weapon], add);
    }

    public int getAmmo(WeaponScriptable weapon)
    {
        return weaponAmmos[weapon];
    }

    public void setMaxAmmo(WeaponScriptable weapon, int ammo, bool add)
    {
        maxAmmos[weapon] += ammo;
        setAmmo(weapon, ammo, add);
    }

    public void setGun(int id)
    {
        UIManager.instance.setGun(weapons[id].uiTexture);
        setAmmo(weapons[id], 0, false);
    }

    public void setGun(WeaponScriptable weapon)
    {
        setGun(System.Array.IndexOf(weapons, weapon));
    }

    public void RefillWeapon(WeaponScriptable weapon)
    {
        weaponAmmos[weapon] = weapon.initialAmmo;
    }

    public void RefillWeapon(int id)
    {
        RefillWeapon(weapons[id]);
    }

    public void slowTime()
    {
        if (!slowed)
        {
            Time.timeScale -= slowAmount;
            slowed = true;
            playerInvincible = true;
        }
        slowTimer = slowDuration;
    }

    public void setIgnoreBullets(float time)
    {
        if (!playerIgnoreBullets)
        {
            PlayerInstance.instance.setBulletCollider(false);
            playerIgnoreBullets = true;
        }
        invincibleTimer = time;
    }

    public void TeleportPlayer(Transform t)
    {
        PlayerInstance.instance.transform.position = t.position;
    }

    public SpriteLibraryAsset getEnemyAsset(int i) { return enemySpriteLibraryAssetsList[i]; }
    public int getNumEnemyAssets() { return enemySpriteLibraryAssetsList.Length; }
    public SoundResources GetSoundResources() { return soundResources; }

    private void ResetPlayer()
    {
        foreach (WeaponScriptable weapon in weapons)
        {
            weaponAmmos[weapon] = weapon.initialAmmo;
            maxAmmos[weapon] = weapon.maxAmmo;
        }
        shotCounter = 0;
        infiniteShotCounter = 0;
        hitCounter = 0;
        infiniteHitCounter = 0;
        MAX_PLAYER_LIFE = 10;
        PLAYER_LIFE = 10;
    }

    void GoToMenu()
    {
        // Vuelve al menu porque ha muerto
        if (PLAYER_LIFE <= 0)
            Tracker.Instance.AddEvent(new EndLevelEvent(playedLevels, SceneManager.GetActiveScene().name));

        checkTimeForArrow = false;

        if (!connected)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        Tracker.Instance.AddEvent(new ShotEvent(PlayerInstance.instance.GetWeaponName(), shotCounter, hitCounter));
        Tracker.Instance.AddEvent(new ShotEvent("Infite", infiniteShotCounter, infiniteHitCounter));
        playedRuns++;
        ResetPlayer();
        transitionTime = -1;
        lastScore = playedLevels - 1;
        playedLevels = 0;
        cardApplied = true;
        musicEmitter.Stop();

        SceneManager.LoadScene("MenuPrincipal");
        Tracker.Instance.Flush();
    }

    void GoToCards()
    {
        PlayerInstance.instance.ToggleArrow(false);
        PlayerInstance.instance.GetFireWeapon().EndAttack();
        checkTimeForArrow = false;
        if (!connected)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        transitionTime = -1;

        Tracker.Instance.AddEvent(new ShotEvent(PlayerInstance.instance.GetWeaponName(), shotCounter, hitCounter));
        Tracker.Instance.AddEvent(new ShotEvent("Infite", infiniteShotCounter, infiniteHitCounter));
        shotCounter = 0;
        infiniteShotCounter = 0;
        hitCounter = 0;
        infiniteHitCounter = 0;

        SceneManager.LoadScene("ItemSelect");

        Tracker.Instance.Flush();

        loadNextLevel();
    }

    void GoToLevel()
    {
        transitionTime = -1;
        cardApplied = true;
    }

    public int GetPlayedLevels() { return playedLevels; }

    public void DynamicGameplayMusic(int num)
    {
        numAttackingEnemies += num;
        if (numAttackingEnemies > 0)
            GetComponent<FMODUnity.StudioEventEmitter>().EventInstance.setParameterByName("ActivateCombat", 1);
        else
            GetComponent<FMODUnity.StudioEventEmitter>().EventInstance.setParameterByName("ActivateCombat", 0);
    }

    public int getNumEnemies() { return numEnemies; }

    public int getLastScore() { return lastScore; }

    public void SetTransitionTime(float t)
    {
        transitionTime = t;
    }

    IEnumerator CheckForControllers()
    {
        while (true)
        {
            var controllers = Input.GetJoystickNames();

            //Debug.LogWarning(controllers.Length);
            //for(int i = 0; i < controllers.Length; ++i)
            //{
            //    Debug.LogWarning(controllers[i]);
            //}

            if (!connected && controllers.Length != 0)
            {
                if (controllers[0] != "")
                {
                    connected = true;
                    Debug.LogWarning("Connected");
                    if (PlayerInstance.instance != null)
                    {
                        PlayerInstance.instance.changeControls(connected);
                        UIManager.instance.changeDashInfo(connected);
                    }
                    if (SceneManager.GetActiveScene().buildIndex == 0 || SceneManager.GetActiveScene().buildIndex == 1)
                    {
                        Cursor.lockState = CursorLockMode.Locked;
                        Cursor.visible = false;
                    }
                }
            }
            else if (connected && (controllers.Length == 0 || controllers[0] == ""))
            {
                connected = false;
                Debug.LogWarning("Disconnected");
                if (PlayerInstance.instance != null)
                {
                    PlayerInstance.instance.changeControls(connected);
                    UIManager.instance.changeDashInfo(connected);
                }
                if (SceneManager.GetActiveScene().buildIndex == 0 || SceneManager.GetActiveScene().buildIndex == 1)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.None;
                }
            }

            yield return null;
        }
    }

    public bool getConnected()
    {
        return connected;
    }

    void postJSONcallback()
    {
        Debug.LogWarning("No doy error en el PostJSON");
    }
    void postJSONfallback()
    {
        Debug.LogError("Error en la llamada a PostJSON");
    }

    public void setFormValue(int f) { form_Value = f; }
    public int getFormValue() { return form_Value; }
    public bool getShowForm() { return showForm; }
    public int getPlayedRuns() { return playedRuns; }

    public void FlushTracker()
    {
        Tracker.Instance.Flush();
    }

    public void AddShot()
    {
        shotCounter++;
    }

    public void AddInfiniteShot()
    {
        infiniteShotCounter++;
    }

    public void AddHit(string name)
    {
        if (name == "PistolInfinite")
            infiniteHitCounter++;
        else
            hitCounter++;
    }
}
