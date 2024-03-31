using System.Text.Json;


var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
EmployeeUseCase employeeUseCase = new EmployeeUseCase();




app.MapGet("/", () => @"REST API to perform Create, Read, Update and Delete methods.


                                        ***Usage***

+--------+----------------------+-----------------------------+---------------+----------------+
|        | API                  | Description                 | Request Body  | Response Body  |
+--------+----------------------+-----------------------------+---------------+----------------+
| CREATE | /AddEmployee         | Add a new employee          | None          | Employee item  |
+--------+----------------------+-----------------------------+---------------+----------------+
| READ   | /ListEmployee/{id}   | Get employee by id          | None          | Employee item  |
+--------+----------------------+-----------------------------+---------------+----------------+
| UDATE  | /UpdateEmployee/{id} | Update an existing employee | None          | Employee item  |
+--------+----------------------+-----------------------------+---------------+----------------+
| DELETE | /DeleteEmployee/{id} | Delete an employee          | None          | None           |
+--------+----------------------+-----------------------------+---------------+----------------+
| LIST   | /ListEmployee        | List all employees          | None          | Employee items |
+--------+----------------------+-----------------------------+---------------+----------------+


Example Employee object:

{
  ""ID"": 23,
  ""FirstName"": ""Emre"",
  ""LastName"": ""Bayraktar"",
  ""DateOfBirth"": ""2001-08-16"",
  ""Gender"": ""Male"",
  ""Salary"": 66000
}


http://localhost:5050/AddEmployee?FirstName=Emre&LastName=Bayraktar&DateOfBirth=2001-08-16&Gender=Male&Salary=66000

");



// To get all employees
app.MapGet("/ListEmployee", () => {

    List<Employee> allEmployees = employeeUseCase.getEmployees();
    return JsonSerializer.Serialize(allEmployees, new JsonSerializerOptions { WriteIndented = true });
});



// To get specified employee
app.MapGet("/ListEmployee/{id}", (HttpContext context, int id) => {

    var employee = employeeUseCase.getEmployeeByID(id);
    
    if (employee == null) {
        return "This employee does not exist";
    }

    // Return the employee data
    var jsonResponse = JsonSerializer.Serialize(employee, new JsonSerializerOptions{WriteIndented = true});
    return jsonResponse;
});



// To add a new employee
app.MapGet("/AddEmployee", (HttpContext context) =>
{
    // Retrieve employee details
    var firstName = context.Request.Query["FirstName"].ToString();
    var lastName = context.Request.Query["LastName"].ToString();
    var dateOfBirth = context.Request.Query["DateOfBirth"].ToString();
    var gender = context.Request.Query["Gender"].ToString();
    var salaryStr = context.Request.Query["Salary"].ToString();
    int salary = int.TryParse(salaryStr, out int parsedSalary) ? parsedSalary : 0;

    // Create a new employee object
    var newEmployee = new NewEmployeeDto{
        FirstName = firstName,
        LastName = lastName,
        DateOfBirth = dateOfBirth,
        Gender = gender,
        Salary = salary
    };

    // Save the new employee data
    employeeUseCase.saveEmployee(newEmployee);

    // Return the created employee data
    var jsonResponse = JsonSerializer.Serialize(newEmployee, new JsonSerializerOptions { WriteIndented = true });

    return jsonResponse;
});


// To Update specified employee
app.MapGet("/UpdateEmployee/{id}", (HttpContext context, int id) =>
{
    // Retrieve update fields
    var updatedFields = new Dictionary<string, object>();

    foreach (var queryParameter in context.Request.Query)
    {
        updatedFields.Add(queryParameter.Key, queryParameter.Value);
    }

    // Update the employee data
    var isSuccess = employeeUseCase.updateEmployee(id, updatedFields);

    // Return the updated employee data
    var updatedEmployee = employeeUseCase.getEmployeeByID(id);

    var jsonResponse = JsonSerializer.Serialize(updatedEmployee, new JsonSerializerOptions { WriteIndented = true });

    return jsonResponse;
});



// To delete specified employee
app.MapGet("/DeleteEmployee/{id}", (HttpContext context, int id) =>
{
    return employeeUseCase.deleteEmployee(id);
});





















app.Run();
