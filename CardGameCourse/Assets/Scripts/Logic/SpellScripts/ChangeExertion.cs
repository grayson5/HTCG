using UnityEngine;
using System.Collections;

public class ChangeExertion : SpellEffect 
{
    

    public override void ActivateEffect(int specialAmount = 0, ICharacter target = null, Player caster = null, Player ptarget = null, string specialData = null)
    {
        Debug.Log("In ChangeExertion");

        if (caster != null)
        {
            caster.NumCardsforExertion = specialAmount;
            new ShowMessageCommand(caster.name + " has a " + specialAmount + " card Exertion", 1f).AddToQueue();
            //Debug.Log("HardExertionsLeft: " + caster.HardExertionsLeft);
        }

    }
}
