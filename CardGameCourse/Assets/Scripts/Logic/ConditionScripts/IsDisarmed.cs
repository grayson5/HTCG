using UnityEngine;
using System.Collections;

public class IsDisarmed : ConditionEffect
{
    

    public override bool CheckCondition (int specialAmount = 0, ICharacter target = null, Player caster = null, Player ptarget = null, string specialName = null, string cardname = null)
    {

        Debug.Log("In IsDisarmed");

        //Check to see if caster has attacks left to play

        if (caster.Disarmed == true)
            return true;
        else
            return false;

    }
}
