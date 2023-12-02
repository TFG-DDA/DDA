using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static System.TimeZoneInfo;
using static UnityEngine.ParticleSystem;

public class Pause : MonoBehaviour
{
    [SerializeField]
    GameObject pauseMenu;

    [SerializeField]
    GameObject optionsMenu;

    [SerializeField]
    GameObject sureMenu;

    FMOD.Studio.VCA Master;
    FMOD.Studio.VCA Music;
    FMOD.Studio.VCA FX;

    bool beenConnected = false;

    private void Start()
    {
        Master = FMODUnity.RuntimeManager.GetVCA("vca:/Master");
        Music = FMODUnity.RuntimeManager.GetVCA("vca:/Music");
        FX = FMODUnity.RuntimeManager.GetVCA("vca:/FX");

        UpdateSliders();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            GameManager.instance.togglePause();

            pauseMenu.SetActive(GameManager.instance.IsPaused());
            optionsMenu.SetActive(false);
            sureMenu.SetActive(false);

            EventSystem.current.SetSelectedGameObject(null);
            if (GameManager.instance.getConnected())
            {
                if (optionsMenu.activeInHierarchy)
                    EventSystem.current.SetSelectedGameObject(optionsMenu.transform.GetChild(3).GetChild(1).gameObject);
                else if (pauseMenu.activeInHierarchy)
                    EventSystem.current.SetSelectedGameObject(pauseMenu.transform.GetChild(1).gameObject);
            }
        }

        if (GameManager.instance.IsPaused())
        {
            if (beenConnected && !GameManager.instance.getConnected())
            {
                EventSystem.current.SetSelectedGameObject(null);
                beenConnected = false;
            }
            else if (!beenConnected && GameManager.instance.getConnected())
            {
                SetSelected();
                beenConnected = true;
            }
            if (GameManager.instance.getConnected() && EventSystem.current.currentSelectedGameObject == null) SetSelected();
        }
    }

    public void Continue()
    {
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);
        if (GameManager.instance.getConnected())
            EventSystem.current.SetSelectedGameObject(null);
        GameManager.instance.togglePause();
    }

    public void Options()
    {
        optionsMenu.SetActive(true);
        pauseMenu.SetActive(false);
        if (GameManager.instance.getConnected())
            EventSystem.current.SetSelectedGameObject(optionsMenu.transform.GetChild(3).GetChild(1).gameObject);
    }

    public void Quit()
    {
        pauseMenu.SetActive(false);
        sureMenu.SetActive(true);
        if (GameManager.instance.getConnected())
            EventSystem.current.SetSelectedGameObject(sureMenu.transform.GetChild(2).gameObject);
    }

    public void QuitYes()
    {
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);
        sureMenu.SetActive(false);
        GameManager.instance.togglePause();
        GameManager.instance.StartTransition(TransitionTypes.TOMENU);
    }

    public void QuitNo()
    {
        pauseMenu.SetActive(true);
        sureMenu.SetActive(false);
        if (GameManager.instance.getConnected())
            EventSystem.current.SetSelectedGameObject(pauseMenu.transform.GetChild(3).gameObject);
    }

    public void CloseOptions()
    {
        optionsMenu.SetActive(false);
        pauseMenu.SetActive(true);
        if (GameManager.instance.getConnected())
            EventSystem.current.SetSelectedGameObject(pauseMenu.transform.GetChild(2).gameObject);
    }

    void SetSelected()
    {
        if(pauseMenu.activeSelf)
            EventSystem.current.SetSelectedGameObject(pauseMenu.transform.GetChild(1).gameObject);
        else if(optionsMenu.activeSelf)
            EventSystem.current.SetSelectedGameObject(optionsMenu.transform.GetChild(3).GetChild(1).gameObject);
        else if(sureMenu.activeSelf)
            EventSystem.current.SetSelectedGameObject(sureMenu.transform.GetChild(2).gameObject);
    }

    public void ChangeMaster(float val)
    {
        Master.setVolume(val);
        RuntimeManager.PlayOneShot(GameManager.instance.GetSoundResources().UI_ACCEPT);
    }

    public void ChangeMusic(float val)
    {
        Music.setVolume(val);
    }

    public void ChangeFX(float val)
    {
        FX.setVolume(val);
        RuntimeManager.PlayOneShot(GameManager.instance.GetSoundResources().UI_ACCEPT);
    }

    public void ToggleFullscreen(bool val)
    {

    }

    public void ChangeResolution()
    {

    }

    void UpdateSliders()
    {
        float sliderVal;
        Master.getVolume(out sliderVal);
        optionsMenu.transform.GetChild(3).GetChild(1).GetComponent<Slider>().value = sliderVal;
        
        Music.getVolume(out sliderVal);
        optionsMenu.transform.GetChild(4).GetChild(1).GetComponent<Slider>().value = sliderVal;
        
        FX.getVolume(out sliderVal);
        optionsMenu.transform.GetChild(5).GetChild(1).GetComponent<Slider>().value = sliderVal;
    }
}
