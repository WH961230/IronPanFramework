using UnityEngine;

public class AnimationFeature : IFeature {
    private AnimationManager animationManager;
    public void Init(Game game) {
        animationManager = new AnimationManager();
        animationManager.Init(game.gameSystem);
    }

    /// <summary>
    /// 注册动画状态机
    /// </summary>
    /// <param name="id"></param>
    /// <param name="animator"></param>
    public void Register(int id, Animator animator) {
        animationManager.Register(id, animator);
    }

    /// <summary>
    /// 反注册动画状态机
    /// </summary>
    /// <param name="id"></param>
    public void UnRegister(int id) {
        animationManager.UnRegister(id);
    }

    /// <summary>
    /// 根据ID获取动画状态机
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Animator GetAnimator(int id) {
        return animationManager.GetAnimator(id);
    }

    /// <summary>
    /// 根据状态机和层级获取动画状态信息
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="layerIndex"></param>
    /// <returns></returns>
    public AnimatorStateInfo GetCurrentAnimatorStateInfo(Animator animator, int layerIndex) {
        if (animator == null) {
            Logger.PrintE("状态机为空！");
        }

        return animator.GetCurrentAnimatorStateInfo(layerIndex);
    }

    public void Update() {
    }

    public void FixedUpdate() {
    }

    public void LateUpdate() {
    }

    public void Clear() {
    }
}