using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace PlayerEvents
{
   public struct ShapeShift : iEvent
   {
        public readonly PState setState;

        public ShapeShift(PState stateToSet)
        {
            setState = stateToSet;
        }
   }

   public struct ObsCollision : iEvent
   {
       public readonly GameObject obstacle;

       public readonly Action<bool> isCorrectState;

       public ObsCollision(GameObject _obstacle, Action<bool> stateCheckCallback )
       {
           obstacle = _obstacle;
           isCorrectState = stateCheckCallback;
       }
   }

    public struct SendTransform : iEvent
    {
        public readonly Transform playerTransform;

        public SendTransform(Transform pTransform)
        {
            playerTransform = pTransform;
        }
    }

    public struct RespawnPlayer : iEvent 
    {
        public Transform playerTransform;

        public RespawnPlayer(Transform referenceToPlayerTransform)
        {
            playerTransform = referenceToPlayerTransform;
        }
    }
}
