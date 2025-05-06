// 可被射击的接口
public interface IShootable
{
    void OnShot(BulletLifecycle bullet); // 被子弹击中时调用的方法
}
