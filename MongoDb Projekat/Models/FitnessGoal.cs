using System;

public class FitnessGoal
{
    public int Id { get; set; }
    public int UserId { get; set; } // ID korisnika koji je postavio cilj
    public string Goal { get; set; } // Opis cilja (npr. "Izgubiti 5 kilograma u naredna 2 meseca")
    public DateTime Deadline { get; set; } // Rok za postizanje cilja
    public bool Achieved { get; set; } // Da li je cilj postignut
    public DateTime CreatedAt { get; set; } // Datum kada je cilj postavljen
}
