namespace Application.Features.Employees.Commands;

public class CreateEmployeeCommand : IRequest<IResponseWrapper>
{
    public CreateEmployeeRequest CreateEmployeeRequest { get; set; }
}

public class CreateEmployeeCommandHandler(IEmployeeService employeeService) : IRequestHandler<CreateEmployeeCommand, IResponseWrapper>
{
    private readonly IEmployeeService _employeeService = employeeService;

    public async Task<IResponseWrapper> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var mappedEmployee = new Employee
        {
            FirstName = request.CreateEmployeeRequest.FirstName,
            LastName = request.CreateEmployeeRequest.LastName,
            Email = request.CreateEmployeeRequest.Email,
            Salary = request.CreateEmployeeRequest.Salary,
        };

        var newEmployee = await _employeeService.CreateEmployeeAsync(mappedEmployee);

        if (newEmployee.Id > 0)
        {
            var mappedResponseEmployee = new EmployeeResponse
            {
                FirstName = newEmployee.FirstName,
                LastName = newEmployee.LastName,
                Email = newEmployee.Email,
                Salary = newEmployee.Salary,
            };

            return await ResponseWrapper<EmployeeResponse>.SuccessAsync(mappedResponseEmployee, "Employee created successfully.");
        }
        return await ResponseWrapper.FailAsync("Failed to create the employee entry.");
    }
}
