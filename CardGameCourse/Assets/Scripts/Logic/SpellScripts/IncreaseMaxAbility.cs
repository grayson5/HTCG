using UnityEngine;
using System.Collections;

public class IncreaseMaxAbility : SpellEffect 
{
    

    public override void ActivateEffect(int specialAmount = 0, ICharacter target = null, Player caster = null, Player ptarget = null, string specialData = null)
    {
        Debug.Log("In IncreaseMaxAbility");

        if (caster != null)
        {
            caster.TempAddAbility += specialAmount;
            //caster.abilitygained += specialAmount;
            new ShowMessageCommand(caster.name + " Gains " + specialAmount + " Ability", 1f).AddToQueue();
            Debug.Log("TempAddAbility: " + caster.TempAddAbility);
        }

    }
}
