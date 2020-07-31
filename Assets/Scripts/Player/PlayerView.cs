using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AnimatorController))]
public class PlayerView : MonoBehaviour
{
    Rigidbody2D playerRB;

    Dictionary<string, GameObject> playerStateColliders = new Dictionary<string, GameObject>();

    Animator playerAnimCtrl;

    bool canMove = true;

    bool isJumping = true;

    Collider2D currentObstacle;

    float jumpForce;

    float moveForce;

    float inAirForce;

    PState pendingState;
    
    // The state the player is shifting to next
    PStateCollider pendingStateCollider;
    // The player's previous state.  Also technically counts as their current state until right before the switch
    PStateCollider previousStateCollider = PStateCollider.SquareCollider;


    private void Start()
    {
        Debug.Log("PV ==> Created");
        EventManager.instance.AddListener<PlayerEvents.RespawnPlayer>(OnRespawnPlayer);
    }

    private void FixedUpdate() 
    {
        //Check if falling
        if(!Physics2D.Raycast(this.transform.position, Vector2.down, 1f))
        {
            isJumping = true;
            Debug.Log("FALLING");
        }

        if (canMove)
        {
            Vector2 direction = Vector2.zero;
            if((Input.GetKey("right") || Input.GetKey("d")) && (!IsState(PStateCollider.SquareCollider) && !IsState(PStateCollider.KeyCollider)))
            {
                // To the right...
                direction = Vector2.right;
            }
            else if((Input.GetKey("left") || Input.GetKey("a")) && (!IsState(PStateCollider.SquareCollider) && !IsState(PStateCollider.KeyCollider)))
            {
                // To the left ...
                direction = Vector2.left;
            }
            else if(Input.GetKey("space") && IsState(PStateCollider.SquareCollider) && !isJumping)
            {
                // One hop this time...
                direction = Vector2.up;
                isJumping = true;
            }
            else if(Input.GetKey("down") || Input.GetKey("s"))
            {
                // how low can you go?
            }
            else if(Input.GetKey("1"))
            {
                canMove = false;
                playerRB.Sleep();
                EventManager.instance.QueueEvent(new PlayerEvents.ShapeShift(PState.SQUARE));
                return;
            }
            else if (Input.GetKey("2"))
            {
                canMove = false;
                playerRB.Sleep();
                EventManager.instance.QueueEvent(new PlayerEvents.ShapeShift(PState.CIRCLE));
                return;
            }
            else if (Input.GetKey("3"))
            {
                canMove = false;
                playerRB.Sleep();
                EventManager.instance.QueueEvent(new PlayerEvents.ShapeShift(PState.PARASOL));
                return;
            }
            else if (Input.GetKey("4"))
            {
                canMove = false;
                playerRB.Sleep();
                EventManager.instance.QueueEvent(new PlayerEvents.ShapeShift(PState.TRIANGLE));
                return;
            }

            if(direction != Vector2.zero)
                UpdateMovement(direction, Time.fixedDeltaTime);
        }
    }

    private void UpdateMovement(Vector2 direction, float delta)
    {
        Vector2 currentPosition = new Vector2(this.transform.position.x, this.transform.position.y);
        if (!isJumping )
        {
            if(IsState(PStateCollider.CircleCollider))
                playerRB.MovePosition(currentPosition + direction * moveForce * delta);
        }
        else
        {
            if(direction != Vector2.up)
            {
                //Glide if we're a parasol ~
                if (IsState(PStateCollider.ParasolCollider))
                {
                    playerRB.MovePosition(currentPosition + direction * moveForce * delta);
                    return;
                }

                //Obey gravity if we're not >.<
                playerRB.AddForce(direction * inAirForce);
            }
            else
            {
                //We're jumping upwards if we're a square :D
                if(IsState(PStateCollider.SquareCollider))
                    playerRB.AddForce(direction * jumpForce);
            }
            
        }
    }

