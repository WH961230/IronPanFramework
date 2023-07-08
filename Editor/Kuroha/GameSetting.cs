using UnityEngine;

namespace Kuroha
{
    public class GameSetting : ScriptableObject
    {
        // 如果出现和活动无关, 只有游戏自身存在框架或者逻辑的巨大变动, 则变更 version1
        public int version1;
        
        // 出现赛季变更时, 修改 version2
        public int version2;
        
        // 出现基本的节日活动时, 修改 version3
        public int version3;
        
        // 微小的功能更新, 修改 version4
        public int version4;
        
        // 紧急修复的问题, 无关紧要的资源更新, 配置更新, 修改 version5
        public int version5;

        public string GetVersion()
        {
            return $"{version1}.{version2}.{version3}.{version4}.{version5}";
        }
    }
}
