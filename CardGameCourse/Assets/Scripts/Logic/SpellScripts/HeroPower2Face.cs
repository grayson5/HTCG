using UnityEngine;
using System.Collections;

public class HeroPower2Face : SpellEffect 
{

    public override void ActivateEffect(int specialAmount = 0, ICharacter target = null, Player caster = null, Player ptarget = null, string specialData = null)
    {
        new DealDamageCommand(TurnManager.Instance.whoseTurn.otherPlayer.PlayerID, 2, TurnManager.Instance.whoseTurn.otherPlayer.Health - 2).AddToQueue();
        TurnManager.Instance.whoseTurn.otherPlayer.Health -= 2;
    }
}
