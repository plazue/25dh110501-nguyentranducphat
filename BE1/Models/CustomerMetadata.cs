using System.ComponentModel.DataAnnotations;
using System.Web;

namespace BE1.Models
{
    [MetadataType(typeof(CustomerMetadata))]
    public partial class Customer
    {
        [DataType(DataType.Upload)]
        [Display(Name = "Ảnh đại diện")]
        public HttpPostedFileBase UploadImg { get; set; }
    }

    public class CustomerMetadata
    {
        [Key]
        public int CustomerID { get; set; }

        [Required(ErrorMessage = "Tên khách hàng không được để trống.")]
        [Display(Name = "Họ tên")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống.")]
        [RegularExpression(@"^[0-9]{10,11}$", ErrorMessage = "Số điện thoại không hợp lệ (phải từ 10-11 chữ số và không chứa khoảng trắng).")]
        [Display(Name = "Số điện thoại")]
        public string CustomerPhone { get; set; }

        [Required(ErrorMessage = "Email không được để trống.")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
        [RegularExpression(@"^\S+$", ErrorMessage = "Email không được chứa khoảng trắng.")]
        [Display(Name = "Email")]
        public string CustomerEmail { get; set; }

        [Display(Name = "Địa chỉ")]
        public string CustomerAddress { get; set; }

        [Required(ErrorMessage = "Tên đăng nhập không được để trống.")]
        [StringLength(50, ErrorMessage = "Tên đăng nhập không được vượt quá 50 ký tự.")]
        [RegularExpression(@"^[a-zA-Z0-9_.-]+$", ErrorMessage = "Tên đăng nhập chỉ chứa chữ cái, số, dấu gạch dưới, dấu chấm và không được có khoảng trắng.")]
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^\S+$", ErrorMessage = "Mật khẩu không được chứa khoảng trắng.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn quyền.")]
        [Display(Name = "Quyền (Role)")]
        public string Role { get; set; }

        [Display(Name = "Ảnh đại diện")]
        public string CustomerImage { get; set; }

        [Display(Name = "Trạng thái")]
        public bool? IsActive { get; set; }
    }
}