using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BE1.Models.viewmodel
{
    public class CheckoutVM
    {
        // Danh sách các sản phẩm trong giỏ hàng
        public List<CartItem> CartItems { get; set; }

        public int CustomerID { get; set; }

        [Display(Name = "Ngày đặt hàng")]
        [DataType(DataType.Date)]
        public DateTime OrderDate { get; set; }

        [Display(Name = "Tổng giá trị")]
        [DisplayFormat(DataFormatString = "{0:N0} đ", ApplyFormatInEditMode = false)]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Trạng thái thanh toán")]
        public string PaymentStatus { get; set; }

        [Display(Name = "Phương thức thanh toán")]
        public string PaymentMethod { get; set; }

        [Display(Name = "Phương thức giao hàng")]
        public string DeliveryMethod { get; set; }

        [Display(Name = "Địa chỉ giao hàng")]
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng")]
        public string ShippingAddress { get; set; }

        public string Username { get; set; }

        // Các thuộc tính khác của đơn hàng (nếu cần chi tiết sản phẩm mua)
        public List<OrderDetail> OrderDetails { get; set; }
    }
}