using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    static public GameController instance = null;

    public Deck playerDeck = new Deck();
    public Deck enemyDeck = new Deck();

    public Hand playersHand = new Hand();
    public Hand enemysHand = new Hand();

    public Player player = null;
    public Player enemy = null;

    public List<CardData> cards = new List<CardData>();

    public Sprite[] healthNumbers = new Sprite[10];
    public Sprite[] damageNumbers = new Sprite[10];
    public GameObject cardPrefab = null;
    public Canvas canvas = null;

    public bool isPlayable = false;

    public GameObject effectFromLeftPrefab = null;
    public GameObject effectFromRightPrefab = null;

    public Sprite fireballImage;
    public Sprite iceBallImage;
    public Sprite multiFireBall;
    public Sprite multiIceBall;
    public Sprite fireAndIceBall;


    [SerializeField]
    private int mainMenuScene;
    private void Awake() 
    {
        instance = this;  

        playerDeck.Create();
        enemyDeck.Create();  
        StartCoroutine(DealHands());
    }
    public void Quit()
    {
        SceneManager.LoadSceneAsync(mainMenuScene);
    }

    public void SkipTurn()
    {

    }

    internal IEnumerator DealHands()
    {
        yield return new WaitForSeconds(1);
        for (int t = 0; t < 3; t++)
        {
            playerDeck.DealCard(playersHand);
            enemyDeck.DealCard(enemysHand);
            yield return new WaitForSeconds(1);
        }
        isPlayable = true;
    }

    internal bool UseCard(Card card, Player usingOnPlayer, Hand fromHand)
    {
        if(!CardValid(card, usingOnPlayer, fromHand))
        {
            return false;
        }
        isPlayable = false;
        CastCard(card,usingOnPlayer,fromHand);
        
        return false;
    }

    internal bool CardValid(Card cardBeingPlayed, Player usingInPlayer, Hand fromHand)
    {
        bool valid = false;

        if(cardBeingPlayed == null)
            return false;
        if(fromHand.isPlayers)
        {
            if(cardBeingPlayed.cardData.cost <= player.mana)
            {
                if(usingInPlayer.isPlayer && cardBeingPlayed.cardData.isDefenseCard)
                {
                    valid = true;
                }
                if(!usingInPlayer.isPlayer && !cardBeingPlayed.cardData.isDefenseCard)
                {
                    valid = true;
                }
            }
        }
        else
        {
                if(cardBeingPlayed.cardData.cost <= enemy.mana)
            {
                if(!usingInPlayer.isPlayer && cardBeingPlayed.cardData.isDefenseCard)
                {
                    valid = true;
                }
                if(usingInPlayer.isPlayer && !cardBeingPlayed.cardData.isDefenseCard)
                {
                    valid = true;
                }
            }
        }
        return valid;
    }
    internal void CastCard(Card card, Player usingOnPlayer, Hand fromHand)
    {
        if(card.cardData.isMirrorCard)
        {

        }
        else
        {
            if (card.cardData.isDefenseCard)
            {

            }
            else
            {
                CastAttackEffect(card, usingOnPlayer);
            }
        }
    }

    internal void CastAttackEffect(Card card, Player usingOnPLayer)
    {
        GameObject effectGO = null;
        if(usingOnPLayer.isPlayer)
        {
            effectGO = Instantiate(effectFromRightPrefab, canvas.gameObject.transform);
        }
        else
        {
            effectGO = Instantiate(effectFromLeftPrefab, canvas.gameObject.transform);
        }

        Effect effect = effectGO.GetComponent<Effect>();
        if(effect)
        {
            effect.targetPlayer = usingOnPLayer;
            effect.sourceCard = card;

            switch(card.cardData.damageType)
            {
                case CardData.DamageType.fire:
                    if(card.cardData.isMulti)
                        effect.effectImage.sprite = multiFireBall;
                    else
                        effect.effectImage.sprite = fireballImage;
                break;

                case CardData.DamageType.ice:
                    if(card.cardData.isMulti)
                        effect.effectImage.sprite = multiIceBall;
                    else
                        effect.effectImage.sprite = iceBallImage;
                break;

                case CardData.DamageType.Both:
                        effect.effectImage.sprite = fireAndIceBall;
                break;
            }
        }
    }
}
