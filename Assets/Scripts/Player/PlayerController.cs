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
        PState newState = playerModel.currentState += 1;
        PStateCollider newStateCollider = playerModel.currentStateCollider += 1;
        if ((int)playerModel.currentState > Enum.GetValues(typeof(PState)).Length -1)
        {
            newState = 0;
            newStateCollider = 0;
        }
        playerModel.currentState = newState;
        playerModel.currentStateCollider = newStateCollider;
        currentPlayer.ShapeShift(newState, newStateCollider);
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
