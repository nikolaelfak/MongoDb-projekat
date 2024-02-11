using System;
using System.Collections.Generic;
using FitandFun.Models;
using MongoDB.Bson;

public class User
{
    //public ObjectId? Id { get; set; }
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public DateTime RegistrationDate { get; set; }
    public int FitnessGoalId { get; set; }

    public List<Workout>? Workouts { get; set; }
}