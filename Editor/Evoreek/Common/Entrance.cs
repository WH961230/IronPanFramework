using System;
using Kuroha.Asset;
using UnityEngine;

namespace Common
{
    public class Entrance : MonoBehaviour
    {
        [SerializeField] private Transform panelUIParent;
        [SerializeField] private Transform windowUIParent;
        public LoggerType loggerType;
        private void OnEnable()
        {
            Global.Asset.OnEnable();
            Global.UI.OnEnable(panelUIParent, windowUIParent);
        }

        private void Start()
        {
            Global.UI.Start();
            //Global.UI.Panel.Open<UI_Update_Controller>();
            Game.Instance.loggerType = loggerType;
            Game.Instance.Start();
        }

        private void Update()
        {
            Global.UI.Update();
            Game.Instance.Update();
        }

        private void FixedUpdate()
        {
            Global.UI.FixedUpdate();
            Game.Instance.FixedUpdate();
        }

        private void LateUpdate()
        {
            Global.UI.LateUpdate();
            Game.Instance.LateUpdate();
        }
    }
}
