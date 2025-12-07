+ new idea: check check huong nhin trong khoang chase left -> chase right, neu ra ngoai roi thi khong thay player nua thi bo quay ve

+ code tấn công nhân vật và mất máu các thứ

+ 1 kinh nghiệm: file chỉ xử lý đúng theo tên fle còn các cái tham chiếu thì nên làm trong file handler => ví dụ PlayerHealthBar xử lý hiển thị thanh máu theo currentHealth của file PlayerHealthSystem => phải tham chiếu tới currentHealth để hiển thị => không tham chiếu trong PlayerHealthBar mà tham chiếu trong PlayerHealthStaminaHandler.