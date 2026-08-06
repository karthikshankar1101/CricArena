using System;
using System.Collections.Generic;
using System.Text;

namespace CricArena.Business.Exceptions
{
    public class DuplicateEmailException : Exception
    {
        public DuplicateEmailException(string email)
        : base($"A player with email '{email}' already exists.")
        {
        }
    }
}
