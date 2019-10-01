using UnityEngine;
using System.Collections;

public class CheckForSpecificPlots : ConditionEffect
{
    

    public override bool CheckCondition (int specialAmount = 0, ICharacter target = null, Player caster = null, Player ptarget = null, string specialName = null, string cardname = null)
    {
        bool plotsexists = false;
        bool plotplayed = false;
        Debug.Log("In CheckForSpecificPlots for card: " + cardname);
        int plotstobefound;
        string[] plotnames = null;

        //Check to see if a copy of plot card is in play already; if so it can't be played.
        if (cardname != "PlotEvent")
        {
            for (int i = 0; i < caster.table.CreaturesOnTable.Count; i++)
            {
                if (cardname == caster.table.CreaturesOnTable[i].ca.PlotName)
                {
                    plotplayed = true;
                    break;
                }
            }
        }

        // if plot card not already in play then proceed
        if (plotplayed == false)
        {
            // if first plot of series than prereqs = "None" so go to if and skip check for prereqs in else
            if (specialName == "None")
            {
                plotstobefound = 0;
                plotsexists = true;
            }
            else //2nd or 3rd card of plot series so check for prereqs
            {
                plotnames = specialName.Split(',');
                plotstobefound = plotnames.Length;

                Debug.Log("Need to find " + plotstobefound + " plot cards");
                for (int i = 0; i < plotstobefound; i++)
                {
                    Debug.Log("i: " + i + " plotname to be found: " + plotnames[i]);
                }

                for (int i = 0; i < caster.table.CreaturesOnTable.Count; i++)
                {
                    for (int x = 0; x <= (plotnames.Length - 1); x++)
                    {
                        Debug.Log("Checking Plotname against Creature on table");
                        Debug.Log("i: " + i + " x:" + x);
                        Debug.Log(caster.table.CreaturesOnTable[i].ca.PlotName + " = " + plotnames[x]);
                        if (caster.table.CreaturesOnTable[i].ca.PlotName == plotnames[x])
                        {
                            plotstobefound--;
                            break;
                        }
                    }
                }

                if (plotstobefound == 0)
                    plotsexists = true;
            }
        }
        else
        {
            Debug.Log("Copy of plot card already in play");
            plotsexists = false;
        }



        if (plotsexists)
            return true;
        else
            return false;

    }
}
