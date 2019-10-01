using UnityEngine;
using System.Collections;

public class BothPlayersCantAttack : SpellEffect 
{
    

    public override void ActivateEffect(int specialAmount = 0, ICharacter target = null, Player caster = null, Player ptarget = null, string specialData = null)
    {
        Debug.Log("In Both Players Opponent Can't Attack");

        if (caster != null && ptarget != null)
        {
            caster.CanAttackThisTurn = false;
            ptarget.CanAttackThisTurn = false;
            ptarget.BothPlayersCantAttackCounter += specialAmount;
            caster.HighlightPlayableCards();
        }

    }
}
