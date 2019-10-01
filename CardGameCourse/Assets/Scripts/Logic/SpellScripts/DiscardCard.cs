using UnityEngine;
using System.Collections;

public class DiscardCard : SpellEffect 
{
    

    public override void ActivateEffect(int specialAmount = 0, ICharacter target = null, Player caster = null, Player ptarget = null, string specialData = null)
    {
        string msg = "";

        if (caster != null)
        {
            
            caster.MMDiscardCards = specialData;
            caster.ProcessEventCard = true;

            switch (specialData)
            {
                case "A":
                    caster.MayMustMessage = "You must discard " + specialAmount + " Attack cards";
                    caster.MMDiscardAttacks = specialAmount; 
                    msg = " Attack cards";
                    break;
            }

            new ShowMessageCommand("You must discard " + specialAmount + msg, 1f).AddToQueue();
        }

    }
}
