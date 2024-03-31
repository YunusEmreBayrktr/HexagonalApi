
public class EmployeeUseCase {

    private readonly EmployeeDao employeeDao ;

    public EmployeeUseCase(){
        employeeDao = new EmployeeDaoAdapter();
    }


    public String saveEmployee(NewEmployeeDto newEmployeeDto){

        if (!IsOverAge18(newEmployeeDto.DateOfBirth)){
            return "Employee is under the age of 18";
        }
        else{
            employeeDao.addEmployee(newEmployeeDto);
            return "Employee Saved Successfully";
        }

    }


    public List<Employee> getEmployees(){
        return employeeDao.findAllEmployee();
    }


    public String updateEmployee(int ID, Dictionary<string, object> updatedFields){

        // check if employee present
        if(employeeDao.findEmployeeByID(ID) == null){
            return "This employee does not exist";
        }
        else{
            employeeDao.updateEmployee(ID, updatedFields);
            return "Employee Updated Successfully";
        }  
    }


    public Employee getEmployeeByID(int ID){

        return employeeDao.findEmployeeByID(ID);
    }


    public String deleteEmployee(int ID){

        // check if employee present
        if(employeeDao.findEmployeeByID(ID) == null){
            return "This imployee does not exist";
        }
        else{
            employeeDao.deleteEmployee(ID);
            return "Employee  Successfully Deleted";
        }  
    }


    private bool IsOverAge18(string dateOfBirthString)
    {
        // Convert date string to DateTime
        if (!DateTime.TryParse(dateOfBirthString, out DateTime dateOfBirth)){
            return false;
        }

        // Calculate age
        int age = DateTime.Today.Year - dateOfBirth.Year;
        if (dateOfBirth > DateTime.Today.AddYears(-age)){
            age--;
        }

        return age >= 18;
    }
}