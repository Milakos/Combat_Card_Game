using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    public Sprite fireDemon = null;
    public Sprite iceDemon = null;
    public Sprite DestructBallImage = null;
    public bool playerTurn = true;
    public TMP_Text turnText;
    public TMP_Text scoretext;
    public int playerScore =0, playerKills =0;
    public Image enemySkipImage;
    [SerializeField] private int mainMenuScene;
    
    public AudioSource playerDieAudio;
    public AudioSource enemyDieAudio;
    private void Awake() 
    {
        instance = this;  
        SetUpEnemy();
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
        if(playerTurn && isPlayable)
        NextPlayersTurn();
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
        player.glowImage.gameObject.SetActive(false);
        enemy.glowImage.gameObject.SetActive(false);
        fromHand.RemoveCard(card);
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
            usingOnPlayer.SetMirror(true);
            usingOnPlayer.PlayMirrorSound();
            NextPlayersTurn();
            isPlayable = true;
        }
        else
        {
            if (card.cardData.isDefenseCard)
            {
                usingOnPlayer.health += card.cardData.damage;
                usingOnPlayer.PlayHealSound();
                if(usingOnPlayer.health > usingOnPlayer.maxHealth)
                {
                    usingOnPlayer.health = usingOnPlayer.maxHealth;
                }

                UpdateHealths();
                StartCoroutine(CastHealthEffect(usingOnPlayer));
            }
            else
            {
                CastAttackEffect(card, usingOnPlayer);
            }

            if(fromHand.isPlayers)
                playerScore += card.cardData.damage;
            UpdateScore();
        }
        if(fromHand.isPlayers)
        {
            GameController.instance.player.mana -= card.cardData.cost;
            GameController.instance.player.UpdateManaBalls();
        }
        else
        {
            GameController.instance.enemy.mana -= card.cardData.cost;
            GameController.instance.enemy.UpdateManaBalls();
        }
    }

    private IEnumerator CastHealthEffect(Player usingOnPlayer)
    {
        yield return new WaitForSeconds(0.5f);
        NextPlayersTurn();
        isPlayable = true;
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
                    effect.PlayFireSound();
                break;

                case CardData.DamageType.ice:
                    if(card.cardData.isMulti)
                        effect.effectImage.sprite = multiIceBall;
                    else
                        effect.effectImage.sprite = iceBallImage;
                    effect.PlayFireSound();
                break;

                case CardData.DamageType.Both:
                        effect.effectImage.sprite = fireAndIceBall;
                        effect.PlayFireSound();
                        effect.PlayIceSound();
                break;
                case CardData.DamageType.Destuct:
                        effect.effectImage.sprite = DestructBallImage;
                        effect.PlayBoomSound();
                break;
            }
        }
    }

    internal void UpdateHealths()
    {
        player.UpdateHealth();
        enemy.UpdateHealth();

        if(player.health <= 0)
        {
            StartCoroutine(GameOver());
        }
        if(enemy.health <= 0)
        {
            playerKills++;
            playerScore+=100;
            UpdateScore();
            StartCoroutine(NewEnemy());
        }

    }

    private IEnumerator NewEnemy()
    {
        enemy.gameObject.SetActive(false);
        enemysHand.ClearHand();
        yield return new WaitForSeconds(0.75f);
        SetUpEnemy();
        enemy.gameObject.SetActive(true);
        StartCoroutine(DealHands());

    }

    private void SetUpEnemy()
    {
        enemy.mana = 0;
        enemy.health = 5;
        enemy.UpdateHealth();
        enemy.isFire = true;
        if(UnityEngine.Random.Range(0,2)==1)
        {
            enemy.isFire = false;
        }
        if(enemy.isFire)
        {
            enemy.playerImage.sprite = fireDemon;
        }
        else
        {
            enemy.playerImage.sprite = iceDemon;  
        }
    }

    private IEnumerator GameOver()
    {
        yield return new WaitForSeconds(1);
        UnityEngine.SceneManagement.SceneManager.LoadScene(2);
    }
    internal void NextPlayersTurn()
    {
        playerTurn = !playerTurn;
        bool enemyisDead = false;
        if(playerTurn)
        {
            if(player.mana < 5)
                player.mana ++;
        }
        else
        {
            if(enemy.health > 0)
            {
                if(enemy.mana < 5)
                enemy.mana ++;
            }else
            {
                enemyisDead = true;
            }

        }

        if(enemyisDead)
        {
            playerTurn = !playerTurn;
            if(player.mana <5)
            player.mana++;
        }
        else
        {
            SetTurnText();
            if(!playerTurn)
                MonstersTurn();
        }

        player.UpdateManaBalls();
        enemy.UpdateManaBalls();
    }

    private void MonstersTurn()
    {
        Card card = AIChooseCard();
        StartCoroutine(MonsterCastCard(card));
    }

    private Card AIChooseCard()
    {
        List<Card> available = new List<Card>();
        for(int i=0;i<3;i++)
        {
            if(CardValid(enemysHand.cards[i], enemy, enemysHand))
            {
                available.Add(enemysHand.cards[i]);
            }
            else if(CardValid(enemysHand.cards[i], player, enemysHand))
            {
                available.Add(enemysHand.cards[i]);
            }
        }

        if(available.Count == 0)
        {
            NextPlayersTurn();
            return null;
        }
        int choice = UnityEngine.Random.Range(0, available.Count);
        return available[choice];
    }
    private IEnumerator MonsterCastCard(Card card)
    {
        yield return new WaitForSeconds(0.5f);

        if(card)
        {
            TurnCard(card);
            yield return new WaitForSeconds(2);
            
            if(card.cardData.isDefenseCard)
                UseCard(card, enemy, enemysHand);
            else
                UseCard(card, player, enemysHand);
            
            yield return new WaitForSeconds(1);

            enemyDeck.DealCard(enemysHand);

            yield return new WaitForSeconds(1);
        }
        else
        {
            enemySkipImage.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
            enemySkipImage.gameObject.SetActive(false);
        }
    }

    internal void TurnCard(Card card)
    {
        Animator animator = card.GetComponentInChildren<Animator>();
        if(animator)
        {
            animator.SetTrigger("Flip");
        }
        else
        {
            Debug.LogError("No Animator found");
        }
    }
    internal void SetTurnText()
    {
        if(playerTurn)
        {
            turnText.text = "Merlins Turn";
        }
        else
        {
            turnText.text = "Enemy's Turn";
        }
    }
private void UpdateScore()
{
    scoretext.text = "Demons Killed: "+ playerKills.ToString() + ".  Score: "+ playerScore.ToString();
}

    internal void PlayPlayerDieSound()
    {
        playerDieAudio.Play();
    }
    internal void PlayEnemySound()
    {
        enemyDieAudio.Play();
    }
}
