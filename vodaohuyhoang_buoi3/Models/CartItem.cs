﻿namespace vodaohuyhoang_buoi3.Models
{
    public class CartItem
    {
        //thông tin sản phẩm
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        //lưu số lượng
        public int Quantity { get; set; }
        public decimal Total => Price * Quantity;
    }
}
