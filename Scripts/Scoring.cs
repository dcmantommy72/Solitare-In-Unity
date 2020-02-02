using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scoring : MonoBehaviour
{

    private int scoreCount;
    public static int scoreValue = 0;
    Text score;
    //public int newScore;



    // Start is called before the first frame update
    void Start()
    {
        score = GetComponent<Text>();

    }

    // Update is called once per frame
    void Update()
    {
        score.text = "" + scoreValue;
    }

    public void UpdateScore(int newScore)
    {
        scoreValue += newScore;
    }


}
