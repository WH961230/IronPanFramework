public interface IWindow {
    void Init(Game game, GameObj gameObj);
    void Update();
    void FixedUpdate();
    void LateUpdate();
    void Clear();
}