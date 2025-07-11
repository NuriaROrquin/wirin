﻿namespace Wirin.Domain.Dtos.User;

public class UserWithRolesDto
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public IEnumerable<string> Roles { get; set; }
}
