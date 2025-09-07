using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodeBreaker
{
    public class PlayerState : State
    {
        public int StateNumber = -1;
        public Player Player;
        public PlayerStateMachine Machine;
        public Transform Transform => Player.transform;

       
        public Rigidbody2D Rb => Player.Rb;
        public PhysicsInfo PhysicsInfo => Player.PhysicsInfo;
        

        public PlayerState(int number = -1)
        {
            StateNumber = number;
        }
        public override void OnEnter()
        {

        }

        public override void OnExit()
        {

        }
        public override void OnUpdate()
        {
           
        }
        public void SetDirection(float xspeed)
        {
            if (Mathf.Abs(xspeed) > .001f)
            {
                Player.Direction = (int)Mathf.Sign(xspeed);
            }
        }

        public bool SameDirection(float XInput, float Speed)
        {
            return Math.Sign(XInput) == Math.Sign(Speed);
        }

    }
}