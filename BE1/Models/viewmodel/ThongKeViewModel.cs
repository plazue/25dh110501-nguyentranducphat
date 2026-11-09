using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BE1.Models.viewmodel
{
    public class ThongKeViewModel
    {
        public string TenLoai { get; set; }
        public int SoLuong { get; set; }
        public decimal GiaCaoNhat { get; set; }
        public decimal GiaThapNhat { get; set; }
        public decimal GiaTrungBinh { get; set; }
    }
}