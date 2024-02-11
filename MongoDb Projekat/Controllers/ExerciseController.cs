using Microsoft.AspNetCore.Mvc;
using FitandFun.Models;
using FitandFun.Services;
using System.Threading.Tasks;

namespace FitandFun.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExerciseController : ControllerBase
    {
        private readonly IExerciseService _exerciseService;
        public ExerciseController(IExerciseService exerciseService)
        {
            _exerciseService = exerciseService;
        }

        [HttpGet("VratiSveVezbe")]
        public async Task<IActionResult> GetExercises()
        {
            var exercise = await _exerciseService.GetAllAsync();
            return Ok(exercise);
        }

        [HttpGet("VratiVezbePoNazivu/{naziv}")]
        public async Task<IActionResult> GetExercisesByName(string name)
        {
            var exercise = await _exerciseService.GetByName(name);
            if (exercise == null)
            {
                return NotFound();
            }
            return Ok(exercise);
        }

        [HttpPost("NapraviVezbe")]
        public async Task<IActionResult> CreateExercise(Exercise exercise)
        {
           await _exerciseService.CreateAsync(exercise);
            return Ok("exercise created successfully");
        }

        [HttpPut("AzurirajVezbu/{name}")]
        public async Task<IActionResult> UpdateExerciseByName(string name, [FromBody] Exercise newExercise)
        {
            var exercise = await _exerciseService.GetByName(name);
            if (exercise == null)
                return NotFound();
            await _exerciseService.UpdateAsync(name, newExercise);
            return Ok("exercise updated successfully");
        }

        [HttpDelete("ObrisiVezbu/{name}")]
        public async Task<IActionResult> DeleteExerciseByName(string name)
        {
            var exercise = await _exerciseService.GetByName(name);
            if (exercise == null)
                return NotFound();
            await _exerciseService.DeleteAsync(name);
            return Ok("deleted successfully");
        }

      
    }
}
