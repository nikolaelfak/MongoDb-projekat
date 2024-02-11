using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using FitAndFun.Services;
using System;
using System.Threading.Tasks;
using FitandFun.Services;


[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IMongoCollection<User> _usersCollection;
    private readonly IMongoCollection<FitnessGoal> _fitnessGoalsCollection;
    private readonly JwtService _jwtService;
    private readonly IUserService _userService;

    public UsersController(IMongoClient mongoClient, JwtService jwtService, IUserService userService)
    {
        var database = mongoClient.GetDatabase("fitandfun");
        _usersCollection = database.GetCollection<User>("users");
        _fitnessGoalsCollection = database.GetCollection<FitnessGoal>("fitnessGoals");
        _jwtService = jwtService;
        _userService = userService;
    }

        [HttpGet]
    //[Authorize] // Dodajte Authorize atribut za zaštićen pristup
    public ActionResult<IEnumerable<User>> GetUsers()
    {
        var users = _usersCollection.Find(user => true).ToList();
        return Ok(users);
    }

    [HttpGet("{id}", Name = "GetUser")]
    //[Authorize] // Dodajte Authorize atribut za zaštićen pristup
    public ActionResult<User> GetUserById(int id)
    {
        var user = _usersCollection.Find(u => u.Id == id).FirstOrDefault();

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPost("login")]
    public ActionResult<string> Login(LoginUser user)
    {
        // Implementirajte logiku za proveru korisničkih podataka i autentikaciju

        // Dummy implementacija: Proveravamo da li postoji korisnik sa datim username-om i password-om
        var existingUser = _usersCollection.Find(u => u.Username == user.Username && u.PasswordHash == user.PasswordHash).FirstOrDefault();

        if (existingUser == null)
        {
            // Ako korisnik ne postoji, vratite Unauthorized
            return Unauthorized();
        }

        // Ako korisnik postoji, generišite JWT token za njega
        var token = _jwtService.GenerateJwtToken(existingUser);

        // Vratite JWT token kao odgovor
        return Ok(token);
    }

        [HttpPost]
        public ActionResult<User> CreateUser(User user, int fitnessGoalId)
        {
            // Generišite jedinstveni ID za novog korisnika
            int nextId = GetNextUserId(); // Ova funkcija treba da postoji i generiše sledeći ID

            // Postavite ID korisnika na generisani ID
            user.Id = nextId;

            // Postavite ID cilja za korisnika
            user.FitnessGoalId = fitnessGoalId;

            // Dodajte korisnika u kolekciju
            _usersCollection.InsertOne(user);

            // Vratite kreiranog korisnika
            return CreatedAtRoute("GetUser", new { id = user.Id }, user);
        }

        // Metoda koja vraća sledeći jedinstveni ID za korisnika
        private int GetNextUserId()
        {
            // Pronađite najveći trenutni ID u bazi podataka
            var maxIdUser = _usersCollection.Find(u => true)
                                            .SortByDescending(u => u.Id)
                                            .Limit(1)
                                            .FirstOrDefault();

            // Ako nema korisnika u bazi, počnite sa ID-om 1
            if (maxIdUser == null)
            {
                return 1;
            }
            else
            {
                // Inače, koristite najveći ID i inkrementirajte ga za 1
                return maxIdUser.Id + 1;
            }
        }

        [HttpDelete("{id}", Name = "DeleteUser")]
        public ActionResult DeleteUser(int id)
        {
            var userToDelete = _usersCollection.Find(u => u.Id == id).FirstOrDefault();

            if (userToDelete == null)
            {
                return NotFound();
            }

            _usersCollection.DeleteOne(u => u.Id == id);
            return NoContent();
        }

        [HttpGet("{id}/goal", Name = "GetUserWithGoal")]
        public ActionResult<UserWithGoal> GetUserWithGoal(int id)
        {
            // Dodajte ispisivanje ID-ja koji se traži
            Console.WriteLine($"Requested user ID: {id}");

            var user = _usersCollection.Find(u => u.Id == id).FirstOrDefault();
            if (user == null)
            {
                // Dodajte ispisivanje poruke ako korisnik nije pronađen
                Console.WriteLine($"User with ID {id} not found");
                return NotFound();
            }

            // Provera da li korisnik ima postavljen cilj
            if (user.FitnessGoalId == 0)
            {
                return NotFound("User's fitness goal not found.");
            }

            var fitnessGoal = _fitnessGoalsCollection.Find(goal => goal.Id == user.FitnessGoalId).FirstOrDefault();
            if (fitnessGoal == null)
            {
                // Dodajte ispisivanje poruke ako cilj nije pronađen
                Console.WriteLine($"Fitness goal for user with ID {id} not found");
                return NotFound();
            }

            var userWithGoal = new UserWithGoal
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                RegistrationDate = user.RegistrationDate,
                FitnessGoal = fitnessGoal // Dodajemo cilj korisnika u objekat
            };

            return Ok(userWithGoal);
        }

        // POST: api/Users/{id}/goal
        // [HttpPost("{id}/goal", Name = "AddGoalForUser")]
        // public ActionResult AddGoalForUser(string id, [FromBody] FitnessGoal goal)
        // {
        //     if (!int.TryParse(id, out int userId))
        //     {
        //         return BadRequest("Invalid user ID format.");
        //     }

        //     var user = _usersCollection.Find(u => u.Id == userId).FirstOrDefault();
        //     if (user == null)
        //     {
        //         return NotFound("User not found.");
        //     }

        //     // Postavite ID korisnika za cilj
        //     goal.UserId = user.Id;

        //     // Možete dodati dodatnu logiku validacije cilja ovde pre dodavanja u bazu podataka
        //     // Na primer, proverite da li je datum roka cilja u budućnosti i da li opis cilja nije prazan

        //     try
        //     {
        //         _fitnessGoalsCollection.InsertOne(goal);
        //         return Ok("Goal added successfully.");
        //     }
        //     catch (Exception ex)
        //     {
        //         return StatusCode(500, $"An error occurred while adding goal: {ex.Message}");
        //     }
        // }

