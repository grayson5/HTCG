using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideDiscardDeck : MonoBehaviour
{

    public PlayerArea PArea;

    void OnMouseExit()
    {

        Debug.Log("Mouse Down");
        PArea.DiscardPanel.SetActive(false);
    }
}
