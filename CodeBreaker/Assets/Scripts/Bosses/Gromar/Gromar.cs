using CodeBreaker;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gromar : MonoBehaviour
{
    public Transform MINSHOOT;
    public Transform MAXSHOOT;
    public Transform MAPMIDPOINT;
    public List<Transform> mapPoints;
    public GameObject bigBullet;
    public GameObject smallBullet;
    public GromarStateMachine StateMachine;
    public Player player;
    public Transform ShootingPoint;

    public Vector3 GetRandomShootPosition()
    {
        var diff = (MAXSHOOT.position - MINSHOOT.position) * UnityEngine.Random.Range(0f, 1f);
        return MINSHOOT.position+ diff;
    }


    public void FacePlayer()
    {
        if (player == null) { return; }

        Vector3 direction = player.transform.position - transform.position;
        if(direction.x > 0)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
        else if (direction.x < 0)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
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
        FacePlayer();
    }
}
