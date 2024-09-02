using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Goal : MonoBehaviour
   
{
    private float score;
    // Start is called before the first frame update
    void Start()
    {
        score=transform.localPosition.y; ;
    }

    public float getScore() {
        return (float)Math.Pow(Math.E,score);
    }


  
}
