using UnityEngine;
using System.Collections;

public class PlayOnlyRangedAttacks : DefenseEffect 
{
    

    public override void ActivateEffect(int specialAmount = 0, ICharacter target = null, Player caster = null, Player ptarget = null, string specialData = null)
    {
        Debug.Log("In PlayOnlyRangedAttacks");
        if (caster != null)
        {
            caster.PlayOnlyRangedAttacks = true;
            caster.AttackAreasBlocked = true;
            new ShowMessageCommand(caster.name + " can only played ranged attacks", 1f).AddToQueue();
        }

    }
}
