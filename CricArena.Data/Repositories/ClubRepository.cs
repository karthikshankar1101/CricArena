using CricArena.Core.Entities;
using CricArena.Data.Context;
using CricArena.Data.Repositories.Interfaces;

namespace CricArena.Data.Repositories
{
    public class ClubRepository : Repository<Club>, IClubRepository
    {
        public ClubRepository(AppDbContext context) : base(context)
        {
        }
    }
}
