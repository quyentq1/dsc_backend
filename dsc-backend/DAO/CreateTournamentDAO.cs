using System.ComponentModel.DataAnnotations;
using System;

namespace dsc_backend.DAO
{
    public class CreateTournamentDAO
    {

        public int SportId { get; set; }


            public int LevelId { get; set; }

            public string Name { get; set; }

            public DateTime StartDate { get; set; }


            public DateTime EndDate { get; set; }

            public DateTime startTime { get; set; }
        

            public DateTime RegistrationDeadline { get; set; }


            public int numberOfParticipants { get; set; }

            public int UserId { get; set; }

            public int teamSize { get; set; }
            public string? note { get; set; }

            public string? location { get; set; }

            public string? TournamentType { get; set; }


        }
}
