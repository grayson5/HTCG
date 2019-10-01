using UnityEngine;
using System.Collections;

public class PlayerTurnMaker : TurnMaker 
{
    public override void OnTurnStart()
    {
       // Debug.Log("In OnTurnStart");
        base.OnTurnStart();
        //Debug.Log("After OnTurnStart");
        // dispay a message that it is player`s turn
        //Debug.Log("Before ShowMessage");
        new ShowMessageCommand(this.name + " Turn!", 2.0f).AddToQueue();
       // Debug.Log("After ShowMessage");
       // p.DrawACard();
      //  Debug.Log("After DrawACard");
        p.SweepPhase();
    }
}
