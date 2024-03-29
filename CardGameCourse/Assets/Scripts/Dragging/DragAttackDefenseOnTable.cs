﻿using UnityEngine;
using System.Collections;
using DG.Tweening;

public class DragAttackDefenseOnTable : DraggingActions {

    private int savedHandSlot;
    private WhereIsTheCardOrCreature whereIsCard;
    private IDHolder idScript;
    private VisualStates tempState;
    private OneCardManager manager;

    public override bool CanDrag
    {
        get
        { 
            // TEST LINE: this is just to test playing creatures before the game is complete 
            // return true;

            // TODO : include full field check
            return base.CanDrag && manager.CanBePlayedNow;
        }
    }

    void Awake()
    {
        whereIsCard = GetComponent<WhereIsTheCardOrCreature>();
        manager = GetComponent<OneCardManager>();
    }

    public override void OnStartDrag()
    {
        savedHandSlot = whereIsCard.Slot;
        tempState = whereIsCard.VisualState;
        whereIsCard.VisualState = VisualStates.Dragging;
        whereIsCard.BringToFront();
        if (playerOwner.NextAttackHidden == true && playerOwner.currentphase == "Attack")
        {
            transform.SetPositionAndRotation(transform.position, Quaternion.Euler(new Vector3(0, -180, 0)));
        }

    }

    public override void OnDraggingInUpdate()
    {

    }

    public override void OnEndDrag()
    {
        // 1) Check if we are holding a card over the table
        int tablePos = 0;
        Debug.Log("ConT: " + playerOwner.table.CreaturesOnTable.Count);
        Debug.Log("In EndDrag: " + TurnManager.Instance.whoseTurn.currentphase);

        if (DragSuccessful())
        {
            if (TurnManager.Instance.whoseTurn.currentphase != "Discard")
            {
                if (playerOwner.ProcessEventCard)
                {
                    Debug.Log("PROCESSEVENTCARD: Drag Successful");
                    playerOwner.DiscardACardFromHand(GetComponent<IDHolder>().UniqueID);
                }
                else
                {
                    if (playerOwner.table.CreaturesOnTable.Count > 1 && (playerOwner.table.CreaturesOnTable[0].ca.AttackDefense == "D" ||
                                                                        playerOwner.table.CreaturesOnTable[0].ca.AttackDefense == "G"))
                    {
                        // determine table position
                        //Debug.Log("EndDrag - If");
                        tablePos = playerOwner.PArea.tableVisual.TablePosForNewCreature(Camera.main.ScreenToWorldPoint(
                            new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z - Camera.main.transform.position.z)).x);
                        if (tablePos > 1)
                        {
                            tablePos = playerOwner.table.CreaturesOnTable.Count - 1;
                        }
                        playerOwner.PlayAttackDefenseFromHand(GetComponent<IDHolder>().UniqueID, tablePos);
                    }
                    else if (playerOwner.table.CreaturesOnTable.Count > 0)
                    {
                        // determine table position
                        //Debug.Log("EndDrag - else if");
                        tablePos = playerOwner.PArea.tableVisual.TablePosForNewCreature(Camera.main.ScreenToWorldPoint(
                        new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z - Camera.main.transform.position.z)).x);
                        // Debug.Log("Table Pos for new Creature: " + tablePos.ToString());
                        // play this card
                        if (tablePos > 0)
                        {
                            tablePos = 0;
                        }
                        Debug.Log("Table Pos: " + tablePos);
                        playerOwner.PlayAttackDefenseFromHand(GetComponent<IDHolder>().UniqueID, tablePos);
                    }
                    else
                    {
                        //Debug.Log("EndDrag - else");
                        tablePos = playerOwner.PArea.tableVisual.TablePosForNewCreature(Camera.main.ScreenToWorldPoint(
                        new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z - Camera.main.transform.position.z)).x);
                        Debug.Log("Table Pos: " + tablePos);
                        playerOwner.PlayAttackDefenseFromHand(GetComponent<IDHolder>().UniqueID, tablePos);
                    }
                }
            }
            else
            {
                Debug.Log("DISCARD: Drag Successful");
                playerOwner.DiscardACardFromHand(GetComponent<IDHolder>().UniqueID);
            }
        }
        else
        {
            Debug.Log("Drag UNSUCCESSFUL");
            OnCancelDrag();
            //// Set old sorting order 
            //whereIsCard.SetHandSortingOrder();
            //whereIsCard.VisualState = tempState;
            //// Move this card back to its slot position
            //HandVisual PlayerHand = playerOwner.PArea.handVisual;
            //Vector3 oldCardPos = PlayerHand.slots.Children[savedHandSlot].transform.localPosition;
            //transform.DOLocalMove(oldCardPos, 1f);
        }

    }

    public override void OnCancelDrag()
    {
        // Set old sorting order 
        whereIsCard.SetHandSortingOrder();
        whereIsCard.VisualState = tempState;
        // Move this card back to its slot position
        HandVisual PlayerHand = playerOwner.PArea.handVisual;
        Vector3 oldCardPos = PlayerHand.slots.Children[savedHandSlot].transform.localPosition;
        transform.DOLocalMove(oldCardPos, 1f);
    }

    protected override bool DragSuccessful()
    {

        if (TurnManager.Instance.whoseTurn.currentphase != "Discard")
        {
            if (playerOwner.ProcessEventCard)
            {
                Debug.Log("Drag Evaluate for ProcessEventCard");
                return PlayerDiscardVisual.CursorOverSomeDiscard;
            }
            else
            {
                bool TableNotFull = (playerOwner.table.CreaturesOnTable.Count < 8);
                return TableVisual.CursorOverSomeTable && TableNotFull;
            }
        }
        else
        {
            Debug.Log("YES! Over Discard Pile");
            return PlayerDiscardVisual.CursorOverSomeDiscard;
        }
        //return false;
    }

}
