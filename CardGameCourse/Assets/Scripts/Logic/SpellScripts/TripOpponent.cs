using UnityEngine;
using System.Collections;

public class TripOpponent : SpellEffect 
{
    

    public override void ActivateEffect(int specialAmount = 0, ICharacter target = null, Player caster = null, Player ptarget = null, string specialData = null)
    {
        Debug.Log("In TripOpponent");

        if (caster != null && ptarget != null)
        {
            caster.NextAttackHidden = true;
            caster.PArea.HiddenIconButton1.image.enabled = true;
            if (ptarget.GuardInPlay == true)
            {
                ptarget.DropGuard();
            }
            //new ShowMessageCommand(ptarget.name +" can't attack next turn", 1f).AddToQueue();
        }

    }
}
