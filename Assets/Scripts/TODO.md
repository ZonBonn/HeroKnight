+ new idea: check check huong nhin trong khoang chase left -> chase right, neu ra ngoai roi thi khong thay player nua thi bo quay ve

+ trang tri them mot so cho enemy (coc ban uong ruou cac thu)

+ đang làm knock back cho player khi mà đỡ được đòn từ enemy, xử lý theo cách Action phần PlayerHealthSystem và StaminaSystem -> tiếp tục tại đây <-, làm die các thứ nữa (lỗi đôi khi đang đỡ thì đứng im xong rơi từ từ chậm xuống ?)

+ 1 kinh nghiệm: file chỉ xử lý đúng theo tên fle còn các cái tham chiếu thì nên làm trong file handler => ví dụ PlayerHealthBar xử lý hiển thị thanh máu theo currentHealth của file PlayerHealthSystem => phải tham chiếu tới currentHealth để hiển thị => không tham chiếu trong PlayerHealthBar mà tham chiếu trong PlayerHealthStaminaHandler.