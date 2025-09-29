﻿namespace OrderManagementBLL.Exceptions;

public class BusinessConflictException : Exception
{
    public BusinessConflictException() { }

    public BusinessConflictException(string message) 
        : base(message) { }

    public BusinessConflictException(string message, Exception innerException) 
        : base(message, innerException) { }
}