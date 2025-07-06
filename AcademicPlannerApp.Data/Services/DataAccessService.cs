using Microsoft.Data.Sqlite;
using AcademicPlannerApp.Data.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AcademicPlannerApp.Data.Services
{
    public class DataAccessService
    {
        private readonly string _databasePath;

        public DataAccessService(string databasePath)
        {
            _databasePath = databasePath;
            InitializeDatabase(); // Llama a este método para crear las tablas si no existen
        }

        // Método para asegurar que la base de datos y sus tablas estén creadas
        private void InitializeDatabase()
        {
            // Asegúrate de que el directorio para la DB exista
            var directory = Path.GetDirectoryName(_databasePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                connection.Open(); // Abrir conexión síncronamente para la inicialización

                var createTablesCommand = connection.CreateCommand();
                createTablesCommand.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Courses (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        Code TEXT,
                        Description TEXT
                    );

                    CREATE TABLE IF NOT EXISTS Assignments (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Title TEXT NOT NULL,
                        Description TEXT,
                        DueDate TEXT NOT NULL, -- Guardado como TEXT (ISO 8601)
                        Status TEXT NOT NULL, -- Guardado como TEXT (Nombre del Enum)
                        Priority TEXT NOT NULL, -- Guardado como TEXT (Nombre del Enum)
                        CourseId INTEGER,
                        FOREIGN KEY (CourseId) REFERENCES Courses(Id) ON DELETE SET NULL
                    );

                    CREATE TABLE IF NOT EXISTS Notes (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Title TEXT NOT NULL,
                        Content TEXT,
                        CreatedDate TEXT NOT NULL, -- Guardado como TEXT (ISO 8601)
                        CourseId INTEGER,
                        FOREIGN KEY (CourseId) REFERENCES Courses(Id) ON DELETE SET NULL
                    );
                ";
                createTablesCommand.ExecuteNonQuery(); // Ejecutar las sentencias CREATE TABLE
            }
        }

        // --- Métodos CRUD para Cursos ---

        public async Task<List<Course>> GetCoursesAsync()
        {
            var courses = new List<Course>();
            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT Id, Name, Code, Description FROM Courses;";

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        courses.Add(new Course
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Code = reader.IsDBNull(reader.GetOrdinal("Code")) ? null : reader.GetString(reader.GetOrdinal("Code")),
                            Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description"))
                        });
                    }
                }
            }
            return courses;
        }

        public async Task SaveCourseAsync(Course course)
        {
            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();

                if (course.Id == 0) // Es un nuevo curso
                {
                    command.CommandText = "INSERT INTO Courses (Name, Code, Description) VALUES (@Name, @Code, @Description); SELECT last_insert_rowid();";
                    command.Parameters.AddWithValue("@Name", course.Name);
                    command.Parameters.AddWithValue("@Code", course.Code ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Description", course.Description ?? (object)DBNull.Value);
                    course.Id = Convert.ToInt32(await command.ExecuteScalarAsync()); // Obtener el ID generado
                }
                else // Es un curso existente
                {
                    command.CommandText = "UPDATE Courses SET Name = @Name, Code = @Code, Description = @Description WHERE Id = @Id;";
                    command.Parameters.AddWithValue("@Name", course.Name);
                    command.Parameters.AddWithValue("@Code", course.Code ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Description", course.Description ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Id", course.Id);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteCourseAsync(int id)
        {
            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM Courses WHERE Id = @Id;";
                command.Parameters.AddWithValue("@Id", id);
                await command.ExecuteNonQueryAsync();
            }
        }

        // --- Métodos CRUD para Asignaciones (Assignments) ---

        public async Task<List<Assignment>> GetAssignmentsAsync()
        {
            var assignments = new List<Assignment>();
            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT Id, Title, Description, DueDate, Status, Priority, CourseId FROM Assignments;";

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        assignments.Add(new Assignment
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Title = reader.GetString(reader.GetOrdinal("Title")),
                            Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                            DueDate = DateTime.Parse(reader.GetString(reader.GetOrdinal("DueDate"))), // Convertir de TEXT a DateTime
                            Status = (AssignmentStatus)Enum.Parse(typeof(AssignmentStatus), reader.GetString(reader.GetOrdinal("Status"))), // Convertir de TEXT a Enum
                            Priority = (AssignmentPriority)Enum.Parse(typeof(AssignmentPriority), reader.GetString(reader.GetOrdinal("Priority"))), // Convertir de TEXT a Enum
                            CourseId = reader.IsDBNull(reader.GetOrdinal("CourseId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("CourseId"))
                        });
                    }
                }
            }
            return assignments;
        }

        public async Task SaveAssignmentAsync(Assignment assignment)
        {
            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();

                if (assignment.Id == 0) // Nueva asignación
                {
                    command.CommandText = "INSERT INTO Assignments (Title, Description, DueDate, Status, Priority, CourseId) VALUES (@Title, @Description, @DueDate, @Status, @Priority, @CourseId); SELECT last_insert_rowid();";
                }
                else // Actualizar asignación existente
                {
                    command.CommandText = "UPDATE Assignments SET Title = @Title, Description = @Description, DueDate = @DueDate, Status = @Status, Priority = @Priority, CourseId = @CourseId WHERE Id = @Id;";
                    command.Parameters.AddWithValue("@Id", assignment.Id);
                }

                command.Parameters.AddWithValue("@Title", assignment.Title);
                command.Parameters.AddWithValue("@Description", assignment.Description ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@DueDate", assignment.DueDate.ToString("yyyy-MM-dd HH:mm:ss")); // Convertir DateTime a TEXT
                command.Parameters.AddWithValue("@Status", assignment.Status.ToString()); // Convertir Enum a TEXT
                command.Parameters.AddWithValue("@Priority", assignment.Priority.ToString()); // Convertir Enum a TEXT
                command.Parameters.AddWithValue("@CourseId", assignment.CourseId ?? (object)DBNull.Value);

                if (assignment.Id == 0)
                {
                    assignment.Id = Convert.ToInt32(await command.ExecuteScalarAsync());
                }
                else
                {
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteAssignmentAsync(int id)
        {
            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM Assignments WHERE Id = @Id;";
                command.Parameters.AddWithValue("@Id", id);
                await command.ExecuteNonQueryAsync();
            }
        }

        // --- Métodos CRUD para Notas (Notes) ---

        public async Task<List<Note>> GetNotesAsync()
        {
            var notes = new List<Note>();
            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT Id, Title, Content, CreatedDate, CourseId FROM Notes;";

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        notes.Add(new Note
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Title = reader.GetString(reader.GetOrdinal("Title")),
                            Content = reader.IsDBNull(reader.GetOrdinal("Content")) ? null : reader.GetString(reader.GetOrdinal("Content")),
                            CreatedDate = DateTime.Parse(reader.GetString(reader.GetOrdinal("CreatedDate"))), // Convertir de TEXT a DateTime
                            CourseId = reader.IsDBNull(reader.GetOrdinal("CourseId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("CourseId"))
                        });
                    }
                }
            }
            return notes;
        }

        public async Task SaveNoteAsync(Note note)
        {
            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();

                if (note.Id == 0) // Nueva nota
                {
                    command.CommandText = "INSERT INTO Notes (Title, Content, CreatedDate, CourseId) VALUES (@Title, @Content, @CreatedDate, @CourseId); SELECT last_insert_rowid();";
                }
                else // Actualizar nota existente
                {
                    command.CommandText = "UPDATE Notes SET Title = @Title, Content = @Content, CreatedDate = @CreatedDate, CourseId = @CourseId WHERE Id = @Id;";
                    command.Parameters.AddWithValue("@Id", note.Id);
                }

                command.Parameters.AddWithValue("@Title", note.Title);
                command.Parameters.AddWithValue("@Content", note.Content ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@CreatedDate", note.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss")); // Convertir DateTime a TEXT
                command.Parameters.AddWithValue("@CourseId", note.CourseId ?? (object)DBNull.Value);

                if (note.Id == 0)
                {
                    note.Id = Convert.ToInt32(await command.ExecuteScalarAsync());
                }
                else
                {
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteNoteAsync(int id)
        {
            using (var connection = new SqliteConnection($"Filename={_databasePath}"))
            {
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM Notes WHERE Id = @Id;";
                command.Parameters.AddWithValue("@Id", id);
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}