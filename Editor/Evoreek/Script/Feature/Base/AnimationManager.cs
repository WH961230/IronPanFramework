using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : IManager {
    private Dictionary<int, AnimatorBlock> animatorDics = new Dictionary<int, AnimatorBlock>();
    private GameSystem gameSystem;

    public void Init(GameSystem gameSystem) {
        this.gameSystem = gameSystem;
    }

    public void Register(int id, Animator animator) {
        if (animatorDics.ContainsKey(id)) {
            AnimatorBlock tempAnim = animatorDics[id];
            if (tempAnim.MyAnimator != null) {
                Logger.PrintELog($"ID: {id} 已存在状态机 重复添加");
            }

            animatorDics[id].MyAnimator = animator;
        } else {
            animatorDics.Add(id, new AnimatorBlock() {
                MyAnimator = animator
            });
        }
    }

    public void UnRegister(int id) {
        if (animatorDics.ContainsKey(id)) {
            AnimatorBlock tempAnim = animatorDics[id];
            if (tempAnim.MyAnimator != null) {
                tempAnim.MyAnimator = null;//清空状态机
            } else {
                Logger.PrintELog($"ID: {id} 状态机已销毁 重复销毁");
            }
        } else {
            Logger.PrintELog($"ID: {id} 未存在注册的状态机 无效销毁");
        }
    }

    public Animator GetAnimator(int id) {
        if (animatorDics.ContainsKey(id)) {
            return animatorDics[id].MyAnimator;
        }
        // Logger.PrintE($"找不到动画状态机 id未注册：{id}");
        return null;
    }
}

internal class AnimatorBlock {
    private Animator myAnimator;
    public Animator MyAnimator {
        get { return myAnimator;}
        set {
            // Logger.Print("设置动画状态机");
            myAnimator = value;
        }
    }
}