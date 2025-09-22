using CodeBreaker;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gromar : MonoBehaviour
{
    public Transform MINSHOOT;
    public Transform MAXSHOOT;
    public Transform mapMidPoint;
    public GameObject bigBullet;
    public GameObject smallBullet;
    public GromarStateMachine StateMachine;
    public Player player;

    public Vector3 GetRandomShootPosition()
    {
        var diff = (MAXSHOOT.position - MINSHOOT.position) * UnityEngine.Random.Range(0f, 1f);
        return MINSHOOT.position+ diff;
    }


    void Start()
    {
        player = FindObjectOfType<Player>();
        if (StateMachine != null)
        {
            StateMachine.Init();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
