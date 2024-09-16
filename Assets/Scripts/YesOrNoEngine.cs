using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Audio;
using System;



public class YesOrNoEngine : MonoBehaviour
{
    public static YesOrNoEngine instance;
    public TextToSpeech textToSpeech;
    private float posedLast = 0;
    private Action<bool> onAnswer;
    public int attentionSpan = 30;
    // Start is called before the first frame update
    void Start()
    {
        if (instance != null)
        {
            Debug.Log("Uh oh");
        }
        instance = this;
    }

    public void PoseQuestion(string prompt, Action<bool> onAnswer)
    {
        textToSpeech.StartSpeaking(prompt);
        posedLast = Time.time;
        this.onAnswer = onAnswer;
    }

    public void SpeakYes()
    {
        if (Time.time - posedLast < attentionSpan && this.onAnswer != null)
        {
            this.onAnswer(true);
        }
    }

    public void SpeakNo()
    {
        if (Time.time - posedLast < attentionSpan && this.onAnswer != null)
        {
            this.onAnswer(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    //Predef questions
    /*public void PromptBluePickup()
    {
        if (RobotActions.instance.state == RobotActions.RobotState.PICK_ON_MOVE)
        {
            instance.PoseQuestion("I see the blue cup has stopped, should I pick it up?", (bool answer) =>
            {
                if (answer)
                {
                    RobotActions.instance.PickUpBlueCup();
                }
            });
        }
    }*/

    public void PromptPrismPickup()
    {
        if (RobotActions.instance.state == RobotActions.RobotState.PICK_ON_MOVE)
        {
            instance.PoseQuestion("I see the prisms on the table, should I begin sorting?", (bool answer) =>
            {
                if (answer)
                {
                    RobotActions.instance.PickUpPrism();
                }
            });
        }
    }

    public void PromptPlaceBox()
    {
        if (RobotActions.instance.state == RobotActions.RobotState.HOLDING)
        {
            instance.PoseQuestion("I see the box, would you like me to put what I'm holding away?", (bool answer) =>
            {
                if (answer)
                {

                    GameHandler.instance.PourToggle(false);
                    RobotActions.instance.PlaceInBox();
                }
            });
        }
    }

    public void PromptInsert()
    {
        if (RobotActions.instance.state == RobotActions.RobotState.HOLDING)
        {
            instance.PoseQuestion("I see the base, would you like me to insert the prism?", (bool answer) =>
            {
                if (answer)
                {

                    /*GameHandler.instance.PourToggle(false);
                    RobotActions.instance.PlaceInBox();*/
                }
            });
        }
    }
}
