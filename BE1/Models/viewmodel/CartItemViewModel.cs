using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BE1.Models.viewmodel
{
    public class CartItemViewModel
    {
        public int ProductID { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }


        public decimal Total
        {
            get { return Price * Quantity; }
        }
    }
}