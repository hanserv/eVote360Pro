using eVote360Pro.Core.Domain.Common;
using eVote360Pro.Core.Domain.Common.Enums;

namespace eVote360Pro.Core.Domain.Entities
{
    public class User : BaseEntity<int>
    {
        public required string Name { get; set; }
        public required string LastName { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public required UserRole Role { get; set; }
        public required bool IsActive { get; set; }

        public int? PoliticalPartyId { get; set; } // If
        public PoliticalParty? PoliticalParty { get; set; } // If
    }
}
