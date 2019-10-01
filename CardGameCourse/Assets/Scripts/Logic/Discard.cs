using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Discard : MonoBehaviour {

    public List<CardLogic> CardsInDiscard = new List<CardLogic>();

    void Awake()
    {
        //CardsInDiscard.Shuffle();
    }
	
}
