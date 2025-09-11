using CodeBreaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    
    public Rigidbody2D Rb;
    public PhysicsInfo PhysicsInfo;
    public float GroundSpeed;
    public int Direction = 1; //-1 = gauche , 1 = droite
    public PlayerStateMachine StateMachine;
    public LayerMask LayerMask;
    public float XSpeed //vitesse horizontale du joueur
    {
        get => Rb.velocity.x;

        set => Rb.velocity = new Vector2(value, Rb.velocity.y);
    }
    public float YSpeed //vitesse verticale du joueur
    {
        get => Rb.velocity.y;
        set => Rb.velocity = new Vector2(Rb.velocity.x, value);
    }

    /// <summary>
    /// Regarde si le joueur touche le sol en envoyant un raycast
    /// </summary>
    /// <returns></returns>
    public bool CheckOnGround()
    {
        var GroundRay = Physics2D.Raycast(transform.position, Vector2.down, 0.25f, LayerMask);
        Debug.DrawRay(transform.position, Vector2.down*.25f);
        if (GroundRay)
        {
            Rb.position = GroundRay.point;
            return true;
        }
        return false;
    }

    void Start()
    {
        StateMachine.Init();
    }

    
    void Update()
    {
        if (PhysicsInfo != null)
        {
            Debug.Log($"[Player Debug] XSpeed: {XSpeed:F2}, YSpeed: {YSpeed:F2}, MaxSpeed: {PhysicsInfo.MaxSpeed:F2}, Acceleration: {PhysicsInfo.Acceleration:F2}");
        }
    }
}
