using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShieldIconButton: MonoBehaviour {

    public void ProcessShieldButton()
    {
        // Command has some static members, so let`s make sure that there are no commands in the Queue
        Debug.Log("In ProcessShieldButton: " + TurnManager.Instance.whoseTurn.name);
        TurnManager.Instance.whoseTurn.DropGuard();
        // reset all card and creature IDs
        //IDFactory.ResetIDs();
        //IDHolder.ClearIDHoldersList();
        //Command.CommandQueue.Clear();
        //Command.CommandExecutionComplete();

    }
}
