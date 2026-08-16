using System;
using System.Collections.Generic;
using System.Text;

namespace CricArena.Business.Exceptions
{
    public class MembershipNotFoundException : Exception
    {
        public MembershipNotFoundException(Guid clubId, Guid playerId)
            : base($"Membership for club '{clubId}' and player '{playerId}' was not found.")
        {
        }
    }
}
