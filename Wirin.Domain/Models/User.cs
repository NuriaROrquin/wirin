﻿namespace Wirin.Domain.Models;

public class User
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }

    public string PhoneNumber { get; set; }
    public IEnumerable<string> Roles { get; set; }
}
