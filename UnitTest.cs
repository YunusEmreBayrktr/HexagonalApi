using System.Data.SQLite;
using Xunit;

public class EmployeeDaoAdapterTests : IDisposable
{
    private readonly string testDbPath;
    private readonly string connectionString;
    private readonly EmployeeDaoAdapter employeeDaoAdapter;



    public EmployeeDaoAdapterTests(){
        //Temporary test database
        testDbPath = Path.GetTempFileName();
        connectionString = $"Data Source={testDbPath};Version=3;";
        employeeDaoAdapter = new EmployeeDaoAdapter(connectionString);

        InitializeDatabase();
    }



    private void InitializeDatabase(){
        // Create Employee table
        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            string query = @"CREATE TABLE Employee (
                                ID INTEGER PRIMARY KEY,
                                FirstName TEXT,
                                LastName TEXT,
                                DateOfBirth TEXT,
                                Gender TEXT,
                                Salary INTEGER
                            );";

            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }



    public void Dispose(){
        File.Delete(testDbPath);
    }



    [Fact]
    public void FindEmployeeByID_WithValidId_ReturnsEmployee(){
        // Arrange
        int id = 1;
        AddEmployeeToDatabase(id);

        // Act
        Employee foundEmployee = employeeDaoAdapter.findEmployeeByID(id);

        // Assert
        Assert.NotNull(foundEmployee);
        Assert.Equal(id, foundEmployee.ID);
    }



    [Fact]
    public void FindEmployeeByID_WithInvalidId_ReturnsNull(){
        // Arrange
        int invalidId = 999;

        // Act
        Employee foundEmployee = employeeDaoAdapter.findEmployeeByID(invalidId);

        // Assert
        Assert.Null(foundEmployee);
    }



    [Fact]
    public void FindAllEmployee_ReturnsAllEmployees(){
        // Arrange
        AddEmployeeToDatabase(1);
        AddEmployeeToDatabase(2);

        // Act
        List<Employee> employees = employeeDaoAdapter.findAllEmployee();

        // Assert
        Assert.Equal(2, employees.Count);
    }



    [Fact]
    public void AddEmployee_AddsNewEmployee(){
        // Arrange
        NewEmployeeDto newEmployee = new NewEmployeeDto { FirstName = "John", LastName = "Doe", DateOfBirth = "1990-01-01", Gender = "Male", Salary = 50000 };

        // Act
        employeeDaoAdapter.addEmployee(newEmployee);
        Employee retrievedEmployee = employeeDaoAdapter.findEmployeeByID(1);

        // Assert
        Assert.NotNull(retrievedEmployee);
        Assert.Equal(newEmployee.FirstName, retrievedEmployee.FirstName);
    }



    [Fact]
    public void UpdateEmployee_WithValidId_UpdatesEmployee(){
        // Arrange
        int id = 1;
        AddEmployeeToDatabase(id);
        Dictionary<string, object> updatedFields = new Dictionary<string, object>
        {
            { "FirstName", "UpdatedFirstName" }
        };

        // Act
        employeeDaoAdapter.updateEmployee(id, updatedFields);
        Employee updatedEmployee = employeeDaoAdapter.findEmployeeByID(id);

        // Assert
        Assert.NotNull(updatedEmployee);
        Assert.Equal("UpdatedFirstName", updatedEmployee.FirstName);
    }



    [Fact]
    public void DeleteEmployee_WithValidId_DeletesEmployee(){
        // Arrange
        int id = 1;
        AddEmployeeToDatabase(id);

        // Act
        employeeDaoAdapter.deleteEmployee(id);
        Employee deletedEmployee = employeeDaoAdapter.findEmployeeByID(id);

        // Assert
        Assert.Null(deletedEmployee);
    }



    private void AddEmployeeToDatabase(int id){
        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            string query = "INSERT INTO Employee (ID, FirstName, LastName, DateOfBirth, Gender, Salary) " +
                           "VALUES (@ID, @FirstName, @LastName, @DateOfBirth, @Gender, @Salary)";

            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ID", id);
                command.Parameters.AddWithValue("@FirstName", "John");
                command.Parameters.AddWithValue("@LastName", "Doe");
                command.Parameters.AddWithValue("@DateOfBirth", "1990-01-01");
                command.Parameters.AddWithValue("@Gender", "Male");
                command.Parameters.AddWithValue("@Salary", 50000);

                command.ExecuteNonQuery();
            }
        }
    }
}
