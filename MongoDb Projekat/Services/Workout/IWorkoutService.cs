using System.Collections.Generic;
using System.Threading.Tasks;
using FitandFun.Models;

namespace FitandFun.Services{

    public interface IWorkoutService 
    {
        Task<IEnumerable<Workout>> GetAllAsync();
        Task<Workout> GetByName(string name);
        Task CreateAsync(Workout workout);
        Task UpdateAsync(string name, string newName, string newDescription);
        Task DeleteAsync(string name);
        //Task AddExerciseToWorkout(string workoutId, string exerciseName);

        

    }
}