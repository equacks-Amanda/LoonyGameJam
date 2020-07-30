using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollowPlayer : MonoBehaviour
{
    [HideInInspector]
    public Transform playerTransform;

    [HideInInspector]
    public Vector2 offSet;

    private void Awake()
    {
        if(EventManager.instance)
        {
            EventManager.instance.AddListener<PlayerEvents.SendTransform>((e)=> 
            {
                playerTransform = e.playerTransform;
            });
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(playerTransform!=null)
        {
            Vector3 temp = this.transform.position;

            temp.x = playerTransform.position.x + offSet.x;
            temp.y = playerTransform.position.y + offSet.y;

            transform.position = temp;
        }
    }
}
