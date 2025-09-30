# ToDoApp ✅

![License](https://img.shields.io/badge/license-MIT-blue.svg)  
![Build Status](https://img.shields.io/badge/build-passing-brightgreen.svg)  

---

## Description
**ToDoApp** is a lightweight ASP.NET WebForms application with SQL Server backend that helps you manage daily tasks.  
It supports adding, editing, deleting, color-tagging, and marking tasks as completed. The app also features **drag-and-drop task ordering** for better organization.

---

## Table of Contents
1. [Introduction](#1-introduction)  
2. [Installation](#2-installation)  
3. [Usage](#3-usage)  
4. [Features](#4-features)  
5. [Contributing](#5-contributing)  
6. [License](#6-license)  
7. [Acknowledgments](#7-acknowledgments)  

---

## 1. Introduction
The motivation behind **ToDoApp** was to create a simple yet powerful task manager with:  
- Fast UI updates using `UpdatePanel` and jQuery  
- Persistent storage with SQL Server  
- A clean interface to manage tasks visually  

This project demonstrates **ASP.NET WebForms + jQuery integration** for real-time task updates and serves as a learning project for beginners in ASP.NET.

---

## 2. Installation

### Prerequisites
- [.NET Framework 4.8](https://dotnet.microsoft.com/download/dotnet-framework/net48)  
- [SQL Server](https://www.microsoft.com/sql-server) (Express or Developer edition)  
- Visual Studio 2019/2022  

### Steps
1. Clone the repository:
   ```bash
   git clone https://github.com/username/ToDoApp.git
   cd ToDoApp
Open the project in Visual Studio.

Configure the database:

Run the SQL script in /Database/ToDoApp.sql to create the ToDoItems table.

Update the connection string in Web.config:

xml
Copy code
<connectionStrings>
  <add name="ToDoDb"
       connectionString="Data Source=.;Initial Catalog=ToDoApp;Integrated Security=True"
       providerName="System.Data.SqlClient" />
</connectionStrings>
Build and run the project (F5 in Visual Studio).

3. Usage
Add a Task
Type a task in the New Task textbox and click Add.
The task will appear in the list immediately.

Mark as Done
Click the Done tab on a task.
✔ The task will be marked with strikethrough and disabled (cannot be undone).

Change Color
Click the Color tab, pick a color → task background updates instantly.

Delete a Task
Click the Delete tab once → shows "SURE?" confirmation.
Click again → task deleted.
Click elsewhere → confirmation is canceled.

Drag-and-Drop
Drag tasks to reorder → order is saved to the database.

4. Features
✅ Add, edit, delete tasks

✅ Mark tasks as Done (one-way only, never undone)

✅ Color-tag tasks for categorization

✅ Drag-and-drop task ordering

✅ ASP.NET WebForms backend with SQL persistence

✅ jQuery-powered frontend interactions

5. Contributing
Contributions are welcome!

Fork the repo

Create a branch (git checkout -b feature/my-feature)

Commit changes (git commit -m "Added new feature")

Push (git push origin feature/my-feature)

Create a Pull Request

Guidelines
Stick to C# coding conventions

Keep UI responsive

Ensure SQL queries are parameterized (no SQL injection)

6. License
This project is licensed under the MIT License.
See LICENSE for details.

7. Acknowledgments
jQuery for front-end event handling

ASP.NET WebForms for rapid backend development

SQL Server for reliable data storage

Inspiration from classic ToDo applications