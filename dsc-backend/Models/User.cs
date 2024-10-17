using System;
using System.Collections.Generic;

namespace dsc_backend.Models;

public partial class User
{
    public int UserId { get; set; }

    public int? RoleId { get; set; }

    public int? AccountId { get; set; }

    public int? UserTeamId { get; set; }

    public int? MemberId { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? FullName { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public string? Gender { get; set; }

    public int? Age { get; set; }

    public decimal? Height { get; set; }

    public decimal? Weight { get; set; }

    public string? Avatar { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<Activity> Activities { get; set; } = new List<Activity>();

    public virtual ICollection<Club> Clubs { get; set; } = new List<Club>();

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<MemberClub> MemberClubs { get; set; } = new List<MemberClub>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual Role? Role { get; set; }

    public virtual ICollection<UserActivity> UserActivities { get; set; } = new List<UserActivity>();

    public virtual ICollection<UserClub> UserClubs { get; set; } = new List<UserClub>();

    public virtual ICollection<UserSport> UserSports { get; set; } = new List<UserSport>();

    public virtual ICollection<UserTeam> UserTeams { get; set; } = new List<UserTeam>();
}
