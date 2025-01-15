using System;
using System.Collections.Generic;

namespace dsc_backend.Models;

public partial class User
{
    public int UserId { get; set; }

    public int? RoleId { get; set; }

    public int? UserTeamId { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

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

    public virtual ICollection<CommentClub> CommentClubs { get; set; } = new List<CommentClub>();

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<RequestJoinActivity> RequestJoinActivities { get; set; } = new List<RequestJoinActivity>();

    public virtual ICollection<RequestJoinClub> RequestJoinClubs { get; set; } = new List<RequestJoinClub>();

    public virtual Role? Role { get; set; }

    public virtual ICollection<Team> Teams { get; set; } = new List<Team>();

    public virtual ICollection<Tournament> Tournaments { get; set; } = new List<Tournament>();

    public virtual ICollection<UserActivity> UserActivities { get; set; } = new List<UserActivity>();

    public virtual ICollection<UserActivityClub> UserActivityClubs { get; set; } = new List<UserActivityClub>();

    public virtual ICollection<UserClub> UserClubs { get; set; } = new List<UserClub>();

    public virtual ICollection<UserSport> UserSports { get; set; } = new List<UserSport>();

    public virtual ICollection<UserTeam> UserTeams { get; set; } = new List<UserTeam>();
}
