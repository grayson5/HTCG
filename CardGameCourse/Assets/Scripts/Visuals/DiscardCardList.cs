using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiscardCardList : MonoBehaviour {
    [SerializeField]
    private Image cardimage;

    public void SetImage(string cardname, PicDirectory picdir)
    {
        Debug.Log("Trying to Load: " + picdir+"/"+cardname);
        cardimage.sprite = Resources.Load<Sprite>("Card Pics/Fullpics/"+picdir+"/"+cardname);
    
    }

    
}
