namespace Kuroha.UtilitiesCollection
{
    internal abstract class ToolBase
    {
        protected readonly ObjectPath objectUI = new ObjectPath();
        protected readonly FolderPath folderUI = new FolderPath();
        protected readonly ProgressBar progressBar = new ProgressBar();

        public abstract void OnGUI();

        public abstract void OnOpen();

        public abstract void OnClose();
    }
}
