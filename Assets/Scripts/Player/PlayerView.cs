﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerView : MonoBehaviour
{
    Collider2D playerCollider;

    Rigidbody2D playerRB;

    SpriteRenderer playerRenderer;

    [SerializeField]
    List<Sprite> playerSprites;

    bool canMove = true;

    bool isJumping = false;

    Collider2D currentObstacle;

    float jumpForce;

    float moveForce;

    private void Start()
    {
        Debug.Log("PV ==> Created");
    }

    private void FixedUpdate() 
    {
        if(canMove)
        {
            Vector2 direction = Vector2.zero;
            if(Input.GetKey("right") || Input.GetKey("d"))
            {
                // To the right...
                direction = Vector2.right;
            }
            else if(Input.GetKey("left") || Input.GetKey("a"))
            {
                // To the left ...
                direction = Vector2.left;
            }
            else if(Input.GetKey("space") && !isJumping)
            {
                // One hop this time...
                direction = Vector2.up;
            }
            else if(Input.GetKey("down") || Input.GetKey("s"))
            {
                // how low can you go?
            }
            else if(Input.GetKey("q"))
            {
                canMove = false;
                EventManager.instance.QueueEvent(new PlayerEvents.ShapeShift());
            }

            UpdateMovement(direction, Time.fixedDeltaTime);
        }
        
    }

    private void UpdateMovement(Vector2 direction, float delta)
    {
        Vector2 currentPosition = new Vector2(this.transform.position.x, this.transform.position.y);
        playerRB.MovePosition(currentPosition + direction * moveForce * delta);
    }

    public void Initialize(PState startingState, float playerJumpSpeed, float playerMoveSpeed)
    {
        //SetSprite(startingState);

        jumpForce = playerJumpSpeed;
        moveForce = playerMoveSpeed;

        playerCollider = GetComponent<Collider2D>();
        playerRB = GetComponent<Rigidbody2D>();
        playerRenderer = GetComponent<SpriteRenderer>();
    }
  
    public void ShapeShift(PState newState)
    {
        //TODO: Probably should animate this and stuff but for now just swap the sprite
        //SetSprite(newState);
    }

    void SetSprite(PState state)
    {
        Sprite startingSprite = playerSprites[(int)state];
        if(startingSprite != null)
        {
            playerRenderer.sprite = startingSprite;
        }
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        //Check to make sure we're not trying to pass over the same obstacle every frame of movement
        if(other.collider != currentObstacle)
        {
             if(other.gameObject.tag == "OBST")
            {
                canMove = false;
                currentObstacle = other.collider;
                //Fire Off Event
                EventManager.instance.QueueEvent(new PlayerEvents.ObsCollision(other.gameObject, AllowMoveThrough));
            }
        }
    }

    private void OnCollisionExit2D(Collision2D other) 
    {
        if(other.collider == currentObstacle)
        {
            //We left the currentObstacle.
            currentObstacle = null;
            canMove = true; 
        }
    }

    private void AllowMoveThrough(bool canPass)
    {
        if(canPass)
        {
            //Resume Moving
            canMove = true;
        }
        else
        {
            //Prevent Moving - push them out of the trigger
            Vector2 force = this.transform.position - currentObstacle.transform.position;

            force.Normalize();
            playerRB.AddForce( -force * jumpForce);
        }
    }

}