using UnityEngine;
using System.Collections;

public class NoSpecialsAllowed : SpellEffect 
{
    

    public override void ActivateEffect(int specialAmount = 0, ICharacter target = null, Player caster = null, Player ptarget = null, string specialData = null)
    {
        Debug.Log("In NoSpecialsAllowed");

        if (caster != null)
        {
            caster.CanPlaySpecials = false;
            caster.CanPlaySpecialsCounter += (specialAmount + 1);
            new ShowMessageCommand("Special cards cannot be played for " + specialAmount + " turn", 1f).AddToQueue();

        }

    }
}
