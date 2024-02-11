using MongoDB.Driver;
using FitandFun.Models;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;



namespace FitandFun.Services{

    public class UserService : IUserService
    {
        private readonly IMongoCollection<User> _userCollection;
        private readonly IOptions<DatabseSettings> _dbSettings;
        private readonly IMongoCollection<Workout> _workoutCollection;
        private readonly IWorkoutService _workoutService;
        public UserService(IOptions<DatabseSettings> dbSettings, IWorkoutService workoutService)
        {
            _dbSettings = dbSettings;
            var mongoClient = new MongoClient(dbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(dbSettings.Value.DatabaseName);
            _userCollection=mongoDatabase.GetCollection<User>(dbSettings.Value.UsersCollectionName);
            _workoutCollection = mongoDatabase.GetCollection<Workout>(dbSettings.Value.WorkoutsCollectionName);
             _workoutService = workoutService;
             

        }

        public async Task<IEnumerable<User>> GetAllAsync()=>
            await _userCollection.Find(_=>true).ToListAsync();

        public async  Task<User> GetById(int id) =>
            await _userCollection.Find(a=>a.Id == id).FirstOrDefaultAsync();
        
        public async Task CreateAsync(User user)=>
            await _userCollection.InsertOneAsync(user);

        public async Task UpdateAsync(int id, User user)=>
            await _userCollection.ReplaceOneAsync(a=> a.Id == id, user);    

        public async Task DeleteAsync(int id)=>
            await _userCollection.DeleteOneAsync(a=>a.Id==id);

        public async Task AddWorkout(int userId, string workoutName)
        {
            // Dobijanje korisnika iz baze podataka
            var user = await _userCollection.Find(u => u.Id == userId).FirstOrDefaultAsync();
            if (user == null)
            {
                throw new ArgumentException("User not found", nameof(userId));
            }

            // Dobijanje treninga iz servisa
            var workout = await _workoutService.GetByName(workoutName);
            if (workout == null)
            {
                throw new ArgumentException("Workout not found", nameof(workoutName));
            }

            // Dodavanje treninga korisniku
            user.Workouts ??= new List<Workout>(); // Inicijalizacija liste ako je null
            user.Workouts.Add(workout);

            // Dodavanje Id korisnika u listu UserIds treninga
            workout.UserIds ??= new List<int>(); // Inicijalizacija liste ako je null
            workout.UserIds.Add(userId);

            // Ažuriranje korisnika u bazi podataka
            var updateUser = Builders<User>.Update.Set(u => u.Workouts, user.Workouts);
            await _userCollection.UpdateOneAsync(u => u.Id == userId, updateUser);

            // Ažuriranje treninga u bazi podataka
            var updateWorkout = Builders<Workout>.Update.Set(w => w.UserIds, workout.UserIds);
            await _workoutCollection.UpdateOneAsync(w => w.Id == workout.Id, updateWorkout);
        }


        // public async Task<IEnumerable<User>> GetUsersWithWorkouts()
        // {
        //     try
        //     {
        //         var usersWithWorkouts = await _userCollection.Find(_ => true).ToListAsync();

        //         foreach (var user in usersWithWorkouts)
        //         {
        //             user.Workouts = await _workoutCollection.Find(w => w.UserIds.Contains(user.Id)).ToListAsync();
        //         }

        //         return usersWithWorkouts;
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine($"Greška prilikom dohvatanja korisnika sa treningom: {ex.Message}");
        //         throw;
        //     }
        // }
    }
}