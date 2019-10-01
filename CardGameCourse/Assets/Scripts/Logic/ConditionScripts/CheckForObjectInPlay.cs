using UnityEngine;
using System.Collections;

public class CheckForObjectInPlay : ConditionEffect
{
    

    public override bool CheckCondition (int specialAmount = 0, ICharacter target = null, Player caster = null, Player ptarget = null, string specialName = null, string cardname = null)
    {
        bool objectexists = false;

        Debug.Log("In CheckForObjectInPlay");

        //Check to see if a plot card is in play; if so then can play card

        for (int i = 0; i < caster.table.CreaturesOnTable.Count; i++)
        {
            if (caster.table.CreaturesOnTable[i].ca.TypeOfSpecial == SpecialType.Object)
            {
                objectexists = true;
                break;
            }
        }

        if (objectexists == false)
        {
            for (int i = 0; i < ptarget.table.CreaturesOnTable.Count; i++)
            {
                if (ptarget.table.CreaturesOnTable[i].ca.TypeOfSpecial == SpecialType.Object)
                {
                    objectexists = true;
                    break;
                }
            }
        }

        if (objectexists)
            return true;
        else
            return false;

    }
}
