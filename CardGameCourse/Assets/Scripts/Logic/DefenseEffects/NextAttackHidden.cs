using UnityEngine;
using System.Collections;

public class NextAttackHidden : DefenseEffect 
{
    

    public override void ActivateEffect(int specialAmount = 0, ICharacter target = null, Player caster = null, Player ptarget = null, string specialData = null)
    {
        Debug.Log("In NextAttackHidden");
        if (caster != null)
        {
            caster.NextAttackHidden = true;
            caster.PArea.HiddenIconButton1.image.enabled = true;
            new ShowMessageCommand(caster.name + " next attack is hidden", 1f).AddToQueue();
        }

    }
}
