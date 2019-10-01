using UnityEngine;
using System.Collections;

public class ReduceAbility : RemoveFromPlayEffect
{
    

    public override void RemoveEffect (int specialAmount = 0, ICharacter target = null, Player caster = null, Player ptarget = null, string specialName = null, string cardname = null)
    {

        Debug.Log("In ReduceAbility: ");

        if (caster != null)
        {
            caster.TempAddAbility -= specialAmount;
            new ShowMessageCommand(caster.name + " loses " + specialAmount + " Ability", 1f).AddToQueue();
            Debug.Log("TempAddAbility: " + caster.TempAddAbility);
        }
        else if (ptarget != null)
        {
            ptarget.TempAddAbility -= specialAmount;
            new ShowMessageCommand(ptarget.name + " loses " + specialAmount + " Ability", 1f).AddToQueue();
            Debug.Log("TempAddAbility: " + ptarget.TempAddAbility);
        }

    }
}
