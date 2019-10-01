using UnityEngine;
using System.Collections;

public class DealDamageToPlayerAndRemoveCards : SpellEffect 
{
    

    public override void ActivateEffect(int specialAmount = 0, ICharacter target = null, Player caster = null, Player ptarget = null, string specialData = null)
    {
        Debug.Log("In DealDamageToPlayerAndRemoveCards");
        //int[] idstoremove = new int[5]; 
        //int[] idindex = new int[5]; 
        //int indexcount = 0;

        if (ptarget != null)
        {

            new DealDamageCommand(ptarget.PlayerID, specialAmount, ptarget.Health - specialAmount).AddToQueue();
            new ShowMessageCommand(specialAmount + " Damage to " + ptarget.name, 1f).AddToQueue();

            string[] cardstoremove = specialData.Split(',');
            int numofcardstoremove = cardstoremove.Length;
            Debug.Log("numofcardstoremove: " + numofcardstoremove);

            while (numofcardstoremove > 0)
            {
                for (int i = 0; i < caster.table.CreaturesOnTable.Count; i++)
                {
                    for (int x = 0; x < cardstoremove.Length; x++)
                    {
                        if (cardstoremove[x] == caster.table.CreaturesOnTable[i].ca.PlotName)
                        {
                            Debug.Log("Remove card: " + cardstoremove[x]);
                            Debug.Log("ID to Remove: " + caster.table.CreaturesOnTable[i].UniqueCreatureID);
                            Debug.Log("x: " + x + " i: " + i);

                            CardLogic newdisCard = new CardLogic(caster.table.CreaturesOnTable[i].ca);
                            newdisCard.owner = caster;
                            caster.discardcards.CardsInDiscard.Insert(0, newdisCard);
                            caster.PArea.tableVisual.RemoveCreatureWithID(caster.table.CreaturesOnTable[i].UniqueCreatureID);
                            caster.table.CreaturesOnTable.Remove(caster.table.CreaturesOnTable[i]);
                            numofcardstoremove--;
                            break;
                        }
                    }
                }
            }
        }
    }
}
