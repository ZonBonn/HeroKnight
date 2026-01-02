+ new idea: check check huong nhin trong khoang chase left -> chase right, neu ra ngoai roi thi khong thay player nua thi bo quay ve

+ trang tri them mot so cho enemy (coc ban uong ruou cac thu)

+ tiếp tục làm blockedNodesHeroKnight

+ VIỆC CẦN LÀM:
+ error 1: nếu Enemy trong trạng thái RTC (rồi người chơi chạy thẳng đi) rồi chuyển về patrol (có thực sự chuyển về patrol từ RTC ??) thì sao ??
+ desgin map level 2

+ TIẾP TỤC:
+ tiếp tục phần đo kích thước nhảy để lực nhảy tùy theo chiều cao

+ 1 kinh nghiệm: file chỉ xử lý đúng theo tên fle còn các cái tham chiếu thì nên làm trong file handler => ví dụ PlayerHealthBar xử lý hiển thị thanh máu theo currentHealth của file PlayerHealthSystem => phải tham chiếu tới currentHealth để hiển thị => không tham chiếu trong PlayerHealthBar mà tham chiếu trong PlayerHealthStaminaHandler.