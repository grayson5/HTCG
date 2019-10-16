using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideExertList : MonoBehaviour
{

    public PlayerArea PArea;

    public void HideExertPanel()
    {
        Debug.Log("Mouse Down");
        PArea.ExertPanel.SetActive(false);
    }
}
