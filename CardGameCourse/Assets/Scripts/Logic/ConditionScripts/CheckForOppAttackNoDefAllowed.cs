using UnityEngine;
using System.Collections;

public class CheckForOppAttackNoDelAllowed : ConditionEffect
{
    

    public override bool CheckCondition (int specialAmount = 0, ICharacter target = null, Player caster = null, Player ptarget = null, string specialName = null, string cardname = null)
    {
        bool attackexists = false;
        bool defexists = false;

        Debug.Log("In CheckForOppAttackNoDefAllowed");

        //Check to see if an Attack is in play

        for (int i = 0; i < ptarget.table.CreaturesOnTable.Count; i++)
        {
            if (ptarget.table.CreaturesOnTable[i].ca.AttackDefense == "A" && ptarget.table.CreaturesOnTable[i].ca.TypeOfAttack != AttackTypes.Ranged)
            {
                attackexists = true;
                break;
            }
        }

        for (int i = 0; i < caster.table.CreaturesOnTable.Count; i++)
        {
            if (caster.table.CreaturesOnTable[i].ca.AttackDefense == "D" || caster.table.CreaturesOnTable[i].ca.AttackDefense == "G")
            {
                defexists = true;
                break;
            }

        }

        if (attackexists && defexists == false)
            return true;
        else
            return false;
    }
}
