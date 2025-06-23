
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;


namespace Wirin.Infrastructure.Entities;

public class UserEntity: IdentityUser
{ 
    public string FullName { get; set; }
    public virtual IEnumerable<IdentityUserRole<string>> Roles { get; set; }
}