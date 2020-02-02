using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateSprite : MonoBehaviour
{

    public Sprite cardFace;
    public Sprite cardBack;
    private SpriteRenderer spriteRenderer;
    private Selectable selectable;
    private Solitare solitare;
    private UserInput userInput;
    

    // Start is called before the first frame update
    void Start()
    {
        List<string> deck = Solitare.GenerateDeck();
        solitare = FindObjectOfType<Solitare>();
        userInput = FindObjectOfType<UserInput>(); // Finding the UserInput script on start

        int i = 0;
        foreach (string card in deck)
        {
            if (this.name == card) 
            {
                cardFace = solitare.cardFaces[i];
                break;
            }
            i++;
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        selectable = GetComponent<Selectable>();

    }

    // Update is called once per frame
    void Update()
    {
         if (selectable.faceUp == true)
         {
             spriteRenderer.sprite = cardFace;
         }
         else
         {
             spriteRenderer.sprite = cardBack;
         }

         // IF there is a card selected in slot1
         if (userInput.slot1)
         {
            // If the name of the card this script is attached too is equal to the name of the gameObject in slot1:
                // Its a selected card, tint it yellow (putting this in the update method, is expensive due to it checking all the time)
                // So its probably better to later on make this so that it runs when a card is clicked instead of on update....
            if (name == userInput.slot1.name)
            {
                spriteRenderer.color = Color.yellow;
            }
            else
            {
                spriteRenderer.color = Color.white;
            }
         }


    }
}
