// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeProtected.Global

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kuroha.UtilitiesCollection
{
    internal class GenericList<T> : IReorderableListAdaptor
    {
        public IList<T> List { get; }
        public float FixedItemHeight { get; }
        public ReorderableListControl.ItemDrawer<T> ItemDrawer { get; }

        public GenericList(IList<T> list, float itemHeight, ReorderableListControl.ItemDrawer<T> itemDrawer = null)
        {
            List = list;
            ItemDrawer = itemDrawer ?? ListUI.DefaultItemDrawer;
            FixedItemHeight = itemHeight;
        }

        public int Count => List.Count;
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
            List.Add(default);
        }

        public virtual void Insert(int index)
        {
            List.Insert(index, default);
        }

        public virtual void Duplicate(int index)
        {
            if (List[index] is ICloneable cloneable)
            {
                List.Insert(index + 1, (T) cloneable.Clone());
            }
        }

        public virtual void Remove(int index)
        {
            List.RemoveAt(index);
        }

        public virtual void Move(int sourceIndex, int destIndex)
        {
            if (destIndex > sourceIndex)
            {
                --destIndex;
            }

            var item = List[sourceIndex];
            List.RemoveAt(sourceIndex);
            List.Insert(destIndex, item);
        }

        public virtual void Clear()
        {
            List.Clear();
        }

        public virtual void DrawItem(Rect rect, int index)
        {
            List[index] = ItemDrawer(rect, List[index]);
        }

        public virtual float GetItemHeight(int index)
        {
            return FixedItemHeight;
        }
    }
}
