using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Solitare : MonoBehaviour
{

    // call userInput
    private UserInput userInput;
    // Card Face Array
    public Sprite[] cardFaces;
    public GameObject cardPrefab;
    public GameObject DeckTop;

    // Managing card position arrays
    public GameObject[] bottomPos;
    public GameObject[] topPos;

    // Static Array of Suits and Values
    public static string[] suits = new string[] { "C", "D", "H", "S" };
    public static string[] values = new string[] { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
    public List<string>[] bottoms;
    public List<string>[] tops;

    public List<string> tripsOnDisplay = new List<string>(); // List of 3 cards
    public List<List<string>> deckTrips = new List<List<string>>(); // List of the List-groups of three cards


    // String Lists For Bottom Row
    private List<string> bottom0 = new List<string>();
    private List<string> bottom1 = new List<string>();
    private List<string> bottom2 = new List<string>();
    private List<string> bottom3 = new List<string>();
    private List<string> bottom4 = new List<string>();
    private List<string> bottom5 = new List<string>();
    private List<string> bottom6 = new List<string>();

    public List<string> deck;
    public List<string> discardPile = new List<string>();
    public int deckLocation; // set to public
    private int trips;
    private int tripsRemainder;

    //ONE CARD DEAL
    public bool threeCardDeal = true; // if false, 1 card deal
    private float zOff = 0.2f;
    public string cardOnDisplay;
    //public List<string> deckSingles;
    private int single;
    private int singleRemainder;
    private int reShuffleCounter;
    public List<string> singleOnDisplay = new List<string>(); // List of up card (could be made just a string)
    public List<List<string>> deckSingles = new List<List<string>>(); // List of the List-groups of single cards

    // SETTING THE SCORE TYPE
    public bool standardScoring = true;
    public static bool vegasScoring = false;
    
    

    // Start is called before the first frame update
    void Start()
    {
        userInput = FindObjectOfType<UserInput>(); // Finding the UserInput script on start
        bottoms = new List<string>[] { bottom0, bottom1, bottom2, bottom3, bottom4, bottom5, bottom6 };

        //Set the score type
        if (!standardScoring)
        {
            vegasScoring = true;
            Scoring.scoreValue = 52;
        }

        PlayCards();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // PlayCards Method that calls gen-deck Starts New Game
    public void PlayCards()
    {
        reShuffleCounter = 0;
        // clear all lists for fresh game
        foreach (List<string> list in bottoms)
        {
            list.Clear();
        }
        
        deck = GenerateDeck();
        //call the shuffle method and feed it the deck
        Shuffle(deck);

        SolitareSort();
        //Call the deal method
        StartCoroutine(SolitareDeal());
        if (threeCardDeal)
        {
            SortDeckIntoTrips();
        }
        else
        {
            SortDeckIntoSingles();
        }
    }

    // Generate a deck by adding a new string

    public static List<string> GenerateDeck()
    {
        List<string> newDeck = new List<string>();
        foreach (string s in suits)
        {
            foreach (string v in values)
            {
                newDeck.Add(s + v);
            }
        }

        return newDeck;
    }
    
    // SHUFFLE METHOD 1 
    void Shuffle<T>(List<T> list)
    {
        System.Random random = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            int k = random.Next(n);
            n--;
            T temp = list[k];
            list[k] = list[n];
            list[n] = temp;
        }
    }
    


    // Method To Deal The Cards
    IEnumerator SolitareDeal()
    {

        for (int i = 0; i < 7; i++)
        {

            float yOffset = 0;
            float zOffset = 0.03f;
            foreach (string card in bottoms[i])
            {
                // SLOW DOWN THE RETURN TO SLOW THE DEAL
                yield return new WaitForSeconds(0.01f);
                // creates a new gameobject called 'card' using the cardprefab at 0,0,0
                GameObject newCard = Instantiate(cardPrefab, new Vector3(bottomPos[i].transform.position.x, bottomPos[i].transform.position.y - yOffset, bottomPos[i].transform.position.z - zOffset), Quaternion.identity, bottomPos[i].transform);
                newCard.name = card;
                newCard.GetComponent<Selectable>().row = i; // Sets Which ROW the card is on
                // make card face up ONLY if card is the bottom card of the bottom array
                if (card == bottoms[i][bottoms[i].Count -1])
                {
                    newCard.GetComponent<Selectable>().faceUp = true;
                }
                

                yOffset = yOffset + 0.3f;
                zOffset = zOffset + 0.03f;
                discardPile.Add(card); // Add Card To Discard Pile List
            }
        }

        // Remove Duplicates?
        foreach (string card in discardPile)
        {
            if (deck.Contains(card))
            {
                deck.Remove(card);
            }
        }
        discardPile.Clear();
    }


    // Sorting The Cards Into Positions
    void SolitareSort()
    {
        for (int i = 0; i < 7; i++)
        {
            for (int j = i; j < 7; j++)
            {
                bottoms[j].Add(deck.Last<string>());
                deck.RemoveAt(deck.Count - 1);
            }
        }
    }


    // Deal Out Three Cards At The Top...
    public void SortDeckIntoTrips()
    {
        trips = deck.Count / 3;
        tripsRemainder = deck.Count % 3;
        deckTrips.Clear();

    int modifier = 0;
    for (int i = 0; i < trips; i++)
    {
        List<string> myTrips = new List<string>();
        for (int j = 0; j < 3; j++)
        {
            myTrips.Add(deck[j + modifier]);
        }
        deckTrips.Add(myTrips);
        modifier = modifier + 3;
    }
    // IF DECK.COUNT IS NOT DIVISIBLE BY 3 aka deck.Count % 3 != 0
    if (tripsRemainder != 0)
    {
        List<string> myRemainders = new List<string>();
        modifier = 0;
        for (int k = 0; k < tripsRemainder; k++)
        {
            myRemainders.Add(deck[deck.Count - tripsRemainder + modifier]);
            modifier++;
        }
        deckTrips.Add(myRemainders);
        trips++;
    }
    deckLocation = 0;

    }


    public void SortDeckIntoSingles()
    {
        trips = deck.Count / 1;
        //tripsRemainder = deck.Count % 1;
        deckTrips.Clear();

        for (int i = 0; i < trips; i++)
        {
            List<string> myTrips = new List<string>();
            myTrips.Add(deck[i]); //Add each card to myTrips list
            deckTrips.Add(myTrips);

        }
        deckLocation = 0;
    }










    // Method To Deal The Three Cards
    public void DealFromDeck()
    {

        //IF 3 CARD GAME IS TRUE
        if (threeCardDeal)
        {
            // add remaining cards to discard pile Remove Cards from DeckTop
            foreach (Transform child in DeckTop.transform)
            {
                if (child.CompareTag("Card"))
                {
                    deck.Remove(child.name);
                    discardPile.Add(child.name);
                    Destroy(child.gameObject);
                    userInput.SetSlot();
                }
            
            }

            if (deckLocation < trips)
            {
                // Draw 3 New Cards
                tripsOnDisplay.Clear();
                float xOffset = 2.5f;
                float zOffset = -0.2f;

                foreach (string card in deckTrips[deckLocation])
                {
                    GameObject newTopCard = Instantiate(cardPrefab, new Vector3(DeckTop.transform.position.x +xOffset, DeckTop.transform.position.y, DeckTop.transform.position.z + zOffset), Quaternion.identity, DeckTop.transform);
                    xOffset = xOffset + 0.5f;
                    zOffset = zOffset - 0.2f;
                    newTopCard.name = card;
                    tripsOnDisplay.Add(card);
                    newTopCard.GetComponent<Selectable>().faceUp = true;
                    newTopCard.GetComponent<Selectable>().inDeckPile = true; //Setting this card as inside the deck pile array

                }
                deckLocation++;
            }
            else 
            {
                //Restack The Top Deck Using Cards We Did Not Use
                RestackTopDeck();

            }
        }
        else // SINGLE CARD GAME IS ACTIVE
        {
            // add remaining cards to discard pile
            foreach (Transform child in DeckTop.transform)
            {
                if (child.CompareTag("Card"))
                {
                    deck.Remove(child.name);
                    //discardPile.Add(child.name);
                    //Destroy(child.gameObject);
                    userInput.SetSlot();
                }
            
            }
            DealFromDeckSingles();

        }
    }


    void RestackTopDeck()
    {
        deck.Clear();
        foreach (string card in discardPile)
        {
            deck.Add(card);
        }
        discardPile.Clear();
        //INCREASE RE-SHUFFLE COUNTER?
        reShuffleCounter++;
        //CHECK FOR GAME OVER...
        // if (!userInput.CheckForMoves())
        // {
        //     // GAME OVER... SEND GAME IS OVER MESSAGE
        //     print("GAME IS OVER DUDE RESTART");
        //     userInput.SetSlot();
        // }
        // else
        // {
        //     userInput.SetSlot();
        // }
        Shuffle(deck); // RESHUFFLE DECK

        if (threeCardDeal)
        {
            SortDeckIntoTrips();
        }
    }


    public void DealFromDeckSingles()
    {
        if (deckLocation < trips)
            {
                // Draw 1 New Cards
                //tripsOnDisplay.Clear();
                float xOffset = 2.5f;
                //float zOffset = 0.2f;
                

                foreach (string card in deckTrips[deckLocation])
                {
                    //zOffset = zOffset + 0.2f;
                    //zOffset += 0.2f;
                    GameObject newTopCard = Instantiate(cardPrefab, new Vector3(DeckTop.transform.position.x +xOffset, DeckTop.transform.position.y, DeckTop.transform.position.z + zOff), Quaternion.identity, DeckTop.transform);
                    //xOffset = xOffset + 0.5f;
                    //zOffset = zOffset + 0.2f;
                    zOff -= 0.2f;
                    newTopCard.name = card;
                    tripsOnDisplay.Add(card);
                    newTopCard.GetComponent<Selectable>().faceUp = true;
                    newTopCard.GetComponent<Selectable>().inDeckPile = true; //Setting this card as inside the deck pile array

                }
                deckLocation++;
            }
            else // DECK IS EMPTY... RESTACK
            {
                //Restack The Top Deck Using Cards We Did Not Use
                //RestackTopDeck();
                deck.Clear(); // Just in case idk
                zOff = 0.2f; //RESET THE Zoff
                foreach (string card in tripsOnDisplay)
                {
                    deck.Add(card);
                }
                tripsOnDisplay.Clear();
                // REMOVE all DeckTop GameObjects
                foreach (Transform child in DeckTop.transform)
                {
                    Destroy(child.gameObject);

                }
                //INCREASE RE-SHUFFLE COUNTER?
                reShuffleCounter++;
                Shuffle(deck); // RESHUFFLE DECK
                SortDeckIntoSingles();
            }
    }





}
