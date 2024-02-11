using System.Runtime.CompilerServices;

namespace FitandFun.Models{

    public class DatabseSettings
    {
        public string? ConnectionString{get; set;}
        public string? DatabaseName{get; set;}
        public string? UsersCollectionName{get; set;}
        public string? WorkoutsCollectionName{get;set;}

        public string? ExercisesCollectionName{get;set;}
    }
}