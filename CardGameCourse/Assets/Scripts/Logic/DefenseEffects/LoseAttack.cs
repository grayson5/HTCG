using UnityEngine;
using System.Collections;

public class LoseAttack : DefenseEffect 
{
    

    public override void ActivateEffect(int specialAmount = 0, ICharacter target = null, Player caster = null, Player ptarget = null, string specialData = null)
    {
        Debug.Log("In LoseAttack");
        if (caster != null)
        {
            caster.AttacksLeftThisTurn = caster.AttacksLeftThisTurn - specialAmount;
            new ShowMessageCommand(caster.name + " loses "+ specialAmount + " attack", 1f).AddToQueue();
        }

    }
}
