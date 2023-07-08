using Kuroha.Asset;
using Kuroha.Utility;

namespace Common
{
    public abstract class Global
    {
        private static AssetSystem assetSystem;
        public static AssetSystem Asset => assetSystem ??= new AssetSystem();

        private static Kuroha.UI.UISystem uiSystem;
        public static Kuroha.UI.UISystem UI => uiSystem ??= new Kuroha.UI.UISystem();

        private static Timer timer;
        public static Timer Timer => timer ??= new Timer();
    }
}
