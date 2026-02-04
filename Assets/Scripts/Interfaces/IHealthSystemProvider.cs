using UnityEngine;

public interface IHealthSystemProvider // cuối mới dùng interface, nghiên cứu thêm  về interface
{
    // bất kì thằng nào thừa kế IHealthSystemProvider thì đều có khả năng đẻ ra HealthSystem => tiện chỉ cần gọi thằng Interface này để lấy HealthSystem
    HealthSystem GetHealthSystem();
}
