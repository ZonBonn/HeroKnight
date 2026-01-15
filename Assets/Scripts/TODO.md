+ new idea: check check huong nhin trong khoang chase left -> chase right, neu ra ngoai roi thi khong thay player nua thi bo quay ve

+ trang tri them mot so cho enemy (coc ban uong ruou cac thu)

+ tiếp tục làm blockedNodesHeroKnight

+ VIỆC CẦN LÀM:
<!-- + error 1: nếu Enemy trong trạng thái RTC (rồi người chơi chạy thẳng đi) rồi chuyển về patrol (có thực sự chuyển về patrol từ RTC ??) thì sao ??
+ error 1.1: (chase -> patrol): check xem có vấn đề gì ? (enemy di chuyển quay đi quay lại một chỗ)
+ error 1.2: RTC -> patrol: check xem có vấn đều gì ? (enemy di chuyển tới một vị trí nhất định thì bị kẹt ?)
+ jump xong lại thành patrol ?? đã jump thì chỉ có nhảy nhảy xong ít nhất 1 lần rồi mới chuyển thành trạng thái khác (done ???)
+ có thể là do map set up block node chưa hợp lý ??? làm tìm đường trên không (đây là game 2d platform không thể di chuyển lên xuống giống như shooting game được timg đường trên không làm nó bị không thể di chuyển lên xuống do rigidbody 2d gravity nữa => đi đi đi lại) ? => giải pháp chắc là sét block node thôi 
+ desgin map level 2 -->

+ TIẾP TỤC:
<!-- + tiếp tục phần đo kích thước nhảy để lực nhảy tùy theo chiều cao -->
+ giờ giớ hạn spawn key lại chỉ spawn theo đúng số rương cho theo từng level và tỉ lệ spawn tăng hoặc giảm tùy theo số lượng đã spawn hay chưa spawn (CONTINUE)
+ tối ưu phần spawn key của Enemy ở random Items: chỉ cần truyền tham số enemy vào là tự vứt ra list các rate (DONE)
+ cho đồ bao giờ tầm khoảng 1s - 3s sau thì player mới được nhặt (DONE)
+ mang key qua màn bên kia hoặc xóa nó đi khi sang màn mới
+ dame mỗi loại tấn công là khác nhau của player

+ 1 kinh nghiệm: file chỉ xử lý đúng theo tên fle còn các cái tham chiếu thì nên làm trong file handler => ví dụ PlayerHealthBar xử lý hiển thị thanh máu theo currentHealth của file PlayerHealthSystem => phải tham chiếu tới currentHealth để hiển thị => không tham chiếu trong PlayerHealthBar mà tham chiếu trong PlayerHealthStaminaHandler.