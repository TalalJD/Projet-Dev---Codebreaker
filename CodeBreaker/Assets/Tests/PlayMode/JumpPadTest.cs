using System.Collections;
using System.Collections.Generic;
using CodeBreaker;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class JumpPadTest
{
    private GameObject _playerGameObject;
    private Player _playerScript;
    private GameObject _jumpPadGameObject;
    private JumpPad _jumpPadScript;
    private BoxCollider2D _playerCollider;
    private Rigidbody2D _playerRB;

    [SetUp]
    public void Setup()
    {
        // Creer le joueur
        _playerGameObject = new GameObject("Player");
        _playerCollider = _playerGameObject.AddComponent<BoxCollider2D>();
        _playerGameObject.tag = "Player";
        _playerScript = _playerGameObject.AddComponent<Player>();
        _playerRB = _playerGameObject.AddComponent<Rigidbody2D>();
        _playerScript.Rb = _playerRB;

        _playerRB.gravityScale = 0f;

        _playerScript.StateMachine = _playerGameObject.AddComponent<PlayerStateMachine>();
        _playerScript.StateMachine.enabled = false;
        _playerScript.StateMachine.Init();

        var airState = new AirState { Player = _playerScript };
        var moveState = new MoveState { Player = _playerScript };

        _playerScript.StateMachine.Add(airState);
        _playerScript.StateMachine.Add(moveState);

        _playerScript.StateMachine.Set<MoveState>();

        airState.Player = _playerScript;
        moveState.Player = _playerScript;


        // Initialize Yspeed
        _playerScript.YSpeed = 0f;

        // Creer JumpPad

        _jumpPadGameObject = new GameObject("JumpPad");
        var jumpCollider = _jumpPadGameObject.AddComponent<BoxCollider2D>();
        jumpCollider.isTrigger = true;
        _jumpPadScript = _jumpPadGameObject.AddComponent<JumpPad>();

        _jumpPadScript.boostForce = 20f;



    }
    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(_playerGameObject);
        if (_jumpPadGameObject != null) Object.DestroyImmediate(_jumpPadGameObject);
    }

    [Test]
    public void JumpPadTest_Once()
    {
        Collider2D playerCollider = _playerGameObject.GetComponent<Collider2D>();
        var airState = _playerScript.StateMachine.Get<AirState>();
        Assert.IsNotNull(airState);

        // Une seule fois
        _jumpPadScript.OnTriggerEnter2D(playerCollider);

        // Assert sans changer de frame
        Assert.AreEqual(20f, _playerScript.YSpeed, "YSpeed doit etre boostForce");
        Assert.IsTrue(airState.isJump, "isJump doit etre true après le trigger");

       
    }
}
