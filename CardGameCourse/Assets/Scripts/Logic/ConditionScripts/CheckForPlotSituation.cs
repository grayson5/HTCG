using UnityEngine;
using System.Collections;

public class CheckForPlotSituation : ConditionEffect
{
    

    public override bool CheckCondition (int specialAmount = 0, ICharacter target = null, Player caster = null, Player ptarget = null, string specialName = null, string cardname = null)
    {
        bool plotexists = false;
        Debug.Log("In CheckForPlotSituation");

        if (caster != null)
        {
            for (int i = 0; i < caster.table.CreaturesOnTable.Count; i++)
            {
                if (caster.table.CreaturesOnTable[i].ca.PlotName != null && caster.table.CreaturesOnTable[i].ca.PlotName != "")
                {
                    plotexists = true;
                    break;
                }
            }
        }

        if (plotexists == false && ptarget != null)
        {
            for (int i = 0; i < ptarget.table.CreaturesOnTable.Count; i++)
            {
                if (ptarget.table.CreaturesOnTable[i].ca.PlotName != null && ptarget.table.CreaturesOnTable[i].ca.PlotName != "")
                {
                    plotexists = true;
                    break;
                }
            }
        }

        if (plotexists)
            return true;
        else
            return false;

    }
}
