using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBT
{
    [AddComponentMenu("")]
    [MBTNode(name = "Conditions/Number Condition")]
    public class NumberCondition : Condition
    {
        public Abort abort;
        public Type type = Type.Float;
        public FloatReference floatReference;
        public IntReference intReference;
        public Comparator comparator = Comparator.Equal;
        public float floatValue;
        public int intValue;

        // IMPROVEMENT: This class could be split into to different nodes
        public override bool Check()
        {
            if (type == Type.Float)
            {
                switch (comparator)
                {
                    case Comparator.Equal:
                        return floatReference.GetVariable().Value == floatValue;
                    case Comparator.GreaterThan:
                        return floatReference.GetVariable().Value > floatValue;
                    case Comparator.LessThan:
                        return floatReference.GetVariable().Value < floatValue;
                }
            }
            else
            {
                switch (comparator)
                {
                    case Comparator.Equal:
                        return intReference.GetVariable().Value == intValue;
                    case Comparator.GreaterThan:
                        return intReference.GetVariable().Value > intValue;
                    case Comparator.LessThan:
                        return intReference.GetVariable().Value < intValue;
                }
            }
            return false;
        }

        public override void OnAllowInterrupt()
        {
            if (abort != Abort.None)
            {
                if (type == Type.Float) {
                    floatReference.GetVariable().AddListener(OnVariableChange);
                } else {
                    intReference.GetVariable().AddListener(OnVariableChange);
                }
            }
        }

        public override void OnDisallowInterrupt()
        {
            if (abort != Abort.None)
            {
                if (type == Type.Float) {
                    floatReference.GetVariable().RemoveListener(OnVariableChange);
                } else {
                    intReference.GetVariable().RemoveListener(OnVariableChange);
                }
            }
        }

        private void OnVariableChange(float newVal, float oldVal)
        {
            EvaluateConditionAndTryAbort(abort);
        }

        private void OnVariableChange(int newVal, int oldVal)
        {
            EvaluateConditionAndTryAbort(abort);
        }

        public enum Type
        {
            Float, Int
        }

        public enum Comparator
        {
            Equal, GreaterThan, LessThan
        }
    }
}
