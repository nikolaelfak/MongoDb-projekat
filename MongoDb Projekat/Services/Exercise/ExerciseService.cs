using MongoDB.Driver;
using FitandFun.Models;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System.Collections.Generic;



namespace FitandFun.Services{

    public class ExerciseService : IExerciseService
    {
        private readonly IMongoCollection<Exercise> _exerciseCollection;
        private readonly IOptions<DatabseSettings> _dbSettings;
        private readonly IMongoCollection<Workout> _workoutCollection;
        private readonly IWorkoutService _workoutService;
        public ExerciseService(IOptions<DatabseSettings> dbSettings, IWorkoutService workoutService)
        {
            _dbSettings = dbSettings;
            var mongoClient = new MongoClient(dbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(dbSettings.Value.DatabaseName);
            _exerciseCollection=mongoDatabase.GetCollection<Exercise>(dbSettings.Value.ExercisesCollectionName);
            _workoutCollection = mongoDatabase.GetCollection<Workout>(dbSettings.Value.WorkoutsCollectionName);
             _workoutService = workoutService;
             

        }

        public async Task<IEnumerable<Exercise>> GetAllAsync()=>
            await _exerciseCollection.Find(_=>true).ToListAsync();

        public async  Task<Exercise> GetByName(string name) =>
            await _exerciseCollection.Find(a=>a.Name == name).FirstOrDefaultAsync();
        
        public async Task CreateAsync(Exercise exercise)=>
            await _exerciseCollection.InsertOneAsync(exercise);

        public async Task UpdateAsync(string name, Exercise exercise)=>
            await _exerciseCollection.ReplaceOneAsync(a=> a.Name == name, exercise);    

        public async Task DeleteAsync(string name)=>
            await _exerciseCollection.DeleteOneAsync(a=>a.Name==name);

    }
}