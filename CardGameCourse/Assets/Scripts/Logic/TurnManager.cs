using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

// this class will take care of switching turns and counting down time until the turn expires
public class TurnManager : MonoBehaviour {

    // PUBLIC FIELDS
    public CardAsset CoinCard;
    public Dice GameDie1;
    public int gamedie1result;

    // for Singleton Pattern
    public static TurnManager Instance;

    // PRIVATE FIELDS
    // reference to a timer to measure 
    private RopeTimer timer;

    // PROPERTIES
    private Player _whoseTurn;
    public Player whoseTurn
    {
        get
        {
            return _whoseTurn;
        }

        set
        {
            _whoseTurn = value;
            timer.StartTimer();

            GlobalSettings.Instance.EnableEndTurnButtonOnStart(_whoseTurn);

            TurnMaker tm = whoseTurn.GetComponent<TurnMaker>();
            // player`s method OnTurnStart() will be called in tm.OnTurnStart();
            tm.OnTurnStart();
            if (tm is PlayerTurnMaker)
            {
                whoseTurn.HighlightPlayableCards();
            }
            // remove highlights for opponent.
            whoseTurn.otherPlayer.HighlightPlayableCards(true);
                
        }
    }


    // METHODS
    void Awake()
    {
        Instance = this;
        timer = GetComponent<RopeTimer>();
    }

    void Start()
    {
       OnGameStart();
    }

    public void OnGameStart()
    {
        Debug.Log("In TurnManager.OnGameStart()");

        GameObject.FindWithTag("Dice1").GetComponent<SpriteRenderer>().enabled = false;
        GameObject.FindWithTag("Dice1").GetComponent<BoxCollider2D>().enabled = false;

        CardLogic.CardsCreatedThisGame.Clear();
        CreatureLogic.CreaturesCreatedThisGame.Clear();

        foreach (Player p in Player.Players)
        {
            p.MustOrMayAction = false;
            p.LoadCharacterInfoFromAsset();
            p.TransmitInfoAboutPlayerToVisual();
            p.PArea.PDeck.CardsInDeck = p.deck.cards.Count;
            p.PArea.HiddenIconButton1.image.enabled = false;
            p.PArea.ShieldIcon.image.enabled = false;
            p.PArea.PAbilityIcon.image.enabled = false;
            p.PArea.DisarmIcon.image.enabled = false;
            if (p.charAsset.PersonaAblConditionChk != null && p.charAsset.PersonaAblConditionChk != "")
            {
                //Debug.Log("SpellScript: " + ca.SpecialScriptName);
                p.personacondcheck = System.Activator.CreateInstance(System.Type.GetType(p.charAsset.PersonaAblConditionChk)) as PersonaConditionEffect;
            }
            // move both portraits to the center
            //TODO: Maybe move Persona's later p.PArea.Portrait.transform.position = p.PArea.InitialPortraitPosition.position;
        }

        //Sequence s = DOTween.Sequence();
        //s.Append(Player.Players[0].PArea.Portrait.transform.DOMove(Player.Players[0].PArea.PortraitPosition.position, 1f).SetEase(Ease.InQuad));
        //s.Insert(0f, Player.Players[1].PArea.Portrait.transform.DOMove(Player.Players[1].PArea.PortraitPosition.position, 1f).SetEase(Ease.InQuad));
        //s.PrependInterval(3f);
        //s.OnComplete(() =>
        //    {
        // determine who starts the game.
        //int rnd = Random.Range(0, 2);  // 2 is exclusive boundary
        //                               // Debug.Log(Player.Players.Length);
        //Player whoGoesFirst = Player.Players[rnd];
        //// Debug.Log(whoGoesFirst);
        //Player whoGoesSecond = whoGoesFirst.otherPlayer;
        // Debug.Log(whoGoesSecond);

        //        // draw 4 cards for first player and 5 for second player
        //        int initDraw = 4;
        //        for (int i = 0; i < initDraw; i++)
        //        {            
        //            // second player draws a card
        //            whoGoesSecond.DrawACard(true);
        //            // first player draws a card
        //            whoGoesFirst.DrawACard(true);
        //        }
        //        // add one more card to second player`s hand
        //        whoGoesSecond.DrawACard(true);
        //        //new GivePlayerACoinCommand(null, whoGoesSecond).AddToQueue();
        //        whoGoesSecond.GetACardNotFromDeck(CoinCard);
        //        new StartATurnCommand(whoGoesFirst).AddToQueue();
        //    });


        // determine who starts the game.
        //int rnd = Random.Range(0, 2);  // 2 is exclusive boundary
        // Debug.Log(Player.Players.Length);
        int rnd = 1;
        Player whoGoesFirst = Player.Players[rnd];
        // Debug.Log(whoGoesFirst);
        Player whoGoesSecond = whoGoesFirst.otherPlayer;
        // Debug.Log(whoGoesSecond);

        // draw cards up to each player's ability

        while(whoGoesFirst.hand.CardsInHand.Count < whoGoesFirst.Health || whoGoesSecond.hand.CardsInHand.Count < whoGoesSecond.Health)
        {
            if (whoGoesFirst.hand.CardsInHand.Count < whoGoesFirst.Health)
            {
                whoGoesFirst.DrawACard(true);
            }

            if (whoGoesSecond.hand.CardsInHand.Count < whoGoesSecond.Health)
            {
                whoGoesSecond.DrawACard(true);
            }
        }

        new StartATurnCommand(whoGoesFirst).AddToQueue();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            EndTurn();
    }

