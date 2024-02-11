using System;
using MongoDB.Bson;

public class LoginUser
{
    public string Username { get; set; }
    public string PasswordHash { get; set; }
}