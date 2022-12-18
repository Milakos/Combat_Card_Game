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
        Card draggingCard = GetComponent<Card>();
        bool overCard = false;
        foreach(GameObject hover in eventData.hovered)
        {
            Player playerCard = hover.GetComponent<Player>();
            if(playerCard != null)
            {
                if(GameController.instance.CardValid(draggingCard, playerCard, GameController.instance.playersHand))
                {
                    playerCard.glowImage.gameObject.SetActive(true);
                    overCard = true;
                }
            }
            BurnZone burnZone = hover.GetComponent<BurnZone>();
       
                if(burnZone != null )
                {
                    if(burnZone.cardEnterd == true)
                    {
                        card.burnImage.gameObject.SetActive(true);
                        print("burnTrue");
                    }
                    else
                    {
                        card.burnImage.gameObject.SetActive(false);
                    }
                }
                if(!overCard)
                {
                    GameController.instance.player.glowImage.gameObject.SetActive(false);
                    GameController.instance.enemy.glowImage.gameObject.SetActive(false);
                    // overCard = false;                    
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
