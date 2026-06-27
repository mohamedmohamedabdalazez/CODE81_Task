namespace LibraryMS.Application.DTOs.Categories;

public class UpdateCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public int? ParentCategoryId { get; set; }
}
