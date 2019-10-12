using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardListControl : MonoBehaviour {

    public Discard discards;

    [SerializeField]
    private GameObject discardcardTemplate;

    public void GenerateDiscardList()
    {
        Debug.Log("In DiscardListControl");
        Debug.Log("Discard Count: "+discards.CardsInDiscard.Count);
        Debug.Log("Child Count: "+transform.childCount);
        Transform viewparent;
        Transform Cardlist;

        viewparent = transform.GetChild(0);
        Debug.Log("viewparent: " + viewparent.name);
        Cardlist = viewparent.GetChild(0);
        Debug.Log("Cardlist: " + Cardlist.name);
        int x = 0;
        foreach (Transform child in Cardlist)
        {
            Debug.Log("In Foreach");
            //if (child.name == "DiscardImage(Clone)")
            if (x > 0 )
                Destroy(child.gameObject);
            Debug.Log("Tag: " + child.tag+" name: "+child.name+".");
            x++;
        }

        for(int i=0; i< discards.CardsInDiscard.Count; i++  )
        {
            Debug.Log("In For for Show Discard");
            GameObject discardcard = Instantiate(discardcardTemplate) as GameObject;
            discardcard.SetActive(true);          
            discardcard.GetComponent<DiscardCardList>().SetImage(discards.CardsInDiscard[i].ca.name, discards.CardsInDiscard[i].ca.PicDir);
            discardcard.transform.SetParent(discardcardTemplate.transform.parent, false);
        }
    }

}
