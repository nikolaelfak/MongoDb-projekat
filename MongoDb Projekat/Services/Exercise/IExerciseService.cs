using System.Collections.Generic;
using System.Threading.Tasks;
using FitandFun.Models;

namespace FitandFun.Services{

    public interface IExerciseService 
    {
        Task<IEnumerable<Exercise>> GetAllAsync();
        Task<Exercise> GetByName(string id);
        Task CreateAsync(Exercise exercise);
        Task UpdateAsync(string id, Exercise exercise);
        Task DeleteAsync(string id);


    }
}