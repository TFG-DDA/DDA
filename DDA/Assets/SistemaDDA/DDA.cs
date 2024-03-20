using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/* TODO: INSTRUMENTALIZACIÓN DEL CÓDIGO
 * 
 * Para instrumentalizar el código se deberán cambiar las variables que quiera modificar el jugador,
 * sustituyéndolas por la información que dará el DDA, ejemplo:
 * 
 * (Antes)
 *      enemyHealth = salasPasadas * 10;
 * 
 * (Despúes)
 *      if(DDA.Instance.modifierType.HasFlag(DifficultyModifierTypes.ENEMIES))
 *          enemyHealth = DDA.Instance.variableModificableDDA;
 *      else
 *          enemyHealth = salasPasadas * 10;
 *          
 * !!! ESTO ES UNA IDEA !!!
 *          
 */



/*
* DifficultyModifierTypes define las diferentes formas de modificar la dificultad en el DDA,
* se pueden utilizar varias a la vez
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

    // Variable usada para implementar el DDA en el código del juego
    public DifficultyModifierTypes modifierType;

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

        // modifierType se utiliza como FLAGS
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
        // Se actualizan tantas variables como flags estén activas en el modifierType
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
