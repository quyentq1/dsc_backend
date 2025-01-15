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

    public virtual DbSet<ActivitiesClub> ActivitiesClubs { get; set; }

    public virtual DbSet<Activity> Activities { get; set; }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Club> Clubs { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<CommentClub> CommentClubs { get; set; }

    public virtual DbSet<Fee> Fees { get; set; }

    public virtual DbSet<Level> Levels { get; set; }

    public virtual DbSet<Match> Matches { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<RequestJoinActivity> RequestJoinActivities { get; set; }

    public virtual DbSet<RequestJoinClub> RequestJoinClubs { get; set; }

    public virtual DbSet<Result> Results { get; set; }

    public virtual DbSet<ResultOfActivitiesClub> ResultOfActivitiesClubs { get; set; }

    public virtual DbSet<ResultOfActivity> ResultOfActivities { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Round> Rounds { get; set; }

    public virtual DbSet<Sport> Sports { get; set; }

    public virtual DbSet<Team> Teams { get; set; }

    public virtual DbSet<TeamTournament> TeamTournaments { get; set; }

    public virtual DbSet<Tournament> Tournaments { get; set; }

    public virtual DbSet<TransferHistory> TransferHistories { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserActivity> UserActivities { get; set; }

    public virtual DbSet<UserActivityClub> UserActivityClubs { get; set; }

    public virtual DbSet<UserClub> UserClubs { get; set; }

    public virtual DbSet<UserSport> UserSports { get; set; }

    public virtual DbSet<UserTeam> UserTeams { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:Database");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ActivitiesClub>(entity =>
        {
            entity.HasKey(e => e.ActivityClubId).HasName("PK__Activiti__140C4775A9591FFD");

            entity.ToTable("ActivitiesClub");

            entity.Property(e => e.ActivityClubId).HasColumnName("ActivityClubID");
            entity.Property(e => e.ActivityName).HasMaxLength(255);
            entity.Property(e => e.Avatar).HasMaxLength(255);
            entity.Property(e => e.ClubId).HasColumnName("ClubID");
            entity.Property(e => e.Expense).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.LevelId).HasColumnName("LevelID");
            entity.Property(e => e.Location).HasMaxLength(255);
            entity.Property(e => e.StartDate).HasColumnType("datetime");

            entity.HasOne(d => d.Club).WithMany(p => p.ActivitiesClubs)
                .HasForeignKey(d => d.ClubId)
                .HasConstraintName("FK_Activities_Club");

            entity.HasOne(d => d.Level).WithMany(p => p.ActivitiesClubs)
                .HasForeignKey(d => d.LevelId)
                .HasConstraintName("FK_Activities_Level_Club");
        });

        modelBuilder.Entity<Activity>(entity =>
        {
            entity.HasKey(e => e.ActivityId).HasName("PK__Activiti__45F4A7F1567643F0");

            entity.Property(e => e.ActivityId).HasColumnName("ActivityID");
            entity.Property(e => e.ActivityName).HasMaxLength(255);
            entity.Property(e => e.Avatar).HasMaxLength(255);
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

        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.AdminId).HasName("PK__Admin__719FE4E8463647CA");

            entity.ToTable("Admin");

            entity.Property(e => e.AdminId).HasColumnName("AdminID");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Fund).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");

            entity.HasOne(d => d.Role).WithMany(p => p.Admins)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_Admin_Role");
        });

        modelBuilder.Entity<Club>(entity =>
        {
            entity.HasKey(e => e.ClubId).HasName("PK__Club__D35058C7AD2C14E6");

            entity.ToTable("Club");

            entity.Property(e => e.ClubId).HasColumnName("ClubID");
            entity.Property(e => e.Avatar).HasMaxLength(255);
            entity.Property(e => e.ClubName).HasMaxLength(255);
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.Fund).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.LevelId).HasColumnName("LevelID");
            entity.Property(e => e.SportId).HasColumnName("SportID");
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Level).WithMany(p => p.Clubs)
                .HasForeignKey(d => d.LevelId)
                .HasConstraintName("FK_Club_Level");

            entity.HasOne(d => d.Sport).WithMany(p => p.Clubs)
                .HasForeignKey(d => d.SportId)
                .HasConstraintName("FK_Club_Sport");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__Comment__C3B4DFAAFC8A5A21");

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

        modelBuilder.Entity<CommentClub>(entity =>
        {
            entity.HasKey(e => e.CommentClubId).HasName("PK__CommentC__A6C6B7F1DAE90B1D");

            entity.ToTable("CommentClub");

            entity.Property(e => e.CommentClubId).HasColumnName("CommentClubID");
            entity.Property(e => e.ActivityClubId).HasColumnName("ActivityClubID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.ActivityClub).WithMany(p => p.CommentClubs)
                .HasForeignKey(d => d.ActivityClubId)
                .HasConstraintName("FK_CommentClub_ActivityClub");

            entity.HasOne(d => d.User).WithMany(p => p.CommentClubs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_CommentClub_User");
        });

        modelBuilder.Entity<Fee>(entity =>
        {
            entity.HasKey(e => e.FeeId).HasName("PK__Fee__B387B20926392934");

            entity.ToTable("Fee");

            entity.Property(e => e.FeeId).HasColumnName("FeeID");
            entity.Property(e => e.RequestFee).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.TournamentId).HasColumnName("TournamentID");

            entity.HasOne(d => d.Tournament).WithMany(p => p.Fees)
                .HasForeignKey(d => d.TournamentId)
                .HasConstraintName("FK_Fee_Tournament");
        });

        modelBuilder.Entity<Level>(entity =>
        {
            entity.HasKey(e => e.LevelId).HasName("PK__Level__09F03C067F407273");

            entity.ToTable("Level");

            entity.Property(e => e.LevelId).HasColumnName("LevelID");
            entity.Property(e => e.LevelName).HasMaxLength(100);
        });

        modelBuilder.Entity<Match>(entity =>
        {
            entity.HasKey(e => e.MatchId).HasName("PK__Match__4218C837D84A9B1C");

            entity.ToTable("Match");

            entity.Property(e => e.MatchId).HasColumnName("MatchID");
            entity.Property(e => e.Location).HasMaxLength(255);
            entity.Property(e => e.RoundId).HasColumnName("RoundID");
            entity.Property(e => e.Team1Id).HasColumnName("Team1ID");
            entity.Property(e => e.Team2Id).HasColumnName("Team2ID");
            entity.Property(e => e.Time).HasColumnType("datetime");

            entity.HasOne(d => d.Round).WithMany(p => p.Matches)
                .HasForeignKey(d => d.RoundId)
                .HasConstraintName("FK_Match_Round");

            entity.HasOne(d => d.Team1).WithMany(p => p.MatchTeam1s)
                .HasForeignKey(d => d.Team1Id)
                .HasConstraintName("FK_Match_Team1");

            entity.HasOne(d => d.Team2).WithMany(p => p.MatchTeam2s)
                .HasForeignKey(d => d.Team2Id)
                .HasConstraintName("FK_Match_Team2");

            entity.HasOne(d => d.Tournament).WithMany(p => p.Matches)
                .HasForeignKey(d => d.TournamentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Match_Tournaments");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E327E71AAF7");

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

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payment__9B556A58546E577A");

            entity.ToTable("Payment");

            entity.Property(e => e.PaymentId).HasColumnName("PaymentID");
            entity.Property(e => e.ClubId).HasColumnName("ClubID");
            entity.Property(e => e.PaymentDate).HasColumnType("datetime");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.TournamentId).HasColumnName("TournamentID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Club).WithMany(p => p.Payments)
                .HasForeignKey(d => d.ClubId)
                .HasConstraintName("FK_Payment_Club");

            entity.HasOne(d => d.Tournament).WithMany(p => p.Payments)
                .HasForeignKey(d => d.TournamentId)
                .HasConstraintName("FK_Payment_Tournament");

            entity.HasOne(d => d.User).WithMany(p => p.Payments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Payment_User");
        });

        modelBuilder.Entity<RequestJoinActivity>(entity =>
        {
            entity.HasKey(e => e.RequestJoinActivityId).HasName("PK__requestJ__7ABA807479079D92");

            entity.ToTable("requestJoinActivity");

            entity.Property(e => e.RequestJoinActivityId).HasColumnName("requestJoinActivityID");
            entity.Property(e => e.ActivitiesId).HasColumnName("ActivitiesID");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Activities).WithMany(p => p.RequestJoinActivities)
                .HasForeignKey(d => d.ActivitiesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_requestJoinActivity_Activity");

            entity.HasOne(d => d.User).WithMany(p => p.RequestJoinActivities)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_requestJoinActivity_User");
        });

        modelBuilder.Entity<RequestJoinClub>(entity =>
        {
            entity.HasKey(e => e.RequestClubId).HasName("PK__RequestJ__7E4BEF017FFF8271");

            entity.ToTable("RequestJoinClub");

            entity.Property(e => e.ClubId).HasColumnName("Club_id");
            entity.Property(e => e.Createdate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UserId).HasColumnName("User_id");

            entity.HasOne(d => d.Club).WithMany(p => p.RequestJoinClubs)
                .HasForeignKey(d => d.ClubId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RequestJoinClub_Club");

            entity.HasOne(d => d.User).WithMany(p => p.RequestJoinClubs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RequestJoinClub_User");
        });

        modelBuilder.Entity<Result>(entity =>
        {
            entity.HasKey(e => e.ResultId).HasName("PK__Result__976902281C48B89C");

            entity.ToTable("Result");

            entity.Property(e => e.ResultId).HasColumnName("ResultID");
            entity.Property(e => e.MatchId).HasColumnName("MatchID");

            entity.HasOne(d => d.Match).WithMany(p => p.Results)
                .HasForeignKey(d => d.MatchId)
                .HasConstraintName("FK_Result_Match");
        });

        modelBuilder.Entity<ResultOfActivitiesClub>(entity =>
        {
            entity.HasKey(e => e.ResultId).HasName("PK__ResultOf__976902286446F8C5");

            entity.ToTable("ResultOfActivitiesClub");

            entity.Property(e => e.ResultId).HasColumnName("ResultID");
            entity.Property(e => e.ActivityClubId).HasColumnName("ActivityClubID");

            entity.HasOne(d => d.ActivityClub).WithMany(p => p.ResultOfActivitiesClubs)
                .HasForeignKey(d => d.ActivityClubId)
                .HasConstraintName("FK_ResultOfActivities_ActivityClub");
        });

        modelBuilder.Entity<ResultOfActivity>(entity =>
        {
            entity.HasKey(e => e.ResultId).HasName("PK__ResultOf__976902283E28A99F");

            entity.Property(e => e.ResultId).HasColumnName("ResultID");
            entity.Property(e => e.ActivityId).HasColumnName("ActivityID");

            entity.HasOne(d => d.Activity).WithMany(p => p.ResultOfActivities)
                .HasForeignKey(d => d.ActivityId)
                .HasConstraintName("FK_ResultOfActivities_Activity");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE3A21B2AFE2");

            entity.ToTable("Role");

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<Round>(entity =>
        {
            entity.HasKey(e => e.RoundId).HasName("PK__Round__94D84E1A461BAB67");

            entity.ToTable("Round");

            entity.Property(e => e.RoundId).HasColumnName("RoundID");
            entity.Property(e => e.TournamentId).HasColumnName("TournamentID");

            entity.HasOne(d => d.Tournament).WithMany(p => p.Rounds)
                .HasForeignKey(d => d.TournamentId)
                .HasConstraintName("FK_Round_Tournament");
        });

        modelBuilder.Entity<Sport>(entity =>
        {
            entity.HasKey(e => e.SportId).HasName("PK__Sport__7A41AF1C0869BD7D");

            entity.ToTable("Sport");

            entity.Property(e => e.SportId).HasColumnName("SportID");
            entity.Property(e => e.Avatar).HasMaxLength(255);
            entity.Property(e => e.SportName).HasMaxLength(100);
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(e => e.TeamId).HasName("PK__Team__123AE7B901BD106A");

            entity.ToTable("Team");

            entity.Property(e => e.TeamId).HasColumnName("TeamID");
            entity.Property(e => e.Avatar).HasMaxLength(255);
            entity.Property(e => e.TeamName).HasMaxLength(255);
            entity.Property(e => e.TournamentId).HasColumnName("TournamentID");

            entity.HasOne(d => d.Tournament).WithMany(p => p.Teams)
                .HasForeignKey(d => d.TournamentId)
                .HasConstraintName("FK_Team_Tournament");

            entity.HasOne(d => d.User).WithMany(p => p.Teams)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Team_User");
        });

        modelBuilder.Entity<TeamTournament>(entity =>
        {
            entity.ToTable("TeamTournament");

            entity.Property(e => e.TeamTournamentId).HasColumnName("TeamTournamentID");
            entity.Property(e => e.NamePlayer).HasMaxLength(100);
            entity.Property(e => e.TeamId).HasColumnName("TeamID");

            entity.HasOne(d => d.Team).WithMany(p => p.TeamTournaments)
                .HasForeignKey(d => d.TeamId)
                .HasConstraintName("FK_TeamTournament_Team");
        });

        modelBuilder.Entity<Tournament>(entity =>
        {
            entity.HasKey(e => e.TournamentId).HasName("PK__Tourname__AC6313330042E623");

            entity.Property(e => e.TournamentId).HasColumnName("TournamentID");
            entity.Property(e => e.Avatar).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.LevelId).HasColumnName("LevelID");
            entity.Property(e => e.LimitRegister).HasColumnType("datetime");
            entity.Property(e => e.Location).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.TournamentType).HasMaxLength(25);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Level).WithMany(p => p.Tournaments)
                .HasForeignKey(d => d.LevelId)
                .HasConstraintName("FK_Tournaments_Level");

            entity.HasOne(d => d.User).WithMany(p => p.Tournaments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Tournaments_User");
        });

        modelBuilder.Entity<TransferHistory>(entity =>
        {
            entity.HasKey(e => e.TransferId).HasName("PK__Transfer__95490171E7C89053");

            entity.ToTable("TransferHistory");

            entity.Property(e => e.TransferId).HasColumnName("TransferID");
            entity.Property(e => e.ReceiverAdminId).HasColumnName("ReceiverAdminID");
            entity.Property(e => e.ReceiverClubId).HasColumnName("ReceiverClubID");
            entity.Property(e => e.SenderAdminId).HasColumnName("SenderAdminID");
            entity.Property(e => e.SenderClubId).HasColumnName("SenderClubID");
            entity.Property(e => e.TransferAmount).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.TransferDate).HasColumnType("datetime");

            entity.HasOne(d => d.ReceiverAdmin).WithMany(p => p.TransferHistoryReceiverAdmins)
                .HasForeignKey(d => d.ReceiverAdminId)
                .HasConstraintName("FK_ReceiverAdmin");

            entity.HasOne(d => d.ReceiverClub).WithMany(p => p.TransferHistoryReceiverClubs)
                .HasForeignKey(d => d.ReceiverClubId)
                .HasConstraintName("FK_ReceiverClub");

            entity.HasOne(d => d.SenderAdmin).WithMany(p => p.TransferHistorySenderAdmins)
                .HasForeignKey(d => d.SenderAdminId)
                .HasConstraintName("FK_SenderAdmin");

            entity.HasOne(d => d.SenderClub).WithMany(p => p.TransferHistorySenderClubs)
                .HasForeignKey(d => d.SenderClubId)
                .HasConstraintName("FK_SenderClub");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CCAC6B2E6FD9");

            entity.ToTable("User");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.Avatar).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(255);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.Height).HasColumnType("decimal(5, 2)");
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
            entity.HasKey(e => e.UserActivityId).HasName("PK__UserActi__82560463C03507F9");

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

        modelBuilder.Entity<UserActivityClub>(entity =>
        {
            entity.HasKey(e => e.UserActivityClubId).HasName("PK__UserActi__6CAE09FCBC802542");

            entity.ToTable("UserActivityClub");

            entity.Property(e => e.UserActivityClubId).HasColumnName("UserActivityClubID");
            entity.Property(e => e.ActivityId).HasColumnName("ActivityID");
            entity.Property(e => e.ClubId).HasColumnName("ClubID");
            entity.Property(e => e.JoinDate).HasColumnType("datetime");
            entity.Property(e => e.Role).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Activity).WithMany(p => p.UserActivityClubs)
                .HasForeignKey(d => d.ActivityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserActivityClub_ActivityClub");

            entity.HasOne(d => d.Club).WithMany(p => p.UserActivityClubs)
                .HasForeignKey(d => d.ClubId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserActivityClub_Club");

            entity.HasOne(d => d.User).WithMany(p => p.UserActivityClubs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserActivityClub_User");
        });

        modelBuilder.Entity<UserClub>(entity =>
        {
            entity.HasKey(e => e.UserClubId).HasName("PK__UserClub__9BFD3C25DE386A9E");

            entity.ToTable("UserClub");

            entity.Property(e => e.UserClubId).HasColumnName("UserClubID");
            entity.Property(e => e.ClubId).HasColumnName("ClubID");
            entity.Property(e => e.JoinDate).HasColumnType("datetime");
            entity.Property(e => e.Role).HasMaxLength(50);
            entity.Property(e => e.Status).HasDefaultValueSql("((1))");
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
            entity.HasKey(e => e.UserSportId).HasName("PK__UserSpor__A6A5EE9BC6162BE6");

            entity.ToTable("UserSport");

            entity.Property(e => e.UserSportId).HasColumnName("UserSportID");
            entity.Property(e => e.LevelId).HasColumnName("LevelID");
            entity.Property(e => e.Position).HasMaxLength(100);
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
            entity.HasKey(e => e.UserTeamId).HasName("PK__UserTeam__9ADF80922DB78C71");

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
