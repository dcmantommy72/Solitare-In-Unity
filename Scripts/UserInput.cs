using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class UserInput : MonoBehaviour
{
    
    public GameObject slot1;
    private Solitare solitare;
    private float timer;
    private float doubleClickTime = 0.3f;
    private int clickCount = 0;

    //Left Mouse Drag
    //private bool isBeingHeld = false;
    public GameObject joker;
    private float startPosX;
    private float startPosY;



    // Start is called before the first frame update
    void Start()
    {
        solitare = FindObjectOfType<Solitare>();
        slot1 = this.gameObject;
        
    }

    // Update is called once per frame
    void Update()
    {

        if (clickCount == 1)
        {
            timer += Time.deltaTime;
        }
        if (clickCount == 3)
        {
            timer = 0;
            clickCount = 1;
        }
        if (timer > doubleClickTime)
        {
            timer = 0;
            clickCount = 0;
        }

        GetMouseClick();
    }


    void GetMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            clickCount++;
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -10));
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit)
            {
                // What has been hit? Deck/Card/EmptySlot...ect
                if (hit.collider.CompareTag("Deck"))
                {
                    // We Clicked On The Deck
                    Deck();
                }
                else if (hit.collider.CompareTag("Card"))
                {
                    // We Clicked Card
                    Card(hit.collider.gameObject);
                }
                else if (hit.collider.CompareTag("Top"))
                {
                    // We Clicked Top Spot
                    Top(hit.collider.gameObject);
                }
                else if (hit.collider.CompareTag("Bottom"))
                {
                    // We Clicked On Bottom Spot
                    Bottom(hit.collider.gameObject);
                }
            }

        }
        if (Input.GetMouseButtonDown(1)) // Left Mouse Button Clicked - Menu
        {
            print("left mouse button clicked");
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -10));
            

        }
        if (Input.GetMouseButtonUp(1)) // Left Mouse Button Released
        {
            print("left mouse button was released");
            // isBeingHeld = false;
            // on mouse release if card is not in a stackable spot, send it back to its starting point
            // else stack card
        }
        // if (isBeingHeld == true)
        // {
        //     // Vector3 leftMousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -10));
        //     // joker.transform.localPosition = new Vector3(leftMousePos.x, leftMousePos.y, 0);
        //     // //startPosX = leftMousePos.x - joker.transform.localPosition.x;
        //     // //startPosY = leftMousePos.y - joker.transform.localPosition.y;
        // }
    }


    //Methods For Each Object Clicked

    void Deck()
    {
        // Deck Clicked Actions
        print("Clicked Deck");
        solitare.DealFromDeck();
        slot1 = this.gameObject;
        // CARDS DELT. RESHUFFLED WHEN EMPTY
        if (solitare.deckLocation == 0)
        {
            if (solitare.threeCardDeal)
            {
                Scoring.scoreValue -= 20; // Three Card Deal -20 Per REshuffle
            }
            else
            {
                Scoring.scoreValue -= 100; // One Card Deal -100 Per Reshuffle
            }
        }
        // if (!solitare.threeCardDeal && solitare.deckLocation == 0) //If ONE CARD REshuffle Deck
        // {
        //     print("DECK IS EMPTY----RESHUFFLE");
        //     Scoring.scoreValue -=100; // -100 Per RE-shuffle
        // }
        // if (solitare.threeCardDeal && solitare.deckLocation == 0)
        // {
        //     Scoring.scoreValue -= 20; // -20 Per RE-Shuffle
        // }
        
    }

    void Card(GameObject selected)
    {
        // When A Card Is Clicked
        print("Clicked On Card");

        if (!selected.GetComponent<Selectable>().faceUp) // if the card clicked on is facedown
        {
            if (!Blocked(selected)) // if the card clicked on is not blocked
            {
                // flip it over
                selected.GetComponent<Selectable>().faceUp = true;
                // REVEAL CARD SCORE +5
                Scoring.scoreValue += 5;
                slot1 = this.gameObject;
            }

        }
        else if (selected.GetComponent<Selectable>().inDeckPile) // if card is in decktrips
        {
            // if it is not blocked
            if (!Blocked(selected))
            {
                if (slot1 == selected) // IF the same card has been clicked twice
                {
                    if (DoubleClick())
                    {
                        // attempt the auto stack
                        AutoStack(selected);
                    }
                }
                else
                {
                    slot1 = selected;
                }

                //slot1 = selected;
            }
        }
        else
        {
            // if the card is face up
            // if there is no card currently selected
            // select the card

            if (slot1 == this.gameObject) // not null we pass in this gameObject instead
            {
                slot1 = selected;
            }

            // if there is already a card selected (and it is not the same card)
            else if (slot1 != selected)
            {
                // if the new card is eligable to stack on the old card
                if (Stackable(selected))
                {
                    Stack(selected);
                    // SCORE UPDATE... DECK TO ROW? +5
                    //Scoring.scoreValue += 5;

                }
                else
                {
                    // select the new card
                    slot1 = selected;
                }
            }

            else if (slot1 == selected) // same cards been selected twice
            {
                if (DoubleClick())
                {
                    // then attempt auto stack
                    AutoStack(selected);
                }
            }
        }
        
    }

    void Top(GameObject selected)
    {
        // Top Spot Clicked Actions
        print("Clicked Top Spot");
        if (slot1.CompareTag("Card"))
        {
            // if the card is an ace and the empty slot is top then stack!
            if (slot1.GetComponent<Selectable>().value == 1) // value 1 = ace
            {
                Stack(selected);
            }
        }
    }

    void Bottom(GameObject selected)
    {
        // Bottom Spot Clicked Actions
        print("Clicked Bottom Spot");
        // If the card is a king and the empty slot is a bottom then stack
        if (slot1.CompareTag("Card"))
        {
            if (slot1.GetComponent<Selectable>().value == 13)
            {
                Stack(selected);
            }
        }

    }



    





    // Method to check if card is stackable, returns true if it can stack, false if not
    bool Stackable(GameObject selected)
    {
        Selectable s1 = slot1.GetComponent<Selectable>(); //Selectable script from the slot1 gameObject (Card)
        Selectable s2 = selected.GetComponent<Selectable>(); //Selectable script from the recently selected gameobject (Card)
        //Compare these two variables to determine if they can stack on one another
        if (!s2.inDeckPile) // IF s2 is NOT in top deck pile, then go ahead
        {
            // If in top
            if (s2.top) //recently selected card is in the top row, must stack suited A to K (10 Points Each)
            {
                if (s1.suit == s2.suit || (s1.value == 1 && s2.suit == null)) //if same suit, OR, s1 is an Ace (value of 1) AND s2 has no suit (empty spot)
                {
                    if (s1.value == s2.value + 1) // if s1 value is 1 LESS than s2, then its next in line, (Ace=1 < Empty0) so Stackable is true
                    {
                        
                        if (Solitare.vegasScoring)
                        {
                            Scoring.scoreValue += 5; // ADD 5$
                            return true;
                        }
                        else
                        {
                            Scoring.scoreValue += 10; //ADD 10 POINTS
                            return true;
                        }  
                        // Scoring.scoreValue += 10; //ADD 10 POINTS
                        // return true;
                    }
                }
                else
                {
                    return false; // Not stackable
                }
            }
            else // recent selected card is in bottom, so must stack alternate colors K to A
            {
                if (s1.value == s2.value -1) // If cards value comes next...
                {
                    bool card1Red = true;
                    bool card2Red = true;

                    if (s1.suit == "C" || s1.suit == "S")
                    {
                        card1Red = false;
                    }
                    if (s2.suit == "C" || s2.suit == "S")
                    {
                        card2Red = false;
                    }

                    if (card1Red == card2Red) // If cards are same color
                    {
                        print("Not Stackable Same Color");
                        return false;
                    }
                    else
                    {
                        print("Stackable");
                        // FOR SCORING ONLY
                        if (s1.top)
                        {
                            // Top To Bottom Move
                            Scoring.scoreValue -= 15;
                        }
                        else if (s1.inDeckPile)
                        {
                            // Deck To Bottom Move
                            Scoring.scoreValue += 5;
                        }
                        // POINTS UPDATE FOR BOTTOM ROW CARD STACKED +5 POINTS
                        //Scoring.scoreValue += 5; //ADD 5 POINTS
                        return true;
                    }
                }
            }
        }
        // If in the bottom pile, must stack alternate colors King to Ace
        return false;
    }


    void Stack(GameObject selected)
    {
        // if on top of king or empty bottom stack the cards in place
        // else stack the cards with a negative y offset

        Selectable s1 = slot1.GetComponent<Selectable>();
        Selectable s2 = selected.GetComponent<Selectable>();
        float yOffset = 0.3f;

        if (s2.top || (!s2.top && s1.value == 13))
        {
            yOffset = 0;
        }

        slot1.transform.position = new Vector3(selected.transform.position.x, selected.transform.position.y - yOffset, selected.transform.position.z - 0.01f);
        slot1.transform.parent = selected.transform; // this makes the children move with the parents

        if (s1.inDeckPile) // removes the cards from the top pile to prevent duplicate cards
        {
            solitare.tripsOnDisplay.Remove(slot1.name);
        }
        else if (s1.top && s2.top && s1.value == 1) // allows movement of cards between top spots
        {
            solitare.topPos[s1.row].GetComponent<Selectable>().value = 0;
            solitare.topPos[s1.row].GetComponent<Selectable>().suit = null;
        }
        else if (s1.top) // keeps track of the current value of the top decks as a card has been removed
        {
            solitare.topPos[s1.row].GetComponent<Selectable>().value = s1.value - 1;
        }
        else // removes the card string from the appropriate bottom list
        {
            solitare.bottoms[s1.row].Remove(slot1.name);
        }

        s1.inDeckPile = false; // you cannot add cards to the trips pile so this is always fine
        s1.row = s2.row;

        if (s2.top) // moves a card to the top and assigns the top's value and suit
        {
            solitare.topPos[s1.row].GetComponent<Selectable>().value = s1.value;
            solitare.topPos[s1.row].GetComponent<Selectable>().suit = s1.suit;
            s1.top = true;
        }
        else
        {
            s1.top = false;
        }

        // after completing move reset slot1 to be essentially null as being null will break the logic
        slot1 = this.gameObject;

    }



    // Method To CHECK if Card is BLOCKED or not
    bool Blocked(GameObject selected)
    {
        Selectable s2 = selected.GetComponent<Selectable>();
        if (s2.inDeckPile == true)
        {
            if (s2.name == solitare.tripsOnDisplay.Last()) // if it is the last trip, it is not blocked
            {
                return false;
            }
            else
            {
                print(s2.name + " is blocked by " + solitare.tripsOnDisplay.Last());
                return true;
            }
        }
        else
        {
            if (s2.name == solitare.bottoms[s2.row].Last()) // check if it is the bottom card
            {
                return false;
            }
            else
            {
                print(s2.name + " is blocked by " + solitare.bottoms[s2.row].Last());
                return true;
            }
        }
    }


    // fix to reference error Sets slot1 properly
    public void SetSlot()
    {
        slot1 = this.gameObject;
    }



    // Setting a double click method
    bool DoubleClick()
    {
        if (timer < doubleClickTime && clickCount == 2)
        {
            print("Double Click");
            return true;
        }
        else
        {
            return false;
        }
    }

    // AUTO-STACK
    void AutoStack(GameObject selected)
    {
        for (int i = 0; i < solitare.topPos.Length; i++)
        {
            Selectable stack = solitare.topPos[i].GetComponent<Selectable>();
            if (selected.GetComponent<Selectable>().value == 1) // If its an Ace
            {
                if (solitare.topPos[i].GetComponent<Selectable>().value == 0) // and topPos is empty
                {
                    slot1 = selected;
                    Stack(stack.gameObject); // stack the ace up top, in the first empty pos found
                    // GIVE POINTS? NOOOO
                    // if (Solitare.vegasScoring)
                    // {
                    //     Scoring.scoreValue += 5; // Add 5$ To Vegas Score
                    // }
                    // else
                    // {
                    //     Scoring.scoreValue += 10; // Add 10 to Standard Score
                    // }
                    break;
                }
            }
            else
            {
                if ((solitare.topPos[i].GetComponent<Selectable>().suit == slot1.GetComponent<Selectable>().suit) && (solitare.topPos[i].GetComponent<Selectable>().value == slot1.GetComponent<Selectable>().value -1))
                {
                    // if it is the last card and has NO children
                    if (HasNoChildren(slot1))
                    {
                        slot1 = selected;

                        // find a top spot that matches the conditions for auto stacking, if exists
                        string lastCardname = stack.suit + stack.value.ToString();
                        if (stack.value == 1)
                        {
                            lastCardname = stack.suit + "A";
                        }
                        if (stack.value == 11)
                        {
                            lastCardname = stack.suit + "J";
                        }
                        if (stack.value == 12)
                        {
                            lastCardname = stack.suit + "Q";
                        }
                        if (stack.value == 13)
                        {
                            lastCardname = stack.suit + "K";
                        }
                        GameObject lastCard = GameObject.Find(lastCardname);
                        Stack(lastCard);
                        if (Solitare.vegasScoring)
                        {
                            Scoring.scoreValue += 5;
                        }
                        else
                        {
                            Scoring.scoreValue += 10;
                        }
                        break;
                    }
                }
            }
        }
    }


    bool HasNoChildren(GameObject card)
    {
        int i = 0;
        foreach (Transform child in card.transform)
        {
            i++;
        }
        if (i == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }



    // CHECK FOR POSSIBLE MOVES
        // True = Moves Available
    public bool CheckForMoves()
    {
        //GameObject ckCard = GameObject.Find(card);
        // iterate thru deck CHECK DECK CARDS
        foreach (string card in solitare.deck)
        {
            GameObject ckCard = GameObject.Find(card);
            //Selectable s1 = ckCard.GetComponent<Selectable>();
            // no actually im setting ckCard to slot1...
            slot1 = ckCard;

            //CHECK DECK IF STACKABLE AT BOTTOM

            if ( !checkBottoms() && !checkTops() && !checkBottomsToTop() )
            {
                // Check Bottoms TO Tops
                print("NO MOVES LEFT TO MAKE YOU LOOSE");
                // Set every bottom card to slot1 and check if stackable to tops
                return false;
            }
        }
        slot1 = this.gameObject;
        return true;
    }




    bool checkTops()
    {
        
        for (var i = 0; i < solitare.topPos.Length; i++)
        {
            //Selectable lastcard = solitare.topPos[i].GetComponent<Selectable>();
            if (Stackable(solitare.topPos[i]))
            {
                return true;
            }
           
        }
        return false;
    }

    bool checkBottoms()
    {
        //GameObject bot = GameObject.Find(solitare.bottoms[0][0]);
        //for (var i = 0; i < solitare.bottoms)
        for (var i = 0; i < solitare.bottoms.Count(); i++)
        {
            //string lcard = solitare.bottoms[i].Last();
            GameObject bot = GameObject.Find(solitare.bottoms[i].Last());
            if (Stackable(bot))
            {
                return true;
            }
        }
        return false;
    }

    bool checkBottomsToTop()
    {
        for (var i = 0; i < solitare.bottoms.Count(); i++)
        {
            //string lcard = solitare.bottoms[i].Last();
            GameObject bot = GameObject.Find(solitare.bottoms[i].Last());
            slot1 = bot;
            if (checkTops())
            {
                return true;
            }
        }
        return false;
    }


    // Returns the first up card in a row...?
    GameObject lastUpCard()
    {
        for (var i = 0; i < solitare.bottoms.Count(); i++)
        {
            for (var k = 0; k < solitare.bottoms[i].Count(); k++)
            {
                GameObject bot = GameObject.Find(solitare.bottoms[i][k]);
                GameObject bot2 = GameObject.Find(solitare.bottoms[i][k - 1]);

                if (k >= 0 && bot.GetComponent<Selectable>().faceUp && !bot2.GetComponent<Selectable>().faceUp)
                {
                    //return bot.GetComponent<Selectable>();
                    //return GameObject bot;
                    //break;
                    //return i;
                    return bot;
                
                }
            }
            
        }
        return null;
    }


    bool checkLastUpCard()
    {
        if (Stackable( lastUpCard() ) )
        {
            return true;
        }
        return false;
    }






}