[HttpPost("{id}/goal", Name = "AddGoalForUser")]
public ActionResult AddGoalForUser(string id, [FromBody] FitnessGoal goal)
{
    if (!int.TryParse(id, out int userId))
    {
        return BadRequest("Invalid user ID format.");
    }

    var user = _usersCollection.Find(u => u.Id == userId).FirstOrDefault();
    if (user == null)
    {
        return NotFound("User not found.");
    }

    // Generišite jedinstveni ID za novi cilj
    int nextGoalId = GetNextGoalId();

    // Postavite ID cilja i korisnika za cilj
    goal.Id = nextGoalId;
    goal.UserId = userId; // Postavljamo direktno integer vrednost UserId

    Console.WriteLine("Received goal data:");
    Console.WriteLine($"Description: {goal.Goal}");
    Console.WriteLine($"Deadline: {goal.Deadline}");

    // Možete dodati dodatnu logiku validacije cilja ovde pre dodavanja u bazu podataka
    // Na primer, proverite da li je datum roka cilja u budućnosti i da li opis cilja nije prazan

    try
    {
        _fitnessGoalsCollection.InsertOne(goal);

        // Ažurirajte FitnessGoalId za korisnika
        user.FitnessGoalId = goal.Id;
        var filter = Builders<User>.Filter.Eq(u => u.Id, userId);
        var update = Builders<User>.Update.Set(u => u.FitnessGoalId, goal.Id);
        _usersCollection.UpdateOne(filter, update);

        return Ok("Goal added successfully.");
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"An error occurred while adding goal: {ex.Message}");
    }
}

// Metoda koja vraća sledeći jedinstveni ID za cilj korisnika
        private int GetNextGoalId()
        {
            // Pronađite najveći trenutni ID za cilj u bazi podataka
            var maxGoalId = _fitnessGoalsCollection.Find(goal => true)
                                                .SortByDescending(goal => goal.Id)
                                                .Limit(1)
                                                .FirstOrDefault();

            // Ako nema ciljeva u bazi, počnite sa ID-om 1
            if (maxGoalId == null)
            {
                return 1;
            }
            else
            {
                // Inače, koristite najveći ID i inkrementirajte ga za 1
                return maxGoalId.Id + 1;
            }
        }
        [HttpPost("DodajTreningKorisniku/{userId}")]
        public async Task<IActionResult> AddWorkoutToUser(int userId, string workoutName)
        {
            try
            {
                await _userService.AddWorkout(userId, workoutName);
                return Ok("Workout added to user successfully");
            }
            catch (Exception)
            {
                
                return StatusCode(500, "An error occurred while adding workout to user.");
            }
        }
        
    }
