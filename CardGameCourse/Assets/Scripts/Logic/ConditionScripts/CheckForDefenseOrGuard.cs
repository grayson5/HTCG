using UnityEngine;
using System.Collections;

public class CheckForDefenseOrGuard : ConditionEffect
{
    

    public override bool CheckCondition (int specialAmount = 0, ICharacter target = null, Player caster = null, Player ptarget = null, string specialName = null, string cardname = null)
    {
        bool blockexists = false;

        Debug.Log("In CheckForDefenseOrGuard");

        //Check to see if a Defense is in play

        for (int i = 0; i < caster.table.CreaturesOnTable.Count; i++)
        {
            if (caster.table.CreaturesOnTable[i].ca.AttackDefense == "D" || caster.table.CreaturesOnTable[i].ca.AttackDefense == "G")
            {
                blockexists = true;
                break;
            }
        }


        if (blockexists)
            return true;
        else
            return false;

    }
}
