using UnityEngine;
using System.Collections;

public class OpponentCantAttack : SpellEffect 
{
    

    public override void ActivateEffect(int specialAmount = 0, ICharacter target = null, Player caster = null, Player ptarget = null, string specialData = null)
    {
        Debug.Log("In Opponent Can't Attack");

        if (caster != null && ptarget != null)
        {
            ptarget.CanAttackThisTurn = false;
            new ShowMessageCommand(ptarget.name +" can't attack next turn", 1f).AddToQueue();
        }

    }
}
