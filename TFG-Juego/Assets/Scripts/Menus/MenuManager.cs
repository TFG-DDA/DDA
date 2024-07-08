using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    GameObject introMenu;
    [SerializeField]
    GameObject optionMenu;
    [SerializeField]
    GameObject creditsMenu;
    [SerializeField]
    GameObject formMenu;

    [SerializeField]
    Animator formInfoAnim;
    Animator formAnim;

    bool showForm = false;

    //[SerializeField]
    TextMeshProUGUI resolutionText;

    Slider music;
    Slider vfx;
    Slider master;

    Resolution[] aux_resolution;
    List<Resolution> resolution_list;
    int actual_resolution;

    bool toOptions;

    [SerializeField]
    GameObject fadeObject;

    FMOD.Studio.VCA Master;
    FMOD.Studio.VCA Music;
    FMOD.Studio.VCA FX;

    // Start is called before the first frame update
    void Start()
    {
#if !UNITY_WEBGL
        Screen.SetResolution(1920, 1080, false);
        setFullScreen(true);
#endif
        //checkFullScreen();
        //checkResolutions();
        checkVolumes();
        if(PlayerInstance.instance != null)
            PlayerInstance.instance.DestroyThyself();
        if(UIManager.instance != null)
            UIManager.instance.DestroyThyself();

        // Formulario, solo lo mostramos si viene de morir
        formAnim = formMenu.GetComponent<Animator>();
        formMenu.SetActive(GameManager.instance.getShowForm());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void checkFullScreen()
    {
        Toggle toggle = optionMenu.transform.Find("Toggle").GetComponent<Toggle>();
        if(Screen.fullScreen)
            toggle.isOn = true;
        else toggle.isOn = false;
    }

    void checkResolutions()
    {
        // Guardamos todas las resoluciones disponibles de nuestro monitor
        aux_resolution = Screen.resolutions;
        actual_resolution = 0;

        // Cogemos la referencia del texto de resolucion
        resolutionText = optionMenu.transform.Find("Resolution").GetComponentInChildren<TextMeshProUGUI>();


        // Miramos la resolucion actual de la pantalla y la asignamos
        int i = 0;
        bool found_res = false;
        Debug.Log(aux_resolution.Length);
        while(Screen.fullScreen && i < aux_resolution.Length && !found_res)
        {
            if (aux_resolution[i].width == Screen.currentResolution.width
                && aux_resolution[i].height == Screen.currentResolution.height)
            {
                actual_resolution = i;
                found_res = true;
                //setResolutionText();
            }
            i++;
        }

        // Nos guardamos solo las resoluciones que nos interesan
        resolution_list = new List<Resolution>();
        for(int j = 0; j < aux_resolution.Length; j++)
        {
            if(Screen.currentResolution.refreshRate == aux_resolution[j].refreshRate)
                resolution_list.Add(aux_resolution[j]);
        }

        for(int j = 0; j< aux_resolution.Length; j++)
        {
            //Debug.Log(aux_resolution[j].width + " " + aux_resolution[j].height);
        }
    }

    void checkVolumes()
    {
        Master = FMODUnity.RuntimeManager.GetVCA("vca:/Master");
        Music = FMODUnity.RuntimeManager.GetVCA("vca:/Music");
        FX = FMODUnity.RuntimeManager.GetVCA("vca:/FX");

        master = optionMenu.transform.Find("MasterOption").GetComponentInChildren<Slider>();
        music = optionMenu.transform.Find("MusicOption").GetComponentInChildren<Slider>();
        vfx = optionMenu.transform.Find("EffectsOption").GetComponentInChildren<Slider>();

        float sliderVal;
        Master.getVolume(out sliderVal);
        master.value = sliderVal;

        Music.getVolume(out sliderVal);
        music.value = sliderVal;

        FX.getVolume(out sliderVal);
        vfx.value = sliderVal;
    }

    public bool getToOptions() { return toOptions; }
    public void displayOptions()
    {
        introMenu.SetActive(false);
        optionMenu.SetActive(true);
        optionMenu.GetComponent<Animator>().SetBool("hide", false);
        if (GameManager.instance.getConnected())
            EventSystem.current.SetSelectedGameObject(optionMenu.transform.GetChild(2).GetChild(1).gameObject);
    }
    public void hideIntro(bool type)
    {
        toOptions = type;
        introMenu.GetComponent<Animator>().SetBool("hide", true);
    }
    public void displayIntro()
    {
        optionMenu.SetActive(false );
        introMenu.SetActive(true);
        introMenu.GetComponent<Animator>().SetBool("hide", false);
    }
    public void hideOptions()
    {
        optionMenu.GetComponent<Animator>().SetBool("hide", true);
        if (GameManager.instance.getConnected())
            EventSystem.current.SetSelectedGameObject(introMenu.transform.GetChild(1).gameObject);
    }

    public void displayCredits()
    {
        introMenu.SetActive(false);
        creditsMenu.SetActive(true);
        creditsMenu.GetComponent<Animator>().SetBool("hide", false);
        if (GameManager.instance.getConnected())
            EventSystem.current.SetSelectedGameObject(creditsMenu.transform.GetChild(4).gameObject);
    }
    public void hideCredits()
    {
        creditsMenu.GetComponent<Animator>().SetBool("hide", true);
        if (GameManager.instance.getConnected())
            EventSystem.current.SetSelectedGameObject(introMenu.transform.GetChild(1).gameObject);
    }

    public void setFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
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

    public void PlayButton()
    {
        GameManager.instance.loadNextLevel();
        Instantiate(fadeObject, introMenu.transform.parent.parent);
    }

    public void displayForm()
    {
        formMenu.SetActive(true);
    }

    public void sendFormInfo()
    {
        // No ha pinchado en ningun numero
        if(GameManager.instance.getFormValue() == 0)
        {
            formInfoAnim.SetTrigger("Appear");
        }
        else
        {
            closeForm();
            Tracker.Instance.AddEvent(new FormDataEvent(GameManager.instance.getFormValue()));
        }
    }

    public void closeForm()
    {
        formAnim.SetTrigger("Close");
        // El form se desactiva desde un evento de animacion
    }
    public void changeFormValue(GameObject tObject)
    {
        Toggle t = tObject.GetComponent<Toggle>();
        int v =  tObject.GetComponent<FormValue>().getValue();
        if (t.isOn)
            GameManager.instance.setFormValue(v);
        else
            GameManager.instance.setFormValue(0);
    }

    public void hoverSound()
    {
        RuntimeManager.PlayOneShot(GameManager.instance.GetSoundResources().UI_CHANGE);
    }
    public void pressSound()
    {
        RuntimeManager.PlayOneShot(GameManager.instance.GetSoundResources().UI_ACCEPT);
    }

}
