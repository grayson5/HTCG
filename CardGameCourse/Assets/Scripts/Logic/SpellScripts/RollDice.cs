using UnityEngine;
using System.Collections;

public class RollDice : SpellEffect 
{

    public override void ActivateEffect(int specialAmount = 0, ICharacter target = null, Player caster = null, Player ptarget = null, string specialData = null)
    {
        Debug.Log("In RollDice");
        

        if (caster != null && ptarget != null)
        {

            caster.HighlightPlayableCards(true);
            GameObject.FindWithTag("Dice1").GetComponent<SpriteRenderer>().enabled = true;
            GameObject.FindWithTag("Dice1").GetComponent<BoxCollider2D>().enabled = true;
            Debug.Log("Reason: " + specialData);
            caster.rollreason = specialData;
            caster.rollamount = specialAmount;
            caster.ProcessEventCard = true;
            if (specialData == "Disarm")
                caster.AttacksLeftThisTurn--;

            new ShowMessageCommand("Touch the Dice to Roll it......", 2f).AddToQueue();

        }

    }
}
