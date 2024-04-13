
using Application.Services;

namespace Application.Features.Employees.Commands;

public class DeleteEmployeeCommand : IRequest<IResponseWrapper>
{
    public int EmployeeId { get; set; }
}

public class DeleteEmployeeCommandHandler(IEmployeeService employeeService) : IRequestHandler<DeleteEmployeeCommand, IResponseWrapper>
{
    private readonly IEmployeeService _employeeService = employeeService;

    async Task<IResponseWrapper> IRequestHandler<DeleteEmployeeCommand, IResponseWrapper>.Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employeeInDb = await _employeeService.GetEmployeeByIdAsync(request.EmployeeId);

        if (employeeInDb is not null)
        {
            var employeeId = await _employeeService.DeleteEmployeeAsync(employeeInDb);
            return await ResponseWrapper<int>.SuccessAsync(employeeId, "Employee deleted successfully.");
        }
        return await ResponseWrapper.FailAsync("Employee does not exist.");
    }
}
