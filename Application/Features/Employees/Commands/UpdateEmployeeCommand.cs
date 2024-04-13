namespace Application.Features.Employees.Commands;

public class UpdateEmployeeCommand : IRequest<IResponseWrapper>
{
    public UpdateEmployeeRequest UpdateEmployeeRequest { get; set; }
}

public class UpdateEmployeeCommandHandler(IEmployeeService employeeService) : IRequestHandler<UpdateEmployeeCommand, IResponseWrapper>
{
    private readonly IEmployeeService _employeeService = employeeService;

    public async Task<IResponseWrapper> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employeeInDb = await _employeeService.GetEmployeeByIdAsync(request.UpdateEmployeeRequest.Id);
        if (employeeInDb is not null)
        {
            employeeInDb.FirstName = request.UpdateEmployeeRequest.FirstName;
            employeeInDb.LastName = request.UpdateEmployeeRequest.LastName;
            employeeInDb.Salary = request.UpdateEmployeeRequest.Salary;
            employeeInDb.Email = request.UpdateEmployeeRequest.Email;

            var updateRecordToDb = await _employeeService.UpdateEmployeeAsync(employeeInDb);

            var mappedResponseEmployee = new EmployeeResponse
            {
                FirstName = updateRecordToDb.FirstName,
                LastName = updateRecordToDb.LastName,
                Email = updateRecordToDb.Email,
                Salary = updateRecordToDb.Salary,
            };
            return await ResponseWrapper<EmployeeResponse>.SuccessAsync(mappedResponseEmployee, "Employee details have been successfully updated.");
        }
        return await ResponseWrapper.FailAsync("Employee does not exist.");
    }  
}
