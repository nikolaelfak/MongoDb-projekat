using System;
using MongoDB.Bson;

public class UserWithGoal
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public DateTime RegistrationDate { get; set; }
    public FitnessGoal FitnessGoal { get; set; } // Dodajemo polje za cilj korisnika
}
