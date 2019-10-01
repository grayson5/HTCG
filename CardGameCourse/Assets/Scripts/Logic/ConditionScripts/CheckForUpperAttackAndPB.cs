using UnityEngine;
using System.Collections;

public class CheckForUpperAttackAndPB : ConditionEffect
{
    

    public override bool CheckCondition (int specialAmount = 0, ICharacter target = null, Player caster = null, Player ptarget = null, string specialName = null, string cardname = null)
    {
        bool attackexists = false;

        Debug.Log("In CheckForUpperAttackAndPB");

        //Check to see if an Upper Attack is in play.

        for (int i = 0; i < caster.table.CreaturesOnTable.Count; i++)
        {
            if (caster.table.CreaturesOnTable[i].ca.AttackDefense == "A")
            {
                string attackgrid = caster.table.CreaturesOnTable[i].ca.GridData;
                string[] attackvalues = attackgrid.Split(',');

                for (int x = 0; x < 3; x++)
                {
                    if (int.Parse(attackvalues[x]) == 1 && caster.table.CreaturesOnTable[i].ca.CanBePowerBlow == true)
                    {
                        attackexists = true;
                        break;
                    }

                }
            }
        }


        if (attackexists)
            return true;
        else
            return false;

    }
}
