using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using BackendForIDO.Models;
using BackendForIDO.Data;
using BackendForIDO.Services;
using System.Globalization;


namespace BackendForIDO.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    public class TasksController : ControllerBase
    {
        private readonly TaskRepository _taskRepository;
        private readonly AuthenticationService _authenticationService;

        public TasksController(TaskRepository taskRepository, AuthenticationService authenticationService)
        {
            _taskRepository = taskRepository;
            _authenticationService = authenticationService;
        }

        // GET api/tasks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskEntity>>> GetAllTasks()
        {
            int userId = _authenticationService.GetLoggedInUserId();

            // Retrieve all tasks for the logged-in user from the repository
            var tasks = await _taskRepository.GetAllTasksForUser(userId);

            return Ok(tasks);
        }



        // GET api/tasks/{id}
        [HttpGet("{id}")]
        public ActionResult<TaskEntity> GetTaskById(int id)
        {
            int userId = _authenticationService.GetLoggedInUserId();

            // Retrieve the task with the provided ID for the logged-in user from the repository
            var task = _taskRepository.GetTaskByIdForUser(userId, id);

            if (task == null)
            {
                return NotFound();
            }

            return Ok(task);
        }

        // POST api/tasks
        [HttpPost]
        public ActionResult<TaskEntity> CreateTask(TaskEntity task)
        {
            int userId = _authenticationService.GetLoggedInUserId();

            // Set the user ID for the task before saving it to the repository
            task.UserId = userId;

            // Save the new task to the repository
            _taskRepository.CreateTaskForUser(userId, task);

            return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, task);
        }

        // PUT api/tasks/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateTask(int id, TaskEntity task)
        {
            int userId = _authenticationService.GetLoggedInUserId();

            // Retrieve the existing task with the provided ID for the logged-in user from the repository
            var existingTask = _taskRepository.GetTaskByIdForUser(userId, id);

            if (existingTask == null)
            {
                return NotFound();
            }

            // Update the properties of the existing task
            existingTask.Title = task.Title;
            existingTask.Completed = task.Completed;
            existingTask.Category = task.Category;
            existingTask.Estimate = new Estimate
            {
                Value = task.Estimate.Value,
                Units = task.Estimate.Units
            };
            existingTask.Importance = task.Importance;
            existingTask.status = task.status;

            if (task.DueDate.HasValue)
                {
                    if (DateTime.TryParseExact(task.DueDate.Value.ToString("yyyy-MM-dd"), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dueDate))
                    {
                        existingTask.DueDate = dueDate;
                    }
                    else
                    {
                        // Handle invalid due date format here if needed
                    }
                }
                else
                {
                    existingTask.DueDate = null; // If the DueDate is null in the input, set the existing task's DueDate to null
                }


            // Update the task in the repository
            _taskRepository.UpdateTaskForUser(userId, id, existingTask);

            return NoContent();
        }

        // DELETE api/tasks/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteTask(int id)
        {
            int userId = _authenticationService.GetLoggedInUserId();

            // Retrieve the task with the provided ID for the logged-in user from the repository
            var task = _taskRepository.GetTaskByIdForUser(userId, id);

            if (task == null)
            {
                return NotFound();
            }

            // Delete the task from the repository
            _taskRepository.DeleteTaskForUser(userId, id);

            return NoContent();
        }
    }
}
