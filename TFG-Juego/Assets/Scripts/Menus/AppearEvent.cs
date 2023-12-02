using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum menuType {INTRO, OPTIONS, CREDITS};
public class AppearEvent : MonoBehaviour
{
    [SerializeField]
    MenuManager menuManager;
    //[SerializeField]
    //bool isIntro = true;

    [SerializeField]
    menuType type;

    public void appear(menuType t)
    {
        //if (isIntro)
        //    menuManager.displayOptions();
        //else
        //    menuManager.displayIntro();

        switch(type)
        {
            case menuType.INTRO:
                if (menuManager.getToOptions())
                    menuManager.displayOptions();
                else
                    menuManager.displayCredits();
                break;
            case menuType.OPTIONS:
                menuManager.displayIntro();
                break;
            case menuType.CREDITS:
                menuManager.displayIntro();
                break;
        }
    }
}
