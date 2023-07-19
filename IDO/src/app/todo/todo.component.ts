// todo.component.ts
import { Component, OnInit } from '@angular/core';
import { Task } from './task.model';
import { TaskService } from './task.service';
import { Router } from '@angular/router';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { __values } from 'tslib';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-todo',
  templateUrl: './todo.component.html',
  styleUrls: ['./todo.component.css']
})
export class TodoComponent implements OnInit {
  tasks: Task[] = []; // Array to store the tasks
  newTask: string = ''; // Variable to store the new task input
  loggedInUser: string = '';  // variable to store username
  showQuoteBanner: boolean= true;
  searchTerm: string = '';

   // Datepicker configuration
   bsConfig: Partial<BsDatepickerConfig> = {
    // Add any other configuration options you need
  };

  constructor(private taskService: TaskService,
              private router: Router, 
              private datePipe: DatePipe) { }

  ngOnInit() {
    
      // Retrieve the logged-in user from localStorage
      const user = localStorage.getItem('loggedInUser');

      if (user) {
        this.loggedInUser = JSON.parse(user).email;
      }

      this.fetchTasks(); // Fetch tasks when the component initializes
  }

  logout(): void {
    // Clear user's authentication token or session information
    localStorage.removeItem('token');

    // Navigate to the login screen
    this.router.navigate(['']);
  }

  fetchTasks() {
    this.taskService.getTasks().subscribe(
      (tasks) => {
        this.tasks = tasks.reverse().map((task) => {
          return {
            ...task
          };
        });
      },
      (error) => {
        console.log('An error occurred while fetching tasks:', error);
      }
    );
  }
  
  addTask() {
    if (this.newTask.trim() === '') {
      return; // If the new task input is empty, return without adding the task
    }
  
    const task: Task = {
      title: this.newTask,
      completed: false,
      status: 'To Do', // Set the default status to 'To Do'
      category: 'Default Category', 
      estimate: {
        value: 2,
        units: 'Default Units'
      },
      importance: 'medium',// Set the category value here or fetch it from the user input
    };
  
    this.taskService.getTasks().subscribe(
      (existingTasks: Task[]) => {
        // Check if any existing task has the same name as the new task
        const hasDuplicateName = existingTasks.some(task => task.title === this.newTask);
  
        if (hasDuplicateName) {
          // Display an error message indicating a task with the same name already exists
          alert('A task with the same name already exists. Please choose a different name.');
        } else {
          // Proceed with creating the new task
          this.taskService.createTask(task).subscribe(
            () => {
              console.log('Task created successfully');
              this.fetchTasks(); // Fetch the tasks again after creating the new task
              this.newTask = ''; // Clear the new task input field
            },
            (error) => {
              console.log('An error occurred while creating the task:', error);
            }
          );
        }
      },
      (error) => {
        console.log('An error occurred while retrieving existing tasks:', error);
      }
    );
  }
  
  updateTask(task: Task, property: any, event: Event, dueDate?: any): void {
  if (!event) {
    return; // Guard clause to handle undefined event
  }
  
  console.log(dueDate);
  var newValue ='';

  const formattedDate = this.datePipe.transform(dueDate, 'yyyy-MM-dd');
  
  if(!(property === 'dueDate'))
  {
   newValue = (event.target as HTMLDivElement).innerText || '';
  }
  
  if (property === 'title') {
    task.title = newValue;
  } else if (property === 'category') {
    task.category = newValue;
  } else if (property === 'dueDate') {
    task.dueDate = dueDate;
  } else if (property === 'estimateValue') {
    const parsedValue = parseFloat(newValue);
    task.estimate!.value = isNaN(parsedValue) ? 0 : parsedValue;
  } else if (property === 'estimateUnits') {
    task.estimate!.units = newValue;
  } else if (property === 'importance') {
    // Map the string value to the corresponding importance level
    if (newValue === 'Low') {
      task.importance = "Low";
    } else if (newValue === 'Medium') {
      task.importance = "Medium";
    } else if (newValue === 'High') {
      task.importance = "High";
    }
  }

  // Update the task using the task service
  this.taskService.updateTask(task).subscribe(
    () => {
      console.log('Task updated successfully');
    },
    (error) => {
      console.log('An error occurred while updating the task:', error);
    }
  );
}

  deleteTask(id?: number) {
    if (id === undefined) {
      return; // If the id is undefined, return without deleting the task
    }

    if (!confirm('Are you sure you want to delete this task?')) {
      return; // If the user cancels the delete confirmation, return without deleting the task
    }

    this.taskService.deleteTask(id).subscribe(
      () => {
        this.tasks = this.tasks.filter((task) => task.id !== id); // Remove the deleted task from the tasks array
      },
      (error) => {
        console.log('An error occurred while deleting the task:', error);
      }
    );
  }

  // Closing the banner
  closeQuoteBanner(): void {
    this.showQuoteBanner = false;
  }

   // Getter method for filtered tasks based on status and search term
   get filteredTasks(): Task[] {
    return this.tasks.filter((task) => {
      const statusMatches = task.status.toLowerCase().includes(this.searchTerm.toLowerCase());
      const titleMatches = task.title.toLowerCase().includes(this.searchTerm.toLowerCase());
      return statusMatches || titleMatches;
    });
  }

  // Method to filter tasks based on the status
  filteredTasksByStatus(status: string): Task[] {
    return this.filteredTasks.filter((task) => task.status === status);
  }

  onDragStart(event: DragEvent, task: Task) {
    // Set the task ID as data to be transferred during the drag
    event.dataTransfer?.setData('text/plain', task.id!.toString());
  }

  onDragOver(event: DragEvent) {
    // Allow the drop event to occur
    event.preventDefault();
  }

  onDrop(event: DragEvent, newStatus: string) {
    // Prevent default handling (e.g., opening a URL when dropping an item)
    event.preventDefault();

    const taskId = parseInt(event.dataTransfer?.getData('text/plain') || '', 10);
    const task = this.tasks.find((t) => t.id === taskId);

    if (task && task.status !== newStatus) {
      task.status = newStatus;
      // Here, you need to update the task status in your database or service.
      // For demonstration purposes, I'll assume you have a method updateTaskStatus(taskId: number, newStatus: string) that updates the task status in your task service.
      this.taskService.updateTask(task).subscribe(
        () => {
          console.log('Task updated successfully');
        },
        (error) => {
          console.log('An error occurred while updating the task:', error);
        }
      );
    }
  }
  
}
