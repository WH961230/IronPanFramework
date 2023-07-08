// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeProtected.Global

using System;
using UnityEditor;
using UnityEngine;

namespace Kuroha.UtilitiesCollection
{
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    internal class MonoBehaviourList : IReorderableListAdaptor
    {
        public SerializedProperty List { get; }
        public float FixedItemHeight { get; }
        public SerializedProperty this[int index] => List.GetArrayElementAtIndex(index);

        public MonoBehaviourList(SerializedProperty list, float fixedItemHeight = 0)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            if (!list.isArray)
            {
                throw new InvalidOperationException("Specified serialized property is not an array.");
            }

            List = list;
            FixedItemHeight = fixedItemHeight;
        }

        public int Count => List.arraySize;
        public virtual void BeginGUI() { }
        public virtual void EndGUI() { }
        public virtual void DrawItemBackground(Rect rect, int index) { }

        public virtual bool CanDrag(int index)
        {
            return true;
        }

        public virtual bool CanRemove(int index)
        {
            return true;
        }

        public virtual void Add()
        {
            var newIndex = List.arraySize;
            ++List.arraySize;
            SerializedPropertyUtility.ResetValue(List.GetArrayElementAtIndex(newIndex));
        }

        public virtual void Insert(int index)
        {
            List.InsertArrayElementAtIndex(index);
            SerializedPropertyUtility.ResetValue(List.GetArrayElementAtIndex(index));
        }

        public virtual void Duplicate(int index)
        {
            List.InsertArrayElementAtIndex(index);
        }

        public virtual void Remove(int index)
        {
            var elementProperty = List.GetArrayElementAtIndex(index);

            // Unity doesn't remove element when it contains an object reference.
            // Set objectReferenceValue null before remove
            if (elementProperty.propertyType == SerializedPropertyType.ObjectReference)
            {
                elementProperty.objectReferenceValue = null;
            }

            List.DeleteArrayElementAtIndex(index);
        }

        public virtual void Move(int sourceIndex, int destIndex)
        {
            if (destIndex > sourceIndex)
            {
                --destIndex;
            }

            List.MoveArrayElement(sourceIndex, destIndex);
        }

        public virtual void Clear()
        {
            List.ClearArray();
        }

        public virtual void DrawItem(Rect rect, int index)
        {
            EditorGUI.PropertyField(rect, this[index], GUIContent.none, false);
        }

        public virtual float GetItemHeight(int index)
        {
            return FixedItemHeight != 0f ? FixedItemHeight : EditorGUI.GetPropertyHeight(this[index], GUIContent.none, false);
        }
    }
}
