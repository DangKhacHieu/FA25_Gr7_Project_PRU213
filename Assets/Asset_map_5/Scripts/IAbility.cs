using UnityEngine;

// Định nghĩa Interface chung cho mọi kỹ năng của quái vật
public interface IAbility
{
    // Phương thức này sẽ được gọi trong Awake() hoặc Start() của Component Kỹ năng
    // để đăng ký lắng nghe các sự kiện từ quái vật (Enemy)
    void SetupAbility(Bongma enemy);
}
