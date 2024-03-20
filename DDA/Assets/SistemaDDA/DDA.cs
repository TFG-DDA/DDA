using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
* DifficultyModifierTypes define las diferentes formas de modificar la dificultad en el DDA
* 
* ENEMIES: Modifica las características de los enemigos
* 
* PLAYER: Modifica las características del player
* 
* ENVIROMENT: Elige entre las escenas que van a aparecer
* 
*/
[Flags]
public enum DifficultyModifierTypes { ENEMIES = 1, PLAYER = 2, ENVIROMENT = 4 }
enum PlayerDifficulty { EASY, MID, HARD }
public class DDA
{
    private static DDA instance = null;

    DDAData config;
    PlayerDifficulty currentPlayerDifficulty;
    DifficultyModifierTypes modifierType;

    public static DDA Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new DDA();
            }
            return instance;
        }
    }

    public void Init(DDAConfig c)
    {
        config = c.data;

        if (config.EnemiesModifierType) modifierType = modifierType | DifficultyModifierTypes.ENEMIES;
        if (config.PlayerModifierType) modifierType = modifierType | DifficultyModifierTypes.PLAYER;
        if (config.EnviromentModifierType) modifierType = modifierType | DifficultyModifierTypes.ENVIROMENT;
    }
    public void Update()
    {

    }
    public void Release()
    {

    }
    // Recibe todos los eventos del Tracker
    public void Send(TrackerEvent e)
    {
        // Se lanza la atualización de dificultad cuando llega el evento de trigger dado por el diseñador
        if (e.GetEventType() == config.triggerEvent)
        {
            UpdateDifficulty();
        }
        // Se reciben el resto de eventos y se calcula la destreza del jugador
        else
        {
            currentPlayerDifficulty = PlayerDifficulty.MID;
        }
    }

    private void UpdateDifficulty()
    {
        if(modifierType.HasFlag(DifficultyModifierTypes.ENEMIES))
        {
            UpdateEnemiesDifficulty();
        }
        if (modifierType.HasFlag(DifficultyModifierTypes.PLAYER))
        {
            UpdatePlayerDifficulty();
        }
        if (modifierType.HasFlag(DifficultyModifierTypes.ENVIROMENT))
        {
            UpdateEnviromentDifficulty();
        }
    }
    private void UpdateEnemiesDifficulty()
    {
        switch (currentPlayerDifficulty)
        {
            case PlayerDifficulty.EASY:

                break;
            case PlayerDifficulty.MID:

                break;
            case PlayerDifficulty.HARD:

                break;
        }
    }
    private void UpdatePlayerDifficulty()
    {
        switch (currentPlayerDifficulty)
        {
            case PlayerDifficulty.EASY:

                break;
            case PlayerDifficulty.MID:

                break;
            case PlayerDifficulty.HARD:

                break;
        }
    }
    private void UpdateEnviromentDifficulty()
    {
        switch (currentPlayerDifficulty)
        {
            case PlayerDifficulty.EASY:

                break;
            case PlayerDifficulty.MID:

                break;
            case PlayerDifficulty.HARD:

                break;
        }
    }
}
