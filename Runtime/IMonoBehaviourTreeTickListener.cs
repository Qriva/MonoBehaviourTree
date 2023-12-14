using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    public interface IMonoBehaviourTreeTickListener
    {
        void OnBehaviourTreeTick();
    }
}
