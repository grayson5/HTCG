using UnityEngine;
using System.Collections;

public class AddExertion : SpellEffect 
{
    

    public override void ActivateEffect(int specialAmount = 0, ICharacter target = null, Player caster = null, Player ptarget = null, string specialData = null)
    {
        Debug.Log("In AddExertion");

        if (caster != null)
        {
            caster.HardExertionsLeft = caster.HardExertionsLeft + specialAmount;
            //caster.abilitygained += specialAmount;
            new ShowMessageCommand(caster.name + " Gains " + specialAmount + " Exertion", 1f).AddToQueue();
            Debug.Log("HardExertionsLeft: " + caster.HardExertionsLeft);
        }

    }
}
