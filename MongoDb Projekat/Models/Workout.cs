using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace FitandFun.Models{

    public class Workout
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        
        [JsonIgnore]
        public List<int>? UserIds { get; set; }
        public List<Exercise>? Exercises { get; set; } 
    }

}