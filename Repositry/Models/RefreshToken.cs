using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class RefreshToken
{
    public string ApplicationUserId { get; set; } = null!;

    public int Id { get; set; }

    public string Token { get; set; } = null!;

    public DateTime ExpiresOn { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime? RevokedOn { get; set; }

    public virtual AspNetUser ApplicationUser { get; set; } = null!;
}
