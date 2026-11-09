using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList; // Đảm bảo đã using PagedList

namespace BE1.Models.viewmodel
{
    public class ProductDetailsVM
    {
        public Product product { get; set; }
        public int quantity { get; set; } = 1;

        // Tính giá trị tạm tính tự động
        public decimal estimatedValue
        {
            get
            {
                return product != null ? product.ProductPrice * quantity : 0;
            }
        }

        // Các thuộc tính hỗ trợ phân trang
        public int PageNumber { get; set; }
        public int PageSize { get; set; } = 3;

        // Danh sách sản phẩm cùng danh mục (dùng IPagedList để hỗ trợ phân trang chuẩn)
        public IPagedList<Product> RelatedProducts { get; set; }

        // Danh sách sản phẩm bán chạy nhất cùng danh mục
        public IPagedList<Product> TopProducts { get; set; }
    }
}