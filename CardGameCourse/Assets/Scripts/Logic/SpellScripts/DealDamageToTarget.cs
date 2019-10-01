using UnityEngine;
using System.Collections;

public class DealDamageToTarget : SpellEffect 
{
    

    public override void ActivateEffect(int specialAmount = 0, ICharacter target = null, Player caster = null, Player ptarget = null, string specialData = null)
    {
        if (ptarget != null)
        {
            ptarget.damageaccrued += specialAmount;
            new ShowMessageCommand(specialAmount + " Damage to " + ptarget.name, 1f).AddToQueue();
        }

    }
}
