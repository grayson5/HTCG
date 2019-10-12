using UnityEngine;
using System.Collections;

public class Combination : SpellEffect 
{
    
    public override void ActivateEffect(int specialAmount = 0, ICharacter target = null, Player caster = null, Player ptarget = null, string specialData = null)
    {
        Debug.Log("In Combination");
        caster.AttacksLeftThisTurn++;
        caster.NextAttackHidden = true;
        caster.PArea.HiddenIconButton1.image.enabled = true;
        new ShowMessageCommand(caster.name + " gets additional hidden attack", 2f).AddToQueue();
        caster.HighlightPlayableCards();

    }
}
