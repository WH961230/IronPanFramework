using UnityEngine;

namespace Kuroha.UtilitiesCollection
{
    public interface IReorderableListAdaptor
    {
        int Count { get; }

        bool CanDrag(int index);

        bool CanRemove(int index);

        void Add();

        void Insert(int index);

        void Duplicate(int index);

        void Remove(int index);

        void Move(int sourceIndex, int destIndex);

        void Clear();

        void BeginGUI();

        void EndGUI();

        void DrawItemBackground(Rect rect, int index);

        void DrawItem(Rect rect, int index);

        float GetItemHeight(int index);
    }
}
