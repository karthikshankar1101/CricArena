using System;
using System.Collections.Generic;
using System.Text;

namespace CricArena.Business.Exceptions
{
    public class ClubNotFoundException : Exception
    {
        public ClubNotFoundException(Guid id)
        : base($"Club '{id}' was not found.")
        {
        }
    }
}
