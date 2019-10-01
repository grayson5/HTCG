using UnityEngine;
using System.Collections;

public class HealCommand : Command{

    private Player target;

    public HealCommand(Player target)
    {
        this.target = target;
    }

    public override void StartCommandExecution()
    {
        target.PArea.Portrait.Heal();
    }
}
