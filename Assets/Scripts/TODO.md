+ new idea: check check huong nhin trong khoang chase left -> chase right, neu ra ngoai roi thi khong thay player nua thi bo quay ve

+ code tấn công player và mất máu stamina cua player các thứ

+ trang tri them mot so cho enemy (coc ban uong ruou cac thu)

+ đang làm knock back cho enemy, đang đoạn xử lý cuối frame của knock back thì làm gì rồi

+ 1 kinh nghiệm: file chỉ xử lý đúng theo tên fle còn các cái tham chiếu thì nên làm trong file handler => ví dụ PlayerHealthBar xử lý hiển thị thanh máu theo currentHealth của file PlayerHealthSystem => phải tham chiếu tới currentHealth để hiển thị => không tham chiếu trong PlayerHealthBar mà tham chiếu trong PlayerHealthStaminaHandler.