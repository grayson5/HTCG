using UnityEngine;
using System.Collections;

public class AddAbilityToBothPlayers : SpellEffect 
{
    

    public override void ActivateEffect(int specialAmount = 0, ICharacter target = null, Player caster = null, Player ptarget = null, string specialData = null)
    {
        if (ptarget != null)
        {
            ptarget.abilitygained += specialAmount;
            new ShowMessageCommand(ptarget.name + " Gains " + specialAmount + " Ability", 1f).AddToQueue();
        }

        if (caster != null)
        {
            caster.abilitygained += specialAmount;
            new ShowMessageCommand(caster.name + " Gains " + specialAmount + " Ability", 1f).AddToQueue();
        }

    }
}
