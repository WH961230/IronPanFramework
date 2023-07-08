public class EntityFeature : IFeature {
    private EntityManager entityManager = new EntityManager();
    private Game game;

    public void Init(Game game) {
        this.game = game;
    }

    public void FixedUpdate() {
        entityManager.FixedUpdate();
    }

    public void LateUpdate() {
        entityManager.LateUpdate();
    }

    public void Clear() {
        entityManager.RemoveAll();
    }

    public void Update() {
        entityManager.Update();
    }

    // 实体注册
    public void Register<T>(Data data) where T : Entity, new() {
        entityManager.Register<T>(game, data);
    }

    public void Remove(int id) {
        entityManager.Remove(id);
    }

    public void Remove(Entity entity) {
        entityManager.Remove(entity);
    }

    public T Get<T>(int id) where T : Entity, new() {
        return (T) entityManager.Get(id);
    }
    
    public Entity Get(int id) {
        return entityManager.Get(id);
    }
}