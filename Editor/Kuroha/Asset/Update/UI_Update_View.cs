using Kuroha.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kuroha.Asset
{
    public class UI_Update_View : UIPanelView
    {
        public TextMeshProUGUI tmpLog;
        public Button tmpButton;
        
        public int percent;
        public int bgWidth;
        public int bgHeight;
        public TextMeshProUGUI tip;
        public RectTransform progressBg;
        public RectTransform progressPercent;
    }
}
