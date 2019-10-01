using UnityEngine;
using System.Collections;

public class RecoverWeapon : SpellEffect 
{

    public override void ActivateEffect(int specialAmount = 0, ICharacter target = null, Player caster = null, Player ptarget = null, string specialData = null)
    {
        Debug.Log("In RecoverWeapon");
        

        if (caster != null )
        {
            caster.Disarmed = false;
            new ShowMessageCommand(caster.name+" has recovered their weapon!", 2f).AddToQueue();
        }

    }
}
