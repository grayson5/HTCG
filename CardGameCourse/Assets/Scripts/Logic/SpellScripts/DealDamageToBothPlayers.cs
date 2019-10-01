using UnityEngine;
using System.Collections;

public class DealDamageToBothPlayers : SpellEffect 
{
    

    public override void ActivateEffect(int specialAmount = 0, ICharacter target = null, Player caster = null, Player ptarget = null, string specialData = null)
    {
        if (ptarget != null && caster != null)
        {
            ptarget.damageaccrued += specialAmount;
            caster.damageaccrued += specialAmount;
            new ShowMessageCommand(specialAmount + " Damage to Both Players", 1f).AddToQueue();
        }

    }
}
