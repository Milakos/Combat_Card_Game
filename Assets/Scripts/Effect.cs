using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Effect : MonoBehaviour
{
    public Player targetPlayer = null;
    public Card sourceCard = null;
    public Image effectImage = null;

    public void EndTrigger()
    {
        bool bounce = false;

        if(targetPlayer.hasMirror())
        {
            targetPlayer.SetMirror(false);
            bounce = true;
            if(targetPlayer.isPlayer)
            {
                GameController.instance.CastAttackEffect(sourceCard, GameController.instance.enemy);
            }
            else
            {
                GameController.instance.CastAttackEffect(sourceCard, GameController.instance.player);
            }
        }
        else
        {
            int damage = sourceCard.cardData.damage;
            if(!targetPlayer.isPlayer)
            {
                if(sourceCard.cardData.damageType == CardData.DamageType.fire && targetPlayer.isFire)
                    damage = damage / 2;
                if(sourceCard.cardData.damageType == CardData.DamageType.ice && !targetPlayer.isFire)
                    damage = damage / 2;
            }
            targetPlayer.health -= damage;
            targetPlayer.PlayHitAnim();
            if(!bounce)
                GameController.instance.NextPlayersTurn();
            GameController.instance.UpdateHealths();

            GameController.instance.isPlayable = true;
        }
        Destroy(gameObject);
    }
}