    // FOR TEST PURPOSES ONLY
    public void EndTurnTest()
    {
        timer.StopTimer();
        timer.StartTimer();
    }

    public void EndTurn()
    {
            // stop timer
            timer.StopTimer();
            // send all commands in the end of current player`s turn
            whoseTurn.OnTurnEnd();
            Debug.Log("Current Player: " + whoseTurn.PlayerID);
            Debug.Log("Next Player: " + whoseTurn.otherPlayer.PlayerID);

            
            new StartATurnCommand(whoseTurn.otherPlayer).AddToQueue();
    }

    public void ProcessButton ()
    {
        Debug.Log("Current Phase: " + whoseTurn.currentphase);

        if (whoseTurn.currentphase == "MayMust" || whoseTurn.ProcessEventCard == true)
        {
            whoseTurn.CheckMustOrMayAction();
            return;
        }

        if (whoseTurn.currentphase == "Defense"  && whoseTurn.ProcessEventCard == false)
        {
            whoseTurn.ProcessDefenses();
            GameObject.FindWithTag("ButtonText").GetComponent<Text>().text = "End Attack Phase";
            GameObject.FindWithTag("PowerButton").GetComponent<Text>().text = "Exert for Attack";
            whoseTurn.currentphase = "Attack";
            whoseTurn.HighlightPlayableCards();
           // Debug.Log("trying to set PB button active");
            return;
        }

        if (whoseTurn.currentphase == "Attack" && whoseTurn.ProcessEventCard == false)
        {
            Debug.Log("In TurnManager After Attack End");
            whoseTurn.currentphase = "Ability Adjustment Phase";
            whoseTurn.AbilityAdjustment();
            GameObject.FindWithTag("ButtonText").GetComponent<Text>().text = "End Discard Phase";
            whoseTurn.currentphase = "Discard";
            //-OLD whoseTurn.currentphase = "End Phase";
        }

        if (whoseTurn.currentphase == "Discard")
        {
            if (whoseTurn.Health < whoseTurn.hand.CardsInHand.Count)
            {
                Debug.Log("Must Dicard: " + (whoseTurn.hand.CardsInHand.Count - whoseTurn.Health) + " cards");
                whoseTurn.HighlightPlayableCards();
                new ShowMessageCommand("You must discard " + (whoseTurn.hand.CardsInHand.Count - whoseTurn.Health) + " cards", 2f).AddToQueue();
                //NEXT LINE IS TEMP
                Debug.Log("After message in Discard");
                //whoseTurn.currentphase = "End Phase";

                if (whoseTurn.hand.CardsInHand.Count <= whoseTurn.Health)
                {
                    whoseTurn.currentphase = "End Phase";
                    EndTurn();
                }
            }
            else
            {
                Debug.Log("No need to Discard");
                whoseTurn.currentphase = "End Phase";
            }

            //if (whoseTurn.hand.CardsInHand.Count <= whoseTurn.Health)
            //{
            //    whoseTurn.currentphase = "End Phase";
            //    EndTurn();
            //}
        }

        if (whoseTurn.currentphase == "End Phase")
        {
            EndTurn();
        }

    }

    public void ProcessPowerBloworBlock()
    {
        Debug.Log("In ProcessPowerBloworBlock");

        whoseTurn.ProcessPowerButton();
        //Debug.Log("C: " + whoseTurn.table.CreaturesOnTable.Count);
    }

    public void StopTheTimer()
    {
        timer.StopTimer();
    }

}

