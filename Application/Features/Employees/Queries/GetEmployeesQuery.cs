using Application.Services;
using Common.Responses.Employee;
using System.Collections.Generic;

namespace Application.Features.Employees.Queries;

public class GetEmployeesQuery : IRequest<IResponseWrapper>
{
}


public class GetEmployeesQueryHandler(IEmployeeService employeeService) : IRequestHandler<GetEmployeesQuery, IResponseWrapper>
{
    private readonly IEmployeeService _employeeService = employeeService;

    public async Task<IResponseWrapper> Handle(GetEmployeesQuery request, CancellationToken cancellationToken)
    {

        var employeeList = await _employeeService.GetEmployeeListAsync();

        if (employeeList.Count > 0)
        {
            var mappedEmployeeList = new List<EmployeeResponse>();

            foreach (var employee in employeeList)
            {
                var mappedResponseEmployees = new EmployeeResponse
                {
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    Email = employee.Email,
                    Salary = employee.Salary,
                };

                mappedEmployeeList.Add(mappedResponseEmployees);
            }

            return await ResponseWrapper<List<EmployeeResponse>>.SuccessAsync(mappedEmployeeList);
        }
        else
        {
            return await ResponseWrapper.FailAsync("No employees were found.");
        }
    }
}
