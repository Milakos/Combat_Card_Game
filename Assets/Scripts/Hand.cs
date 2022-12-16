using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Hand
{
    public Card[] cards = new Card[3];
    public Transform[] positions = new Transform[3];
    public string[] animationNames = new string[3];
    public bool isPlayers;
    public void BurnCard(Card card)
    {
        for (int i = 0; i < cards.Length; i++)
        {
            if(cards[i] == card)
            {
                GameObject.Destroy(cards[i].gameObject);
                cards[i] = null;
                if(isPlayers)
                {
                    GameController.instance.playerDeck.DealCard(this);
                }
                else
                {
                    GameController.instance.enemyDeck.DealCard(this);
                }
                break;
            }
        }
    }
}