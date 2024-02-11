using MongoDB.Driver;
using FitandFun.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;


namespace FitandFun.Services{

    public class WorkoutService : IWorkoutService
    {
        private readonly IMongoCollection<Workout> _workoutCollection;
        private readonly IOptions<DatabseSettings> _dbSettings;
         //private readonly IMongoCollection<Exercise> _exerciseCollection;
        // private readonly IExerciseService _exerciseService;
        public WorkoutService(IOptions<DatabseSettings> dbSettings)
        {
            _dbSettings = dbSettings;
            var mongoClient = new MongoClient(dbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(dbSettings.Value.DatabaseName);
            _workoutCollection=mongoDatabase.GetCollection<Workout>(dbSettings.Value.WorkoutsCollectionName);
             //_exerciseCollection = mongoDatabase.GetCollection<Exercise>(dbSettings.Value.ExercisesCollectionName);
             //_exerciseService=exerciseService;

        }

        public async Task<IEnumerable<Workout>> GetAllAsync()=>
            await _workoutCollection.Find(_=>true).ToListAsync();

        public async  Task<Workout> GetByName(string name) =>
            await _workoutCollection.Find(a=>a.Name == name).FirstOrDefaultAsync();
        
        public async Task CreateAsync(Workout workout)=>
            await _workoutCollection.InsertOneAsync(workout);

public async Task UpdateAsync(string name, string newName, string newDescription)
        {
            try
            {
                var filter = Builders<Workout>.Filter.Eq(w => w.Name, name);
                var update = Builders<Workout>.Update
                    .Set(w => w.Name, newName)
                    .Set(w => w.Description, newDescription); 

                var result = await _workoutCollection.UpdateOneAsync(filter, update);

                if (result.ModifiedCount == 0)
                {
                    throw new ArgumentException("Workout not found", nameof(name));
                }
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Greška prilikom ažuriranja treninga: {ex.Message}");
                throw;
            }
        }


        public async Task DeleteAsync(string name)=>
            await _workoutCollection.DeleteOneAsync(a=>a.Name==name);

        // public async Task AddeExercise(string workoutId, string exerciseName)
        // {
        //     // Dobijanje treninga iz baze podataka
        //     var workout = await _workoutCollection.Find(u => u.Id == workoutId).FirstOrDefaultAsync();
        //     if (workout == null)
        //     {
        //         throw new ArgumentException("Workout not found", nameof(workoutId));
        //     }

        //     // Dobijanje vezbe iz servisa
        //     var exercise = await _exerciseService.GetByName(exerciseName);
        //     if (exercise == null)
        //     {
        //         throw new ArgumentException("Exercise not found", nameof(exerciseName));
        //     }

        //     // Dodavanje vezbi treningu
        //     workout.Exercises??= new List<Exercise>(); // Inicijalizacija liste ako je null
        //     workout.Exercises.Add(exercise);

            
        //     // exercise.Workout ??= new Workout(); // Inicijalizacija liste ako je null
        //     // exercise.Workout.

        //     // Ažuriranje korisnika u bazi podataka
        //     var updateWorkout = Builders<Workout>.Update.Set(u => u.Exercises, workout.Exercises);
        //     await _workoutCollection.UpdateOneAsync(u => u.Id == workoutId, updateWorkout);

        //     // Ažuriranje treninga u bazi podataka
        //     // var updateExercise = Builders<Exercise>.Update.Set(w => w.UserIds, workout.UserIds);
        //     // await _workoutCollection.UpdateOneAsync(w => w.Id == workout.Id, updateExercise);
        // }

    }
}