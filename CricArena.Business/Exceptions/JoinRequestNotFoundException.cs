using System;
using System.Collections.Generic;
using System.Text;

namespace CricArena.Business.Exceptions
{
    public class JoinRequestNotFoundException : Exception
    {
        public JoinRequestNotFoundException(Guid id)
        : base($"JoinRequest '{id}' was not found.")
        {
        }
    }
}
