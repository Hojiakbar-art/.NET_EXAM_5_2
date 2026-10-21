namespace Application.DTOs.Categories;

public class UpdateCategoryDto
{
    public int Id{get;set;}
    public string Name{get;set;} = string.Empty;
    public string? Description{get;set;}

}