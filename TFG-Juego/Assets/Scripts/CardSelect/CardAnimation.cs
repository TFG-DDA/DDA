using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class CardAnimation : MonoBehaviour
{
    Sprite back, front;
    GameObject child_sprite;
    GameObject child_text;
    TextMeshProUGUI cardtext;
    string auxText = "";
    bool flip = false;
    MovementVFX movementVFX;

    [SerializeField] 
    CardSelector cardselector;

    private void Awake()
    {
        child_sprite = transform.GetChild(0).gameObject;
        child_text = child_sprite.transform.GetChild(0).gameObject;
        cardtext = child_text.GetComponent<TextMeshProUGUI>();
        movementVFX = transform.GetComponentInParent<MovementVFX>();
    }

    public void setDataCard(Sprite new_spriteB, Sprite new_spriteF, string t)
    {
        back = new_spriteB;
        front = new_spriteF;
        child_sprite.GetComponent<Image>().sprite = back;
        auxText = t;    
    }

    public void flipSprite()
    {
        child_sprite.GetComponent<Image>().sprite = front;
        cardtext.text = auxText;
        movementVFX.enabled = true;
        cardselector.enableButtons(true);
        RuntimeManager.PlayOneShot(GameManager.instance.GetSoundResources().CARD_FLIP);
    }

    public bool isFlip() { return flip; }

}
