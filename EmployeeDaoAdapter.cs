using System.Data.SQLite;
using System.Text;

public class EmployeeDaoAdapter : EmployeeDao{

    private readonly string connectionString;


    public EmployeeDaoAdapter(string connectionString = "Data Source=SampleDB.db;Version=3;"){
        this.connectionString = connectionString;
    }


    public Employee findEmployeeByID(int ID){

        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            string query = "SELECT * FROM Employee WHERE ID = @Id";
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", ID);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Employee
                        {
                            ID = Convert.ToInt32(reader["ID"]),
                            FirstName = reader["FirstName"].ToString(),
                            LastName = reader["LastName"].ToString(),
                            DateOfBirth = reader["DateOfBirth"].ToString(),
                            Gender = reader["Gender"].ToString(),
                            Salary = Convert.ToInt32(reader["Salary"]),
                        };
                    }
                    else
                    {
                        return null; // Employee not found
                    }
                }
            }
        }
    }



    public List<Employee> findAllEmployee(){

        List<Employee> employees = new List<Employee>();

        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            string query = "SELECT * FROM Employee";
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Employee employee = new Employee
                        {
                            ID = Convert.ToInt32(reader["ID"]),
                            FirstName = reader["FirstName"].ToString(),
                            LastName = reader["LastName"].ToString(),
                            DateOfBirth = reader["DateOfBirth"].ToString(),
                            Gender = reader["Gender"].ToString(),
                            Salary = Convert.ToInt32(reader["Salary"]),
                        };
                        employees.Add(employee);
                    }
                }
            }
        }

        return employees;
    }



    public void addEmployee(NewEmployeeDto newEmployee){
        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            string query = "INSERT INTO Employee (FirstName, LastName, DateOfBirth, Gender, Salary) " +
                           "VALUES (@FirstName, @LastName, @DateOfBirth, @Gender, @Salary)";

            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                // Add parameters to prevent SQL injection
                command.Parameters.AddWithValue("@FirstName", newEmployee.FirstName);
                command.Parameters.AddWithValue("@LastName", newEmployee.LastName);
                command.Parameters.AddWithValue("@DateOfBirth", newEmployee.DateOfBirth);
                command.Parameters.AddWithValue("@Gender", newEmployee.Gender);
                command.Parameters.AddWithValue("@Salary", newEmployee.Salary);

                // Execute the query
                command.ExecuteNonQuery();
            }
        }
    }



    public void updateEmployee(int id, Dictionary<string, object> updatedFields){
        
        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            StringBuilder queryBuilder = new StringBuilder("UPDATE Employee SET ");

            foreach (var field in updatedFields)
            {
                queryBuilder.Append($"{field.Key} = @{field.Key}, ");
            }

            queryBuilder.Remove(queryBuilder.Length - 2, 2); // Remove the last comma and space
            queryBuilder.Append($" WHERE ID = @Id");

            string query = queryBuilder.ToString();

            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", id);

                foreach (var field in updatedFields)
                {
                    command.Parameters.AddWithValue($"@{field.Key}", field.Value);
                }

                // Execute the update query
                int rowsAffected = command.ExecuteNonQuery();
            }
        }

    }



    public void deleteEmployee(int ID){
        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            string query = "DELETE FROM Employee WHERE ID = @Id";

            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", ID);

                // Execute the delete query
                int rowsAffected = command.ExecuteNonQuery();
            }
        }
    }
}