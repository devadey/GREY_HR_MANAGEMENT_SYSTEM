namespace Infrastructure.Services
{
    public class EmployeeService(ApplicationDbContext context) : IEmployeeService
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<Employee> CreateEmployeeAsync(Employee employee)
        {
            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();
            return employee;
        }

        public async Task<int> DeleteEmployeeAsync(Employee employee)
        {
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            return employee.Id;
        }

        public async Task<Employee> GetEmployeeByIdAsync(int id)
        {
            var employeeInDb = await _context.Employees
                .Where(x => x.Id == id).FirstOrDefaultAsync();
            return employeeInDb;
        }

        public async Task<List<Employee>> GetEmployeeListAsync()
        {
            return await _context.Employees.ToListAsync();
        }

        public async Task<Employee> UpdateEmployeeAsync(Employee employee)
        {
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
            return employee;
        }
    }
}
