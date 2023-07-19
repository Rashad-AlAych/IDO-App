using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using BackendForIDO.Models;
using BackendForIDO.Data;
using System.Globalization;


namespace BackendForIDO.Data
{
    public class TaskRepository
    {
        private readonly AppDbContext _dbContext;

        public TaskRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Retrieves all tasks for a specific user from the repository
        public async Task<List<TaskEntity>> GetAllTasksForUser(int userId)
        {
            var tasks = await _dbContext.tasks
                .Where(t => t.UserId == userId)
                .ToListAsync();

            return tasks;
        }

        // Retrieves a specific task for a user by its ID from the repository
        public TaskEntity GetTaskByIdForUser(int userId, int taskId)
        {
            return _dbContext.tasks.FirstOrDefault(t => t.UserId == userId && t.Id == taskId);
        }

        // Creates a new task for a user in the repository
        public TaskEntity CreateTaskForUser(int userId, TaskEntity task)
        {
            task.UserId = userId;
            _dbContext.tasks.Add(task);
            _dbContext.SaveChanges();
            return task;
        }

        // Updates an existing task for a user in the repository
        public TaskEntity UpdateTaskForUser(int userId, int taskId, TaskEntity task)
        {
            var existingTask = _dbContext.tasks.FirstOrDefault(t => t.UserId == userId && t.Id == taskId);
            if (existingTask != null)
            {
                existingTask.Title = task.Title;
                existingTask.Completed = task.Completed;
                existingTask.Category = task.Category;
                existingTask.Estimate.Value = task.Estimate.Value;
                existingTask.Estimate.Units = task.Estimate.Units;
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

                _dbContext.SaveChanges();
                return existingTask;
            }
            return null;
        }

        // Deletes a task for a user from the repository
        public TaskEntity DeleteTaskForUser(int userId, int taskId)
        {
            var existingTask = _dbContext.tasks.FirstOrDefault(t => t.UserId == userId && t.Id == taskId);
            if (existingTask != null)
            {
                _dbContext.tasks.Remove(existingTask);
                _dbContext.SaveChanges();
                return existingTask;
            }
            return null;
        }
    }
}
