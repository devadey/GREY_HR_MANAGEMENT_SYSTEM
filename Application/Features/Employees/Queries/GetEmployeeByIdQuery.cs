
using Application.Services;
using Common.Responses.Employee;

namespace Application.Features.Employees.Queries;

public class GetEmployeeByIdQuery : IRequest<IResponseWrapper>
{
    public int EmployeesId { get; set; }
}

public class GetEmployeeByIdQueryHandler(IEmployeeService employeeService) : IRequestHandler<GetEmployeeByIdQuery, IResponseWrapper>
{
    private readonly IEmployeeService _employeeService = employeeService;

    public async Task<IResponseWrapper> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
    {
        var employeeInDb = await _employeeService.GetEmployeeByIdAsync(request.EmployeesId);
        if (employeeInDb is not null)
        {
            var mappedResponseEmployee = new EmployeeResponse
            {
                FirstName = employeeInDb.FirstName,
                LastName = employeeInDb.LastName,
                Email = employeeInDb.Email,
                Salary = employeeInDb.Salary,
            };
            return await ResponseWrapper<EmployeeResponse>.SuccessAsync(mappedResponseEmployee);
        }
        return await ResponseWrapper.FailAsync("Employee does not exist.");
    }
}
