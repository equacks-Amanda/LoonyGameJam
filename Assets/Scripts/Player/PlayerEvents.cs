using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace PlayerEvents
{
   public struct ShapeShift : iEvent
   {

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
}
