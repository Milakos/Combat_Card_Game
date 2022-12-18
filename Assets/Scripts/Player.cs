using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Player : MonoBehaviour, IDropHandler
{
    public Image playerImage = null;
    public Image mirrorImage = null;
    public Image healthNUmberImage = null;
    public Image glowImage = null;
    public int maxHealth = 5;
    public int health = 5;
    public int mana = 1;
    public bool isPlayer;
    public bool isFire;
    public GameObject[] manaBalls = new GameObject[5];
    private Animator anim = null;

    void Start()
    {
        anim = GetComponent<Animator>();
        UpdateHealth();
        UpdateManaBalls();
    }

    internal void PlayHitAnim()
    {
        if(anim != null)
        {
            anim.SetTrigger("Hit");
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if(!GameController.instance.isPlayable)
            return;
        Debug.Log("Card Drop ont PLaye/Enemy");

        GameObject obj = eventData.pointerDrag;
        if(obj != null)
        {
            Card card = obj.GetComponent<Card>();
            if(card != null)
            {
                GameController.instance.UseCard(card, this, GameController.instance.playersHand);
            }
        }
    }

    internal void UpdateHealth()
    {
        if(health >= 0 && health < GameController.instance.healthNumbers.Length)
        {
            healthNUmberImage.sprite = GameController.instance.healthNumbers[health];
        }
        else
        {
            Debug.LogError("Health is not a valid Number " + health.ToString());
        }
    }

    internal void SetMirror(bool on)
    {
        mirrorImage.gameObject.SetActive(on);
    }
    internal bool hasMirror()
    {
        return mirrorImage.gameObject.activeInHierarchy;
    }
    internal void UpdateManaBalls()
    {
        for(int m = 0; m< 5; m++)
        {
            if(mana>m)
            {
                manaBalls[m].SetActive(true);
            }
            else
            {
                manaBalls[m].SetActive(false);
            }
        }
    }
}
