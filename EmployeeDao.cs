
public interface EmployeeDao {

    Employee? findEmployeeByID(int ID);

    List<Employee> findAllEmployee();

    void addEmployee(NewEmployeeDto employee);

    void updateEmployee(int id, Dictionary<string, object> updatedFields);

    void deleteEmployee(int ID);


}