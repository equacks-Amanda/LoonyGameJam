using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PlayerStats
{
    public PState currentState;

    public int totalHealth;

    public int currentHealth;

    public float moveSpeed;

    public float jumpSpeed;

    PlayerStats(PState startingState, int _totalHealth, int _currentHealth, float _moveSpeed, float _jumpSpeed)
    {
        currentState = startingState;
        totalHealth = _totalHealth;
        currentHealth = _currentHealth;
        moveSpeed = _moveSpeed;
        jumpSpeed = _jumpSpeed;
    }

    public bool IsDead()
    {
        if(currentHealth <= 0)
        {
            return true;
        }
        return false;
    }

    public void ResetHealth()
    {
        currentHealth = totalHealth;
    }

}

public enum PState
{
    square,
    circle,
    tear,
    triangle
}