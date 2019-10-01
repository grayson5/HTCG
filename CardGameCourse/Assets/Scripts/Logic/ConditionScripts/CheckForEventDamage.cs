using UnityEngine;
using System.Collections;

public class CheckForEventDamage : ConditionEffect
{
    

    public override bool CheckCondition (int specialAmount = 0, ICharacter target = null, Player caster = null, Player ptarget = null, string specialName = null, string cardname = null)
    {
        if (caster.EventDamage > 0)
        {
            Debug.Log("Has Event Damage");
            return true;
        }
        else
        {
            Debug.Log("No Event Damage");
            return false;
        }
    }
}
