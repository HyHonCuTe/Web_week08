using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using vodaohuyhoang_buoi3.Models;

namespace vodaohuyhoang_buoi3.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize] 
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Products.Include(p => p.Category);
            return View(await applicationDbContext.ToListAsync());
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null) return NotFound();

            return View(product);
        }

        [Authorize(Roles = "Admin,Employee")]
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,Description,ImageUrl,CategoryId")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // ✨ Chỉ Employee và Admin mới có quyền chỉnh sửa sản phẩm
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,Description,ImageUrl,CategoryId")] Product product)
        {
            if (id != product.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // ❌ Chỉ Admin có quyền xóa
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null) return NotFound();

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }





        //[HttpGet]
        //public async Task<IActionResult> Search(string keyword, int page = 1, int pageSize = 5)
        //{
        //    // Nếu không có keyword, trả về toàn bộ sản phẩm với phân trang
        //    if (string.IsNullOrWhiteSpace(keyword))
        //    {
        //        var allProducts = await _context.Products
        //            .Include(p => p.Category)
        //            .OrderBy(p => p.Id)
        //            .Skip((page - 1) * pageSize)
        //            .Take(pageSize)
        //            .ToListAsync();

        //        var totalItems = await _context.Products.CountAsync();
        //        ViewBag.CurrentPage = page;
        //        ViewBag.PageSize = pageSize;
        //        ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
        //        ViewBag.SearchKeyword = "";
        //        ViewBag.SearchResultCount = totalItems;
        //        return View("Index", allProducts);
        //    }

        //    // Chuẩn hóa keyword: loại bỏ khoảng trắng thừa và chuyển về chữ thường
        //    var searchTerm = keyword.Trim().ToLower();
        //    // Tách thành các từ riêng lẻ nếu có khoảng trắng
        //    var searchTerms = searchTerm.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        //    // Xây dựng truy vấn cơ bản
        //    var query = _context.Products
        //        .Include(p => p.Category)
        //        .AsQueryable();

        //    // Áp dụng điều kiện tìm kiếm cho từng từ
        //    foreach (var term in searchTerms)
        //    {
        //        query = query.Where(p =>
        //            (p.Name != null && p.Name.ToLower().Contains(term)) ||
        //            (p.Description != null && p.Description.ToLower().Contains(term)));
        //    }

        //    // Đếm tổng số kết quả
        //    var totalSearchItems = await query.CountAsync();

        //    // Lấy danh sách sản phẩm với phân trang
        //    var results = await query
        //        .OrderBy(p => p.Id)
        //        .Skip((page - 1) * pageSize)
        //        .Take(pageSize)
        //        .ToListAsync();

        //    // Thiết lập thông tin cho view
        //    ViewBag.CurrentPage = page;
        //    ViewBag.PageSize = pageSize;
        //    ViewBag.TotalPages = (int)Math.Ceiling((double)totalSearchItems / pageSize);
        //    ViewBag.SearchKeyword = keyword;
        //    ViewBag.SearchResultCount = totalSearchItems;

        //    return View("Index", results);
        //}

        [HttpGet]
        public async Task<IActionResult> Search(string keyword, int page = 1, int pageSize = 5)
        {
            // Nếu không có keyword, trả về toàn bộ sản phẩm với phân trang
            if (string.IsNullOrWhiteSpace(keyword))
            {
                var allProducts = await _context.Products
                    .Include(p => p.Category)
                    .OrderBy(p => p.Id)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var totalItems = await _context.Products.CountAsync();
                ViewBag.CurrentPage = page;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
                ViewBag.SearchKeyword = "";
                ViewBag.SearchResultCount = totalItems;
                return View("Index", allProducts);
            }

            // Chuẩn hóa keyword
            var searchTerm = keyword.Trim().ToLower();
            var searchTerms = searchTerm.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // Xây dựng truy vấn
            var query = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            foreach (var term in searchTerms)
            {
                query = query.Where(p =>
                    (p.Name != null && p.Name.ToLower().Contains(term)) ||
                    (p.Description != null && p.Description.ToLower().Contains(term)));
            }

            // Đếm tổng số kết quả
            var totalSearchItems = await query.CountAsync();

            // Lấy danh sách sản phẩm
            var results = await query
                .OrderBy(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Thiết lập thông tin cho view
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalSearchItems / pageSize);
            ViewBag.SearchKeyword = keyword;
            ViewBag.SearchResultCount = totalSearchItems;

            return View("Index", results);
        }

    }
}
