using Microsoft.AspNetCore.Mvc;
using FitandFun.Models;
using FitandFun.Services;
using System.Threading.Tasks;
using System;
using MongoDB.Driver;

namespace FitandFun.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkoutController : ControllerBase
    {
        private readonly IWorkoutService _workoutService;
        private readonly IMongoCollection<Workout> _workoutsCollection;
        public WorkoutController(IWorkoutService workoutService, IMongoClient mongoClient)
        {
            _workoutService = workoutService;
            var database = mongoClient.GetDatabase("fitandfun");
            _workoutsCollection = database.GetCollection<Workout>("workouts");
        }

        [HttpGet("VratiSveTreninge")]
        public async Task<IActionResult> GetWorkouts()
        {
            var workouts = await _workoutService.GetAllAsync();
            return Ok(workouts);
        }

        [HttpGet("VratiTreningPoNazivu/{naziv}")]
        public async Task<IActionResult> GetWorkoutByName(string name)
        {
            var workout = await _workoutService.GetByName(name);
            if (workout == null)
            {
                return NotFound();
            }
            return Ok(workout);
        }

        [HttpPost("NapraviTrening")]
        public async Task<IActionResult> CreateWorkout(Workout workout)
        {
           await _workoutService.CreateAsync(workout);
            return Ok("workout created successfully");
        }

        [HttpPut("AzurirajTrening/{name}")]
public async Task<IActionResult> UpdateWorkoutByName(string name, string newName, string newDescription)
{
    try
    {
        await _workoutService.UpdateAsync(name, newName, newDescription);
        return Ok("Workout updated successfully");
    }
    catch (ArgumentException ex)
    {
        return NotFound(ex.Message);
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"An error occurred: {ex.Message}");
    }
}

        [HttpDelete("ObrisiTrening/{name}")]
        public async Task<IActionResult> DeleteWorkoutByName(string name)
        {
            var workout = await _workoutService.GetByName(name);
            if (workout == null)
                return NotFound();
            await _workoutService.DeleteAsync(name);
            return Ok("deleted successfully");
        }

        // [HttpPost("DodajVezbuTreningu/{workoutId}/exercises")]
        // public async Task<IActionResult> AddExerciseToWorkout(string workoutId,  string exerciseName)
        // {
        //     try
        //     {
        //         await _workoutService.AddExerciseToWorkout(workoutId, exerciseName);
        //         return Ok("Exercise added to workout successfully.");
        //     }
        //     catch (ArgumentException ex)
        //     {
        //         return NotFound(ex.Message);
        //     }
        //     catch (Exception ex)
        //     {
        //         return StatusCode(500, $"An error occurred: {ex.Message}");
        //     }
        // }
      
    }
}
