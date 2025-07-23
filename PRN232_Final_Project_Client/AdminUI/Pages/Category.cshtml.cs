using DTOs.CategoryDTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service.Interfaces;
using Service.Interfaces;
using Service.Services;
using System.Net.Http;

namespace PRN232_Final_Project_Client.Pages.Categories
{
    public class CategoryModel : PageModel
    {
        private readonly ICategoryService _categoryService;

        public CategoryModel(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public List<ReadCategoryDTO> Categories { get; set; } = new();



        [BindProperty]
        public UpdateCategoryDTO EditCategory { get; set; } = new();

        public async Task OnGetAsync()
        {


        }

        public async Task<IActionResult> OnGetAllCategoryAsync()
        {
            var Categories = await _categoryService.GetAllCategoriesAsync();
            return new JsonResult(Categories);
        }

        [BindProperty]
        public CreateCategoryDTO NewCategory { get; set; } = new();
        public async Task<IActionResult> OnPostCreateAsync()
        {
            // Debug trước khi validate
            Console.WriteLine($"NewCategory.CategoryName: '{NewCategory?.CategoryName}'");
            Console.WriteLine($"NewCategory.Description: '{NewCategory?.Description}'");
            Console.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");

            // In ra tất cả ModelState entries
            foreach (var ms in ModelState)
            {
                Console.WriteLine($"Key: {ms.Key}, Valid: {ms.Value.ValidationState}");
            }

            if (!ModelState.IsValid)
            {
                return await ReloadPage();
            }

            var token = HttpContext.Session.GetString("AdminToken");
            var result = await _categoryService.CreateCategoryAsync(NewCategory, token);

            if (!result)
            {
                ModelState.AddModelError(string.Empty, "Failed to create category.");
                return await ReloadPage();
            }

            return RedirectToPage();
        }

        [BindProperty]
        public int CategoryID { get; set; }

        [BindProperty]
        public string CategoryName { get; set; } = string.Empty;

        [BindProperty]
        public string? Description { get; set; }

        public async Task<IActionResult> OnPostUpdateAsync()
        {
            Console.WriteLine($"CategoryID: {CategoryID}");
            Console.WriteLine($"CategoryName: {CategoryName}");
            Console.WriteLine($"Description: {Description}");

            if (!ModelState.IsValid)
            {
                return await ReloadPage();
            }

            var token = HttpContext.Session.GetString("AdminToken");

            var dto = new UpdateCategoryDTO
            {
                CategoryID = CategoryID,
                CategoryName = CategoryName,
                Description = Description ?? string.Empty
            };

            var result = await _categoryService.UpdateCategoryAsync(CategoryID, dto, token);

            if (!result)
            {
                ModelState.AddModelError(string.Empty, "Failed to update category.");
                return await ReloadPage();
            }

            return RedirectToPage("/Category");
        }



        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            ModelState.Clear();
            var token = HttpContext.Session.GetString("AdminToken");
            var result = await _categoryService.DeleteCategoryAsync(id, token);

            if (!result)
            {
                ModelState.AddModelError(string.Empty, "Failed to delete category.");
                return await ReloadPage();

            }

            return RedirectToPage("/Category");
        }

        private async Task<IActionResult> ReloadPage()
        {
            Categories = await _categoryService.GetAllCategoriesAsync();
            return Page();
        }

        public async Task<JsonResult> OnGetSearchAsync(string searchTerm, int pagenumber)
        {
            try
            {
                int pageSize = 5;
                int skip = (pagenumber - 1) * pageSize;



                // Nếu cần filter searchTerm thì filter ở đây (lấy list full rồi filter)
                var filtered = await _categoryService.SearchCategoriesOdataAsync(searchTerm, 0, 0);

                int totalCount = (int)Math.Ceiling(filtered.Count() / 5f);

                // Lấy phân trang từ list filtered
                var pagedCategories = await _categoryService.SearchCategoriesOdataAsync(searchTerm, 5, skip);

                return new JsonResult(new
                {
                    categories = pagedCategories,
                    totalCount = totalCount
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { error = "Lỗi khi tìm kiếm danh mục." }) { StatusCode = 500 };
            }
        }






    }
}