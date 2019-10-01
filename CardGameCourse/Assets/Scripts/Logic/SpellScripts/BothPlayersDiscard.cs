using UnityEngine;
using System.Collections;

public class BothPlayersDiscard : SpellEffect 
{
    

    public override void ActivateEffect(int specialAmount = 0, ICharacter target = null, Player caster = null, Player ptarget = null, string specialData = null)
    {
        string msg = "";

        if (ptarget != null && caster != null)
        {
            ptarget.MustOrMayAction = true;
            ptarget.MMDiscardCards = specialData;
            caster.MMDiscardCards = specialData;

            switch (specialData)
            {
                case "A":
                    ptarget.MMDiscardAttacks = specialAmount;
                    ptarget.MayMustMessage = "You must discard " + specialAmount + " Attack cards";
                    caster.MMDiscardAttacks = specialAmount; 
                    msg = " Attack cards";
                    break;
            }

            caster.ProcessEventCard = true;

            new ShowMessageCommand("Both players must discard " + specialAmount + msg, 1f).AddToQueue();
        }

    }
}
