using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExertListControl : MonoBehaviour {

    //public Discard discards;

    [SerializeField]
    private GameObject exertcardTemplate;

    public void AddExertCardToList(CardAsset c, int UniqueID)
    {
        Debug.Log("In AddExertCardToList");
        GameObject exertcard = Instantiate(exertcardTemplate) as GameObject;
        exertcard.SetActive(true);          
        exertcard.GetComponent<ExertCardList>().SetImage(c.name, c.PicDir, UniqueID);
        exertcard.transform.SetParent(exertcardTemplate.transform.parent, false);
        Debug.Log("AttDef: " + c.AttackDefense);

        if (c.AttackDefense == "S" || c.AttackDefense == "A")
        {
            Debug.Log("AttDef 2: " + c.AttackDefense);
            Debug.Log("Name: " + c.name);
            exertcard.GetComponent<Button>().interactable = false;
        }
        
        Command.CommandExecutionComplete();

    }

}
