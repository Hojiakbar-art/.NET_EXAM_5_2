namespace Application.DTOs.User;

public class UpdateUserDto
{
    public string FirstName {get;set;} = string.Empty;
    public string LastName {get;set;} = string.Empty;
    public DateOnly BirthDate {get;set;}
}
