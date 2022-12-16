using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraAbdDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Card card;
    private Vector3 originalPosition;
    private bool escCheck = false;

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = transform.position;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!GameController.instance.isPlayable)
            return;
        transform.position += (Vector3)eventData.delta;

        Card card = GetComponent<Card>();
        
        foreach(GameObject hover in eventData.hovered)
        {
            BurnZone burnZone = hover.GetComponent<BurnZone>();
       
                if(burnZone != null )
                {
                    if(burnZone.cardEnterd == true)
                    {
                        card.burnImage.gameObject.SetActive(true);
                        print("burnTrue");
                    }
                }     
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Card card = GetComponent<Card>();
       transform.position = originalPosition;
       GetComponent<CanvasGroup>().blocksRaycasts = true;

        foreach(GameObject hover in eventData.hovered)
        {
            BurnZone burnZone = hover.GetComponent<BurnZone>();
            if(burnZone == null)
            {
                card.burnImage.gameObject.SetActive(false);
                print("burnfalse");
            }
        }
    }

}