    public void Initialize(PState startingState, PStateCollider startingStateCollider, float playerJumpSpeed, float playerMoveSpeed, float playerAirForce)
    {
        jumpForce = playerJumpSpeed;
        moveForce = playerMoveSpeed;
        inAirForce = playerAirForce;

        playerRB = GetComponent<Rigidbody2D>();
        playerAnimCtrl = GetComponent<Animator>();
        // Ensure starting collider matches starting state and all colliders are referenced
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            GameObject temp = gameObject.transform.GetChild(i).gameObject;
            playerStateColliders[temp.name.ToLower()] = temp;
        }
        previousStateCollider = startingStateCollider;
        pendingStateCollider = startingStateCollider;
        ShapeShift(startingState, startingStateCollider);
    }
  
    public void ShapeShift(PState newState, PStateCollider newStateCollider)
    {
        playerRB.velocity = Vector2.zero;
        pendingState = newState;
        pendingStateCollider = newStateCollider;

        Debug.LogFormat("PLAY STATE -> {0}", newState);
        //Reset Rotation
        gameObject.transform.rotation = new Quaternion(0, 0, 0, 0);
        //Allow RB motion as circle
        playerRB.freezeRotation = newState.ToString().ToLower().Equals("circle") ? false : true;

        playerAnimCtrl.Play("TRANSFORM");
    }

    public void EXT_OnTransformEnd()
    {
        if (previousStateCollider != pendingStateCollider)
        {
            playerStateColliders[pendingStateCollider.ToString().ToLower()].SetActive(true);
            playerStateColliders[previousStateCollider.ToString().ToLower()].SetActive(false);
        }
        previousStateCollider = pendingStateCollider;
        playerAnimCtrl.Play(pendingState.ToString());
        canMove = true;
        
        playerRB.WakeUp();
        //UpdateMovement(new Vector2(velocityBeforeTransition, 0), Time.fixedDeltaTime);
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
             else if( other.gameObject.tag == "GRND")
            {
                isJumping = false;
                playerRB.velocity = Vector2.zero;
                //Raycast out
                if (ReturnDirection(this.gameObject, other.gameObject) != HitDirection.Bottom)
                {
                    currentObstacle = other.collider;
                    AllowMoveThrough(false);
                }
            }
             else if(other.gameObject.tag == "Finish")
            {
                currentObstacle = other.collider;
                // Fire off event
                EventManager.instance.QueueEvent(new PlayerEvents.ObsCollision(other.gameObject, CanUnlockDoor));
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

    private void OnRespawnPlayer(PlayerEvents.RespawnPlayer @event)
    {
        playerRB.velocity = Vector2.zero;
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
            currentObstacle = null;
        }
    }

    private void CanUnlockDoor(bool canUnlock)
    {
        if (canUnlock)
        {
            Debug.Log("YOU WON!");
            playerRB.Sleep();
            playerAnimCtrl.Play("TRANSFORM");
            return;
        }

        Debug.Log("Sorry, but that's not a key to this door");
    }

    /// <summary>
    /// Compares current collider state to the parameter's state
    /// </summary>
    /// <param name="stateForCheck"></param>
    /// <returns>true if they match, else false</returns>
    private bool IsState(PStateCollider stateForCheck)
    {
        return (previousStateCollider == stateForCheck);
    }

    // TOOK THIS CODE FROM UNITY FORUMS - NEEDS REVIEWD AND POSSIBLY REVISED
    private enum HitDirection { None, Top, Bottom, Forward, Back, Left, Right }
    private HitDirection ReturnDirection(GameObject player, GameObject objCollision)
    {

        HitDirection hitDirection = HitDirection.None;
        RaycastHit MyRayHit;
        Vector3 direction = (player.transform.position - objCollision.transform.position).normalized;
        Ray MyRay = new Ray(objCollision.transform.position, direction);

        if (Physics.Raycast(MyRay, out MyRayHit))
        {

            if (MyRayHit.collider != null)
            {

                Vector3 MyNormal = MyRayHit.normal;
                MyNormal = MyRayHit.transform.TransformDirection(MyNormal);

                if (MyNormal == MyRayHit.transform.up) { hitDirection = HitDirection.Top; }
                if (MyNormal == -MyRayHit.transform.up) { hitDirection = HitDirection.Bottom; }
                if (MyNormal == MyRayHit.transform.forward) { hitDirection = HitDirection.Forward; }
                if (MyNormal == -MyRayHit.transform.forward) { hitDirection = HitDirection.Back; }
                if (MyNormal == MyRayHit.transform.right) { hitDirection = HitDirection.Right; }
                if (MyNormal == -MyRayHit.transform.right) { hitDirection = HitDirection.Left; }
            }
        }
        return hitDirection;
    }
}
