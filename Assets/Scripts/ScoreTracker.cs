using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreTracker : MonoBehaviour
{
    public TMP_Text scoreDisplay;
    public float scoreMultiplier;
    private float score = 0;
    private string originalText;

    // Start is called before the first frame update
    void Start()
    {
        originalText = scoreDisplay.text;
    }

    // Update is called once per frame
    void Update()
    {
        score += Time.deltaTime;
        scoreDisplay.text = originalText + System.Math.Floor(score).ToString();
    }
}
