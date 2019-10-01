using UnityEngine;
using System.Collections;

public class TargetDiscardCard : SpellEffect 
{
    

    public override void ActivateEffect(int specialAmount = 0, ICharacter target = null, Player caster = null, Player ptarget = null, string specialData = null)
    {
        string msg = "";

        if(target.ID == ptarget.ID)
        {
            ptarget.MustOrMayAction = true;
            ptarget.MMDiscardCards = specialData;

            switch (specialData)
            {
                case "A":
                    ptarget.MMDiscardAttacks = specialAmount;
                    ptarget.MayMustMessage = "You must discard " + specialAmount + " Attack cards";
                    msg = ptarget.name + " must discard " + specialAmount + " Attack cards";
                    break;
                case "C":
                    ptarget.MMDiscardAnyCards = specialAmount;
                    ptarget.MayMustMessage = "You must discard " + specialAmount + " cards";
                    msg = ptarget.name + " must discard " + specialAmount + " cards";
                    break;
                case "P":
                    ptarget.MMDiscardAnyCards = specialAmount;
                    ptarget.MayMustMessage = " You must discard ALL plot card";
                    msg = ptarget.name + " must discard ALL plot cards";
                    break;
            }
            new ShowMessageCommand(msg, 1f).AddToQueue();
        }
            
    }
}
