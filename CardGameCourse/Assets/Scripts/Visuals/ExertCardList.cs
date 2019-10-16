using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExertCardList : MonoBehaviour {
    [SerializeField]
    private Image cardimage;
    public int cardID;

    public void SetImage(string cardname, PicDirectory picdir, int cid)
    {
        Debug.Log("Trying to Load: " + picdir+"/"+cardname);
        cardimage.sprite = Resources.Load<Sprite>("Card Pics/Fullpics/"+picdir+"/"+cardname);
        cardID = cid;
           
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
}
