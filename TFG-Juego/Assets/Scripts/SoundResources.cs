using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Otra castanya
public enum GUNSOUND
{
    REVOLVER,
    AK_47,
    SNIPER,
    MINIGUN,
    SHOTGUN,
    FLAMETHROWER,
    M16,
    GRENADE_LAUNCHER,
    BAZOKA
}

// Esto es una pedazo de puta mierda gigantesca ya lo se, no penseis mal ni que fuese gilipollas
public class SoundResources
{
    // Cartas
    public string CARD_IN = "event:/Cards scene/Cards in";
    public string CARD_OUT = "event:/Cards scene/Cards out";
    public string CARD_FLIP = "event:/Cards scene/Cards flip";
    public string CARD_CHOOSE = "event:/Cards scene/Choose card";

    // Disparos Armas
    public string GUN_REVOLVER = "event:/Shots/PistolShot";
    public string GUN_AK47 = "event:/Shots/AK47Shot";
    public string GUN_SNIPER = "event:/Shots/SniperShot";
    public string GUN_MINIGUN = "event:/Shots/MinigunShot";
    public string GUN_SHOTGUN = "event:/Shots/ShotgunShot";
    public string GUN_FLAMETHROWER = "event:/Shots/FlameThrowerLoop";          
    public string GUN_M16 = "event:/Shots/M16Shot";                            
    public string GUN_GRENADELAUNCHER = "event:/Shots/GrenadeLauncherShot";    
    public string GUN_BAZOKA = "event:/Shots/BazokaShot";                      //

    // Armas Enemigo
    public string GUN_ENEMY_REVOLVER = "event:/Shots/Enemy Shots/Enemy PistolShot 3D";
    public string GUN_ENEMY_AK47 = "event:/Shots/Enemy Shots/Enemy AK47Shot 3D";
    public string GUN_ENEMY_SNIPER = "event:/Shots/Enemy Shots/Enemy SniperShot 3D";
    public string GUN_ENEMY_MINIGUN = "event:/Shots/Enemy Shots/Enemy MinigunShot 3D";
    public string GUN_ENEMY_SHOTGUN = "event:/Shots/Enemy Shots/Enemy ShotgunShot 3D";

    // Impactos
    public string IMPACT_FLESH = "event:/Impacts/Bullet impact flesh";
    public string IMPACT_PLAYER = "event:/Impacts/Bullet impact player";
    public string IMPACT_ROCK = "event:/Impacts/Bullet impact rock 3D";
    public string IMPACT_ENEMY = "event:/Impacts/Bullet impact enemy 3D";
    public string GRENADE_EXPLOSION = "event:/Impacts/Grenade Explosion 3D";    
    public string IMPACT_BAZOKA = "event:/Impacts/Bazoka Explosion 3D";         //

    // Pick
    public string PICK_HP = "event:/Pickables/Pickup health";
    public string PICK_AMMO = "event:/Pickables/Pickup ammo";

    // UI
    public string UI_ACCEPT = "event:/UI/Accept";
    public string UI_CHANGE = "event:/UI/Change Selection";

    // OTHER
    public string CHANGE_WEAPON = "event:/Change weapon";
    public string VORTEX = "event:/Earthquake vortex 3D";
    public string PLAYER_DEATH_GRUNT = "event:/Impacts/Player Death grunt";
    public string PLAYER_DEATH_FALL = "event:/Impacts/Player Death Body fall";
    public string ENEMY_DEATH = "event:/Impacts/Enemy Death 3D";
    public string SWING = "event:/Swing";
    public string DASH = "event:/Footsteps/Dash";
    public string PLAYER_LEFT = "event:/Footsteps/Left Player Footstep";
    public string PLAYER_RIGHT = "event:/Footsteps/Right Player Footsetp";


    // Conversor mierdon
    public string gunSound(GUNSOUND s)
    {
        string sound = GUN_REVOLVER; // por si acaso peta yo que se
        switch (s)
        {
            case GUNSOUND.REVOLVER:
                sound = GUN_REVOLVER;
                break;
            case GUNSOUND.AK_47:
                sound = GUN_AK47;
                break;
            case GUNSOUND.SNIPER:
                sound = GUN_SNIPER;
                break;
            case GUNSOUND.MINIGUN:
                sound = GUN_MINIGUN;
                break;
            case GUNSOUND.SHOTGUN:
                sound = GUN_SHOTGUN;
                break;
            case GUNSOUND.FLAMETHROWER:
                sound = GUN_FLAMETHROWER;
                break;
            case GUNSOUND.M16:
                sound = GUN_M16;
                break;
            case GUNSOUND.GRENADE_LAUNCHER:
                sound = GUN_GRENADELAUNCHER;
                break;
            case GUNSOUND.BAZOKA:
                sound = GUN_BAZOKA;
                break;
        }
        return sound;
    }
}



