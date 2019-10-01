using UnityEngine;
using System.Collections;

public class NoDodgesForBothPlayers : SpellEffect 
{
    

    public override void ActivateEffect(int specialAmount = 0, ICharacter target = null, Player caster = null, Player ptarget = null, string specialData = null)
    {
        Debug.Log("In NoDodgesForBothPlayers");

        if (caster != null && ptarget != null)
        {
            caster.CanPlayDodges = false;
            ptarget.CanPlayDodges = false;
            //caster.TurnCounterOn = true;
            caster.DodgeTurnCounter += (specialAmount + 1);
            new ShowMessageCommand("Dodges cannot be played until the end of "+caster.name+"'s next turn", 1f).AddToQueue();

        }

    }
}
