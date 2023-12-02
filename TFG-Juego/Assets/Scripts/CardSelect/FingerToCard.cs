using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerToCard : MonoBehaviour
{
    GameObject SelectedCard;
    [SerializeField]
    CardSelector cardSelector;

    public void Select()
    {
        // Cambiamos el padre de la carta por el Dedo
        RectTransform finger_r = GetComponent<RectTransform>();
        RectTransform finger_child_r = transform.GetChild(0).gameObject.GetComponent<RectTransform>();
        RectTransform card_r = SelectedCard.transform.parent.GetComponentInParent<RectTransform>();
        card_r.SetParent(finger_r, true);

        // Lo movemos en la jerarquia para el render
        finger_child_r.SetAsLastSibling();
    }

    public void setCard(GameObject card) { SelectedCard = card.transform.GetChild(0).gameObject; }

    public void endScene()
    {
        cardSelector.passScene();
    }
}
