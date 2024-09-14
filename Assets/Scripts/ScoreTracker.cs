using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreTracker : MonoBehaviour
{
    public TMP_Text scoreDisplay;
    public TornadoPhysics tornado;
    public float scoreMultiplier;
    private float score = 0;
    private string originalText;

    private float startTime;
    public float stopTimeSeconds;

    // Multiplier for weight, increasing this reduces score
    public float weightMultiplier;

    public System.Action<float, Multiplier> UpdateScore;

    public enum Multiplier
    {
        None,
        Score,
        Weight,
        ScoreWeight
    }

    private void NoOpScore(float amount, Multiplier m) { }

    public void AlterScore(float amount)
    {
        this.AlterScore(amount, Multiplier.None);
    }

    public void AlterScore(float amount, Multiplier m)
    {
        switch(m) 
        {
            case Multiplier.None:
                this.score += amount;
                break;
            case Multiplier.Score:
                this.score += amount * this.scoreMultiplier;
                break;
            case Multiplier.Weight:
                this.score += amount / this.weightMultiplier;
                break;
            case Multiplier.ScoreWeight:
                this.score += amount * (this.scoreMultiplier / this.weightMultiplier);
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        originalText = scoreDisplay.text;
        this.UpdateScore = this.AlterScore;
    }

    // Update is called once per frame
    void Update()
    {
        this.UpdateScore(Time.deltaTime, Multiplier.ScoreWeight);
        scoreDisplay.text = originalText + System.Math.Floor(score).ToString();
    }

    void FixedUpdate()
    {
        if ((Time.time - this.startTime) > stopTimeSeconds)
        {
            this.UpdateScore = this.NoOpScore;
            tornado.OnStopScoring();
        }
    }
}
