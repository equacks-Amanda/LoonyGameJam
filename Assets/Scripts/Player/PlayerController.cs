using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerController
{
    PlayerView playerPrefab;

    PlayerView currentPlayer;

    PlayerStats playerModel;

    public PlayerController(PlayerView prefab, PlayerStats model, Transform spawner)
    {
        playerPrefab = prefab;
        playerModel = model;

        Subscribe();
        InitializePlayer(spawner);
        Debug.Log(" PC ==> CREATED");
    }

    void Subscribe()
    {
        if(EventManager.instance != null)
        {
            EventManager.instance.AddListener<PlayerEvents.ObsCollision>(OnCollisionWithObstacle);
            EventManager.instance.AddListener<PlayerEvents.ShapeShift>(OnShapeShift);
        }
    }
    
    void InitializePlayer(Transform playerSpawn)
    {
        if(playerSpawn != null && currentPlayer == null)
        {
            //Found Spawn
            currentPlayer = GameObject.Instantiate(playerPrefab, playerSpawn, true);
            EventManager.instance.QueueEvent(new PlayerEvents.SendTransform(currentPlayer.transform));
            currentPlayer.Initialize(playerModel.currentState, playerModel.currentStateCollider, playerModel.jumpSpeed,playerModel.moveSpeed, playerModel.airSpeed);
        }
    }

    void OnShapeShift(PlayerEvents.ShapeShift @event)
    {
        int shape = (int)@event.setState;
        PStateCollider newStateCollider = (PStateCollider)shape;
        
        playerModel.currentState = @event.setState;
        playerModel.currentStateCollider = newStateCollider;
        currentPlayer.ShapeShift(@event.setState, newStateCollider);
    }

    void OnCollisionWithObstacle(PlayerEvents.ObsCollision @event)
    {
        //Get Obstacle Class
        Obstacle collidedObs = @event.obstacle.GetComponent<Obstacle>();
        if(collidedObs)
        {
            if(playerModel.currentState == collidedObs.requiredState)
            {
                @event.isCorrectState(true);
            }
            else
            {
                @event.isCorrectState(false);
            }
        }
    }
}
