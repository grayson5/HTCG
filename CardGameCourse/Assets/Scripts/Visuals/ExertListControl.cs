using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExertListControl : MonoBehaviour {

    //public Discard discards;

    [SerializeField]
    private GameObject exertcardTemplate;

    public void AddExertCardToList(int UniqueID, CardLogic cl, string exerttype)
    {
        Debug.Log("In AddExertCardToList");
        GameObject exertcard = Instantiate(exertcardTemplate) as GameObject;
        exertcard.SetActive(true);          
        exertcard.GetComponent<ExertCardList>().SetImage(cl.ca.name, cl.ca.PicDir, UniqueID, cl);
        exertcard.transform.SetParent(exertcardTemplate.transform.parent, false);
        Debug.Log("AttDef: " + cl.ca.AttackDefense);

        if (exerttype == "Def")
        {
            if (cl.ca.AttackDefense == "S" || cl.ca.AttackDefense == "A")
            {
                Debug.Log("AttDef 2: " + cl.ca.AttackDefense);
                Debug.Log("Name: " + cl.ca.name);
                exertcard.GetComponent<Button>().interactable = false;
            }
        }

        if (exerttype == "Att")
        {
            if (cl.ca.AttackDefense == "S" || cl.ca.AttackDefense == "D" || cl.ca.AttackDefense == "G")
            {
                Debug.Log("AttDef 2: " + cl.ca.AttackDefense);
                Debug.Log("Name: " + cl.ca.name);
                exertcard.GetComponent<Button>().interactable = false;
            }
        }

        Command.CommandExecutionComplete();

    }

    public void AddExertCardToTable()
    {
        Transform viewparent;
        Transform Cardlist;

        viewparent = transform.GetChild(0);
        Debug.Log("viewparent: " + viewparent.name);
        Cardlist = viewparent.GetChild(0);
        Debug.Log("Cardlist: " + Cardlist.name);

        foreach (Transform child in Cardlist)
        {
            Debug.Log("In Foreach - Add");
            Debug.Log("Selected Card: " + child.name);
            if (child.GetComponentInChildren<Text>().fontSize == 128)
            {
                Debug.Log("Selected Card: " + child.name);
                child.GetComponent<ExertCardList>().AddCardToTable();
            }
            else
            {
                child.GetComponent<ExertCardList>().AddCardToDiscard();
            }

            if(child.GetComponent<ExertCardList>().cardID != 0)
                Destroy(child.gameObject);

        }
    }

}
