using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExertCardList : MonoBehaviour {
    [SerializeField]
    private Image cardimage;
    public int cardID;
    //public CardAsset ca;
    public Player p;
    public Discard discarddeck;
    public CardLogic cl;

    public void SetImage(string cardname, PicDirectory picdir, int cid, CardLogic si_cl)
    {
        Debug.Log("Trying to Load: " + picdir+"/"+cardname);
        cardimage.sprite = Resources.Load<Sprite>("Card Pics/Fullpics/"+picdir+"/"+cardname);
        cardID = cid;
        //ca = si_ca;
        cl = si_cl;
    }

    public void SelectCard()
    {
        Debug.Log("In SelectCard");

        Transform viewparent;

        viewparent = transform.parent;
        Debug.Log("viewparent: " + viewparent.name);
        Button button;
        button = GetComponent<Button>();

        if (button.GetComponentInChildren<Text>().fontSize == 0)
        {
            foreach (Transform child in viewparent)
            {
                Debug.Log("In Foreach");
                Button button1;
                button1 = child.GetComponent<Button>();
                button1.GetComponentInChildren<Text>().fontSize = 0;
            }
            button.GetComponentInChildren<Text>().fontSize = 128;
        }
        else
            button.GetComponentInChildren<Text>().fontSize = 0;

        

        //if (button.GetComponentInChildren<Text>().fontSize == 0)
        //    button.GetComponentInChildren<Text>().fontSize = 128;
        //else
        //    button.GetComponentInChildren<Text>().fontSize = 0;

    }

    public void AddCardToTable()
    {
        Debug.Log("In AddExertCardToTable");
        Debug.Log("Name: " + cl.ca.name);
        Debug.Log("Position: " + p.table.CreaturesOnTable.Count);
        //Get table position and add selected card to table
        p.PlayAttackDefenseFromHand(cardID, p.table.CreaturesOnTable.Count);

    }

    public void AddCardToDiscard()
    {
        if (cardID != 0)
            discarddeck.CardsInDiscard.Insert(0, cl);
    }
}
