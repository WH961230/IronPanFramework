namespace Kuroha.UtilitiesCollection
{
    public interface IReorderableListDropTarget
    {
        bool CanDropInsert(int insertionIndex);

        void ProcessDropInsertion(int insertionIndex);
    }
}