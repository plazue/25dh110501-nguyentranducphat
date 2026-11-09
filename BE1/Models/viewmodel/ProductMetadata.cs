using System;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace BE1.Models
{
    // Liên kết với class Product được sinh ra từ Entity Framework
    [MetadataType(typeof(ProductMetadata))]
    public partial class Product
    {
        // Thuộc tính tạm để nhận file upload từ form (không lưu vào DB trực tiếp)
        [DataType(DataType.Upload)]
        [Display(Name = "Ảnh sản phẩm")]
        public HttpPostedFileBase UploadImg { get; set; }
    }

    public class ProductMetadata
    {
        [Key]
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn danh mục sản phẩm.")]
        [Display(Name = "Danh mục")]
        public int CategoryID { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm không được để trống.")]
        [StringLength(255, ErrorMessage = "Tên sản phẩm không được vượt quá 255 ký tự.")]
        [Display(Name = "Tên sản phẩm")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Mô tả sản phẩm không được để trống.")]
        [Display(Name = "Mô tả sản phẩm")]
        public string ProductDecription { get; set; }

        [Required(ErrorMessage = "Giá sản phẩm không được để trống.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá sản phẩm phải lớn hơn 0.")]
        [Display(Name = "Giá sản phẩm")]
        public decimal ProductPrice { get; set; }

        [Required(ErrorMessage = "Số lượng tồn kho không được để trống.")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn kho phải lớn hơn hoặc bằng 0.")]
        [Display(Name = "Số lượng tồn kho")]
        public int Quantity { get; set; }

        [Display(Name = "Ảnh sản phẩm")]
        public string ProductImage { get; set; }
    }
}