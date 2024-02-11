using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Collections.Generic;

[Route("api/[controller]")]
[ApiController]
public class FitnessGoalsController : ControllerBase
{
    private readonly IMongoCollection<FitnessGoal> _fitnessGoalsCollection;

    public FitnessGoalsController(IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase("fitandfun");
        _fitnessGoalsCollection = database.GetCollection<FitnessGoal>("fitnessGoals");
    }

    [HttpGet]
    public ActionResult<IEnumerable<FitnessGoal>> GetFitnessGoals()
    {
        var fitnessGoals = _fitnessGoalsCollection.Find(goal => true).ToList();
        return Ok(fitnessGoals);
    }

    [HttpGet("{id}", Name = "GetFitnessGoal")]
    public ActionResult<FitnessGoal> GetFitnessGoalById(int id)
    {
        var fitnessGoal = _fitnessGoalsCollection.Find(g => g.Id == id).FirstOrDefault();

        if (fitnessGoal == null)
        {
            return NotFound();
        }

        return Ok(fitnessGoal);
    }

    [HttpPost]
    public ActionResult<FitnessGoal> CreateFitnessGoal(FitnessGoal fitnessGoal)
    {
        // Generišite jedinstveni ID za novi cilj
        int nextId = GetNextFitnessGoalId(); 

        // Postavite ID cilja na generisani ID
        fitnessGoal.Id = nextId;

        // Dodajte cilj u kolekciju
        _fitnessGoalsCollection.InsertOne(fitnessGoal);

        // Vratite kreirani cilj
        return CreatedAtRoute("GetFitnessGoal", new { id = fitnessGoal.Id }, fitnessGoal);
    }

    // Metoda koja vraća sledeći jedinstveni ID za fitnes cilj
    private int GetNextFitnessGoalId()
    {
        // Pronađite najveći trenutni ID cilja u bazi podataka
        var maxIdFitnessGoal = _fitnessGoalsCollection.Find(goal => true)
                                                    .SortByDescending(goal => goal.Id)
                                                    .Limit(1)
                                                    .FirstOrDefault();

        // Ako nema ciljeva u bazi, počnite sa ID-om 1
        if (maxIdFitnessGoal == null)
        {
            return 1;
        }
        else
        {
            // Inače, koristite najveći ID i inkrementirajte ga za 1
            return maxIdFitnessGoal.Id + 1;
        }
    }
    [HttpDelete("{id}", Name = "DeleteFitnessGoal")]
    public ActionResult DeleteFitnessGoal(int id)
    {
        var fitnessGoalToDelete = _fitnessGoalsCollection.Find(g => g.Id == id).FirstOrDefault();

        if (fitnessGoalToDelete == null)
        {
            return NotFound();
        }

        _fitnessGoalsCollection.DeleteOne(g => g.Id == id);
        return NoContent();
    }
}
