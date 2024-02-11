using System.Collections.Generic;
using System.Threading.Tasks;
using FitandFun.Models;

namespace FitandFun.Services{

    public interface IUserService 
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> GetById(int id);
        Task CreateAsync(User user);
        Task UpdateAsync(int id, User user);
        Task DeleteAsync(int id);
        Task AddWorkout(int userId, string workoutName);
        // Task<IEnumerable<User>> GetUsersWithWorkouts();

    }
}