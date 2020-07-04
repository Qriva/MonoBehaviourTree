﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    [AddComponentMenu("")]
    public class Vector2Variable : Variable<Vector2>
    {
        
    }

    [System.Serializable]
    public class Vector2Reference : VariableReference<Vector2Variable, Vector2>
    {
        public Vector2 Value
        {
            get
            {
                return (useConstant)? constantValue : this.GetVariable().Value;
            }
            set
            {
                if (useConstant)
                {
                    constantValue = value;
                }
                else
                {
                    this.GetVariable().Value = value;
                }
            }
        }
    }
}
