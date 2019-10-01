using UnityEngine;
using System.Collections;

public class IsAttackLeftAndCanAttack : ConditionEffect
{
    

    public override bool CheckCondition (int specialAmount = 0, ICharacter target = null, Player caster = null, Player ptarget = null, string specialName = null, string cardname = null)
    {

        Debug.Log("In IsAttackLeft");

        //Check to see if caster has attacks left to play

        if (caster.AttacksLeftThisTurn > 0 && ptarget.Disarmed == false && caster.CanAttackThisTurn == true)
            return true;
        else
            return false;

    }
}
