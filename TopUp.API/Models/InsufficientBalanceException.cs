﻿namespace TopUp.API.Models
{
    public class InsufficientBalanceException: Exception
    {
        public InsufficientBalanceException(string message): base(message)
        {
        }
    }
}
