using UnityEngine;
using System.Collections;

public class RunThrough : SpellEffect 
{
    

    public override void ActivateEffect(int specialAmount = 0, ICharacter target = null, Player caster = null, Player ptarget = null, string specialData = null)
    {
        Debug.Log("In RunThrough");

        if (caster != null)
        {
            caster.CanPlayDefenses = false;
            ptarget.CanPlayBlocks = false;
            new ShowMessageCommand("You cannot play Defenses this turn and your next attack cannot be blocked", 1f).AddToQueue();
        }

    }
}
