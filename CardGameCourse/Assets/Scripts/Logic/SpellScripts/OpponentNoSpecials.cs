using UnityEngine;
using System.Collections;

public class OpponentNoSpecials : SpellEffect 
{
    

    public override void ActivateEffect(int specialAmount = 0, ICharacter target = null, Player caster = null, Player ptarget = null, string specialData = null)
    {
        Debug.Log("In OpponentNoSpecials");

        if (caster != null)
        {
            ptarget.CanPlaySpecials = false;
            new ShowMessageCommand(ptarget.name + " cannot play Special cards next turn", 1f).AddToQueue();
        }

    }
}
