using UnityEngine;
using System.Collections;

public class CheckForOppAttack : ConditionEffect
{
    

    public override bool CheckCondition (int specialAmount = 0, ICharacter target = null, Player caster = null, Player ptarget = null, string specialName = null, string cardname = null)
    {
        bool attackexists = false;

        Debug.Log("In CheckForOppAttack");

        //Check to see if an Attack is in play

        for (int i = 0; i < ptarget.table.CreaturesOnTable.Count; i++)
        {
            if (ptarget.table.CreaturesOnTable[i].ca.AttackDefense == "A")
            {
                attackexists = true;
                break;
            }
        }


        if (attackexists)
            return true;
        else
            return false;

    }
}
