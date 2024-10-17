using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace dsc_backend.Models;

public partial class DscContext : DbContext
{
    public DscContext()
    {
    }

    public DscContext(DbContextOptions<DscContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Activity> Activities { get; set; }

    public virtual DbSet<Club> Clubs { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Level> Levels { get; set; }

    public virtual DbSet<Match> Matches { get; set; }

    public virtual DbSet<MemberClub> MemberClubs { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Result> Results { get; set; }

    public virtual DbSet<ResultOfActivity> ResultOfActivities { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Round> Rounds { get; set; }

    public virtual DbSet<Sport> Sports { get; set; }

    public virtual DbSet<Team> Teams { get; set; }

    public virtual DbSet<TeamTournament> TeamTournaments { get; set; }

    public virtual DbSet<Tournament> Tournaments { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserActivity> UserActivities { get; set; }

    public virtual DbSet<UserClub> UserClubs { get; set; }

    public virtual DbSet<UserSport> UserSports { get; set; }

    public virtual DbSet<UserTeam> UserTeams { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:Database");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Activity>(entity =>
        {
            entity.HasKey(e => e.ActivityId).HasName("PK__Activiti__45F4A7F197C91A67");

            entity.Property(e => e.ActivityId).HasColumnName("ActivityID");
            entity.Property(e => e.ActivityName).HasMaxLength(255);
            entity.Property(e => e.Expense).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.LevelId).HasColumnName("LevelID");
            entity.Property(e => e.Location).HasMaxLength(255);
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Level).WithMany(p => p.Activities)
                .HasForeignKey(d => d.LevelId)
                .HasConstraintName("FK_Activities_Level");

            entity.HasOne(d => d.User).WithMany(p => p.Activities)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Activities_User");
        });

        modelBuilder.Entity<Club>(entity =>
        {
            entity.HasKey(e => e.ClubId).HasName("PK__Club__D35058C7A1E45342");

            entity.ToTable("Club");

            entity.Property(e => e.ClubId).HasColumnName("ClubID");
            entity.Property(e => e.ClubName).HasMaxLength(255);
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.LevelId).HasColumnName("LevelID");
            entity.Property(e => e.MemberId).HasColumnName("MemberID");
            entity.Property(e => e.SportId).HasColumnName("SportID");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Level).WithMany(p => p.Clubs)
                .HasForeignKey(d => d.LevelId)
                .HasConstraintName("FK_Club_Level");

            entity.HasOne(d => d.Sport).WithMany(p => p.Clubs)
                .HasForeignKey(d => d.SportId)
                .HasConstraintName("FK_Club_Sport");

            entity.HasOne(d => d.User).WithMany(p => p.Clubs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Club_User");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__Comment__C3B4DFAA8C041D17");

            entity.ToTable("Comment");

            entity.Property(e => e.CommentId).HasColumnName("CommentID");
            entity.Property(e => e.ActivityId).HasColumnName("ActivityID");
            entity.Property(e => e.Comment1).HasColumnName("Comment");
            entity.Property(e => e.Image).HasMaxLength(255);
            entity.Property(e => e.TournamentId).HasColumnName("TournamentID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Activity).WithMany(p => p.Comments)
                .HasForeignKey(d => d.ActivityId)
                .HasConstraintName("FK_Comment_Activity");

            entity.HasOne(d => d.Tournament).WithMany(p => p.Comments)
                .HasForeignKey(d => d.TournamentId)
                .HasConstraintName("FK_Comment_Tournament");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Comment_User");
        });

        modelBuilder.Entity<Level>(entity =>
        {
            entity.HasKey(e => e.LevelId).HasName("PK__Level__09F03C06D4413247");

            entity.ToTable("Level");

            entity.Property(e => e.LevelId).HasColumnName("LevelID");
            entity.Property(e => e.LevelName).HasMaxLength(100);
        });

        modelBuilder.Entity<Match>(entity =>
        {
            entity.HasKey(e => e.MatchId).HasName("PK__Match__4218C83773A97489");

            entity.ToTable("Match");

            entity.Property(e => e.MatchId).HasColumnName("MatchID");
            entity.Property(e => e.RoundId).HasColumnName("RoundID");
            entity.Property(e => e.Team1Id).HasColumnName("Team1ID");
            entity.Property(e => e.Team2Id).HasColumnName("Team2ID");

            entity.HasOne(d => d.Round).WithMany(p => p.Matches)
                .HasForeignKey(d => d.RoundId)
                .HasConstraintName("FK_Match_Round");

            entity.HasOne(d => d.Team1).WithMany(p => p.MatchTeam1s)
                .HasForeignKey(d => d.Team1Id)
                .HasConstraintName("FK_Match_Team1");

            entity.HasOne(d => d.Team2).WithMany(p => p.MatchTeam2s)
                .HasForeignKey(d => d.Team2Id)
                .HasConstraintName("FK_Match_Team2");
        });

        modelBuilder.Entity<MemberClub>(entity =>
        {
            entity.HasKey(e => new { e.MemberId, e.ClubId }).HasName("PK__MemberCl__61C54EB466C56C6D");

            entity.Property(e => e.MemberId).HasColumnName("MemberID");
            entity.Property(e => e.ClubId).HasColumnName("ClubID");
            entity.Property(e => e.JoinDate).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Club).WithMany(p => p.MemberClubs)
                .HasForeignKey(d => d.ClubId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MemberClubs_Club");

            entity.HasOne(d => d.User).WithMany(p => p.MemberClubs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_MemberClubs_User");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E328B765A91");

            entity.ToTable("Notification");

            entity.Property(e => e.NotificationId).HasColumnName("NotificationID");
            entity.Property(e => e.ActivityId).HasColumnName("ActivityID");
            entity.Property(e => e.TournamentId).HasColumnName("TournamentID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Activity).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.ActivityId)
                .HasConstraintName("FK_Notification_Activity");

            entity.HasOne(d => d.Tournament).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.TournamentId)
                .HasConstraintName("FK_Notification_Tournament");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Notification_User");
        });

        modelBuilder.Entity<Result>(entity =>
        {
            entity.HasKey(e => e.ResultId).HasName("PK__Result__9769022802496FC0");

            entity.ToTable("Result");

            entity.Property(e => e.ResultId).HasColumnName("ResultID");
            entity.Property(e => e.MatchId).HasColumnName("MatchID");

            entity.HasOne(d => d.Match).WithMany(p => p.Results)
                .HasForeignKey(d => d.MatchId)
                .HasConstraintName("FK_Result_Match");
        });

        modelBuilder.Entity<ResultOfActivity>(entity =>
        {
            entity.HasKey(e => e.ResultId).HasName("PK__ResultOf__97690228A96B7636");

            entity.Property(e => e.ResultId).HasColumnName("ResultID");
            entity.Property(e => e.ActivityId).HasColumnName("ActivityID");

            entity.HasOne(d => d.Activity).WithMany(p => p.ResultOfActivities)
                .HasForeignKey(d => d.ActivityId)
                .HasConstraintName("FK_ResultOfActivities_Activity");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE3A82F32E17");

            entity.ToTable("Role");

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<Round>(entity =>
        {
            entity.HasKey(e => e.RoundId).HasName("PK__Round__94D84E1A0D036E1B");

            entity.ToTable("Round");

            entity.Property(e => e.RoundId).HasColumnName("RoundID");
            entity.Property(e => e.TournamentId).HasColumnName("TournamentID");

            entity.HasOne(d => d.Tournament).WithMany(p => p.Rounds)
                .HasForeignKey(d => d.TournamentId)
                .HasConstraintName("FK_Round_Tournament");
        });

        modelBuilder.Entity<Sport>(entity =>
        {
            entity.HasKey(e => e.SportId).HasName("PK__Sport__7A41AF1C39FF4A8D");

            entity.ToTable("Sport");

            entity.Property(e => e.SportId).HasColumnName("SportID");
            entity.Property(e => e.Position).HasMaxLength(100);
            entity.Property(e => e.SportName).HasMaxLength(100);
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(e => e.TeamId).HasName("PK__Team__123AE7B9509B8F72");

            entity.ToTable("Team");

            entity.Property(e => e.TeamId).HasColumnName("TeamID");
            entity.Property(e => e.TeamName).HasMaxLength(255);
            entity.Property(e => e.TournamentId).HasColumnName("TournamentID");
            entity.Property(e => e.UserTeamId).HasColumnName("UserTeamID");

            entity.HasOne(d => d.Tournament).WithMany(p => p.Teams)
                .HasForeignKey(d => d.TournamentId)
                .HasConstraintName("FK_Team_Tournament");
        });

        modelBuilder.Entity<TeamTournament>(entity =>
        {
            entity.HasKey(e => e.TeamTournamentId).HasName("PK__TeamTour__7B13E8608AE38152");

            entity.ToTable("TeamTournament");

            entity.Property(e => e.TeamTournamentId).HasColumnName("TeamTournamentID");
            entity.Property(e => e.TeamId).HasColumnName("TeamID");
            entity.Property(e => e.TournamentId).HasColumnName("TournamentID");

            entity.HasOne(d => d.Team).WithMany(p => p.TeamTournaments)
                .HasForeignKey(d => d.TeamId)
                .HasConstraintName("FK_TeamTournament_Team");

            entity.HasOne(d => d.Tournament).WithMany(p => p.TeamTournaments)
                .HasForeignKey(d => d.TournamentId)
                .HasConstraintName("FK_TeamTournament_Tournament");
        });

        modelBuilder.Entity<Tournament>(entity =>
        {
            entity.HasKey(e => e.TournamentId).HasName("PK__Tourname__AC631333D7E21B87");

            entity.Property(e => e.TournamentId).HasColumnName("TournamentID");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.LevelId).HasColumnName("LevelID");
            entity.Property(e => e.Location).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Level).WithMany(p => p.Tournaments)
                .HasForeignKey(d => d.LevelId)
                .HasConstraintName("FK_Tournaments_Level");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CCACE1C4905E");

            entity.ToTable("User");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.AccountId).HasColumnName("AccountID");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.Avatar).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(255);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.Height).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.MemberId).HasColumnName("MemberID");
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UserTeamId).HasColumnName("UserTeamID");
            entity.Property(e => e.Weight).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_User_Role");
        });

        modelBuilder.Entity<UserActivity>(entity =>
        {
            entity.HasKey(e => e.UserActivityId).HasName("PK__UserActi__82560463CBCD2B73");

            entity.Property(e => e.UserActivityId).HasColumnName("UserActivityID");
            entity.Property(e => e.ActivityId).HasColumnName("ActivityID");
            entity.Property(e => e.JoinDate).HasColumnType("datetime");
            entity.Property(e => e.RoleInActivity).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Activity).WithMany(p => p.UserActivities)
                .HasForeignKey(d => d.ActivityId)
                .HasConstraintName("FK_UserActivities_Activity");

            entity.HasOne(d => d.User).WithMany(p => p.UserActivities)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UserActivities_User");
        });

        modelBuilder.Entity<UserClub>(entity =>
        {
            entity.HasKey(e => e.UserClubId).HasName("PK__UserClub__9BFD3C25C85C183D");

            entity.ToTable("UserClub");

            entity.Property(e => e.UserClubId).HasColumnName("UserClubID");
            entity.Property(e => e.ClubId).HasColumnName("ClubID");
            entity.Property(e => e.Role).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Club).WithMany(p => p.UserClubs)
                .HasForeignKey(d => d.ClubId)
                .HasConstraintName("FK_UserClub_Club");

            entity.HasOne(d => d.User).WithMany(p => p.UserClubs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UserClub_User");
        });

        modelBuilder.Entity<UserSport>(entity =>
        {
            entity.HasKey(e => e.UserSportId).HasName("PK__UserSpor__A6A5EE9B7EBC77D4");

            entity.ToTable("UserSport");

            entity.Property(e => e.UserSportId).HasColumnName("UserSportID");
            entity.Property(e => e.LevelId).HasColumnName("LevelID");
            entity.Property(e => e.SportId).HasColumnName("SportID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Level).WithMany(p => p.UserSports)
                .HasForeignKey(d => d.LevelId)
                .HasConstraintName("FK_UserSport_Level");

            entity.HasOne(d => d.Sport).WithMany(p => p.UserSports)
                .HasForeignKey(d => d.SportId)
                .HasConstraintName("FK_UserSport_Sport");

            entity.HasOne(d => d.User).WithMany(p => p.UserSports)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UserSport_User");
        });

        modelBuilder.Entity<UserTeam>(entity =>
        {
            entity.HasKey(e => e.UserTeamId).HasName("PK__UserTeam__9ADF8092A296D54B");

            entity.ToTable("UserTeam");

            entity.Property(e => e.UserTeamId).HasColumnName("UserTeamID");
            entity.Property(e => e.TeamId).HasColumnName("TeamID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Team).WithMany(p => p.UserTeams)
                .HasForeignKey(d => d.TeamId)
                .HasConstraintName("FK_UserTeam_Team");

            entity.HasOne(d => d.User).WithMany(p => p.UserTeams)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UserTeam_User");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
