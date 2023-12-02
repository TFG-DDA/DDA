using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct Card_Data
{
    public GameObject obj;
    public Card data;
}
public class CardSelector : MonoBehaviour
{
    [SerializeField]
    GameObject Dedo;

    [SerializeField]
    GameObject Card_IZ;
    [SerializeField]
    GameObject Card_DE;

    Card_Data card_left;
    Card_Data card_right;
    Card_Data card_selected;

    // Referencias a Botones
    Button button_left;
    Button button_right;
    bool interactable = false;

    [SerializeField]
    GameObject fadeObject;

    // Start is called before the first frame update
    void Start()
    {
        card_selected = new Card_Data();

        card_left = new Card_Data();
        card_left.obj = Card_IZ;

        card_right = new Card_Data();
        card_right.obj = Card_DE;

        Card[] cartas = GameManager.instance.getCard();
        // Elegimos 2 cartas del gamemanager
        card_left.data = cartas[0];
        card_right.data = cartas[1];

        // Asignamos los sprites que necesite
        Card_IZ.transform.GetChild(0).GetComponent<CardAnimation>().setDataCard(card_left.data.backImage, card_left.data.frontImage, card_left.data.text);
        Card_DE.transform.GetChild(0).GetComponent<CardAnimation>().setDataCard(card_right.data.backImage, card_right.data.frontImage, card_right.data.text);

        // Cogemos los botones de las cartas
        button_left = Card_IZ.transform.GetChild(0).GetComponentInChildren<Button>();
        button_right = Card_DE.transform.GetChild(0).GetComponentInChildren<Button>();
        enableButtons(false);

        if (PlayerInstance.instance != null)
            PlayerInstance.instance.gameObject.SetActive(false);
        if (UIManager.instance != null)
        {
            UIManager.instance.gameObject.SetActive(false);
            UIManager.instance.ResetFade();
        }
    }

    public void ItemSelected(bool right)
    {
        enableButtons(false);
        if(right)
        {
            card_selected = card_right;
            Debug.Log("DERECHA");
        }
        else
        { 
            card_selected = card_left; 
            Debug.Log("IZQUIERDA");
        }

        // Activo el animator del dedo
        Animator anim = Dedo.GetComponent<Animator>();
        anim.enabled = true;

        // Movemos el dedo a la posicion de la carta en la X
        RectTransform dedo_r = Dedo.GetComponent<RectTransform>();
        RectTransform carta_r = card_selected.obj.GetComponent<RectTransform>();
        dedo_r.anchoredPosition = new Vector2(carta_r.anchoredPosition.x, dedo_r.anchoredPosition.y);

        Dedo.GetComponent<FingerToCard>().setCard(card_selected.obj);
    }

    public void passScene()
    {
        // Avisamos al GameManager la carta que se ha elegido para que aplique los cambios
        Instantiate(fadeObject, Dedo.transform.parent);
        GameManager.instance.applyCard(card_selected.data);
    }

    public void chooseCardSound()
    {
        if(interactable)
        {
            RuntimeManager.PlayOneShot(GameManager.instance.GetSoundResources().CARD_CHOOSE);
        }
    }

    public void outCardSound()
    {
        Camera.main.GetComponent<CameraShake>().StartShake();
        RuntimeManager.PlayOneShot(GameManager.instance.GetSoundResources().CARD_OUT);
        RuntimeManager.PlayOneShot(GameManager.instance.GetSoundResources().GUN_REVOLVER);
    }

    public void enableButtons(bool interact)
    {
        interactable = interact;
        button_left.interactable = interact;
        button_right.interactable = interact;
    }
}
