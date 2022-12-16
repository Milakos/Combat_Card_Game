using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BurnZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public bool cardEnterd = false;
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("Card droped in burned zone");

        GameObject obj = eventData.pointerDrag;
        Card card = obj.GetComponent<Card>();
        
        if (card != null)
        {
            GameController.instance.playersHand.BurnCard(card);
        }
        else return;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        print("Enter in burnzone");
        cardEnterd = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        print("Exit in burnzone");
         cardEnterd = false;
    }
}
