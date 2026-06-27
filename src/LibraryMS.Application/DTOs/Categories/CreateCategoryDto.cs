namespace LibraryMS.Application.DTOs.Categories;

public class CreateCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public int? ParentCategoryId { get; set; }
}
