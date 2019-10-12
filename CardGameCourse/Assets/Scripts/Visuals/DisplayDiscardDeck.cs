using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayDiscardDeck : MonoBehaviour
{

    public PlayerArea PArea;

    void OnMouseDown()
    {
        PArea.DiscardPanel.SetActive(true);
        PArea.DiscardPanel.GetComponentInChildren<DiscardListControl>().GenerateDiscardList();
    }
}
