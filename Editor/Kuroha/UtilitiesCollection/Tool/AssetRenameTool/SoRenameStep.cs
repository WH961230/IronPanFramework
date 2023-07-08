using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kuroha.UtilitiesCollection
{
    [Serializable]
    internal class SoRenameStep : ScriptableObject
    {
        public enum OperaType
        {
            Delete,
            Remove,
            Insert,
            Replace,
            Order
        }

        public enum PositionType
        {
            Begin,
            End,
            Index
        }

        [Serializable]
        public struct DeleteStep
        {
            public int beginIndex;
            public int length;
        }

        [Serializable]
        public struct RemoveStep
        {
            public string regex;
        }

        [Serializable]
        public struct ReplaceStep
        {
            public string regex;
            public string newString;
        }

        [Serializable]
        public struct InsertStep
        {
            public PositionType paramType;
            public int index;
            public string content;
        }

        [Serializable]
        public struct OrderStep
        {
            public string suffix;
            public int beginNumber;
            public int zeroCount;
        }

        [Serializable]
        internal class RenameStep : ICloneable
        {
            [SerializeField]
            public OperaType operaType;

            [SerializeField]
            public InsertStep insertStep;

            [SerializeField]
            public DeleteStep deleteStep;

            [SerializeField]
            public RemoveStep removeStep;

            [SerializeField]
            public ReplaceStep replaceStep;

            [SerializeField]
            public OrderStep orderStep;

            public object Clone()
            {
                var clone = new RenameStep
                {
                    operaType = operaType,
                    insertStep = insertStep,
                    deleteStep = deleteStep,
                    removeStep = removeStep,
                    replaceStep = replaceStep,
                    orderStep = orderStep
                };

                return clone;
            }
        }

        [SerializeField]
        public List<RenameStep> steps = new List<RenameStep>();
    }
}
