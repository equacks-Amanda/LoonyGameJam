using System.Collections;
using System.Collections.Generic;
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
            currentPlayer.Initialize(playerModel.currentState, playerModel.jumpSpeed,playerModel.moveSpeed, playerModel.airSpeed);
        }
    }

    void OnShapeShift(PlayerEvents.ShapeShift @event)
    {
        playerModel.currentState += 1;
        currentPlayer.ShapeShift(playerModel.currentState);
    }

    void OnCollisionWithObstacle(PlayerEvents.ObsCollision @event)
    {
        //Get Obstacle Class
        Obstacle collidedObs = @event.obstacle.GetComponent<Obstacle>();
        if(collidedObs)
        {
            if(playerModel.currentState == collidedObs.requiredState)
            {
                @event.canPlayerPass(true);
            }
            else
            {
                @event.canPlayerPass(false);
            }
        }
    }
}
