using UnityEngine;
using System.Collections;

public class RemoveEventDamageFromTarget : SpellEffect 
{
    

    public override void ActivateEffect(int specialAmount = 0, ICharacter target = null, Player caster = null, Player ptarget = null, string specialData = null)
    {
        if (caster != null && caster.EventDamage > 0)
        {
            caster.damageaccrued -= specialAmount;
            caster.EventDamage -= specialAmount;
            new ShowMessageCommand(specialAmount + " Event Damage Prevented for " + caster.name, 1f).AddToQueue();
        }

    }
}
