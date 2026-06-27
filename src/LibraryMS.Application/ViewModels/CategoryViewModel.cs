namespace LibraryMS.Application.ViewModels;

public class CategoryViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? ParentCategoryId { get; set; }
    public string? ParentCategoryName { get; set; }
    public List<CategoryViewModel> SubCategories { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}
