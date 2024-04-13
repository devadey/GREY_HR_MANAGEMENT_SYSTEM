namespace WebApi.Controllers;

[Route("api/[controller]")]
public class EmployeesController : BaseController<EmployeesController>
{
    private readonly ISender _sender;

    public EmployeesController(ISender sender, ITokenService tokenService) : base(sender, tokenService)
    {
        this._sender = sender;
    }

    [MustHavePermission(feature:AppFeature.Employees, action:AppAction.Create)]
    [HttpPost]
    public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeRequest request)
    {
        var response = await _sender.Send(new CreateEmployeeCommand { CreateEmployeeRequest = request });
        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    [MustHavePermission(feature: AppFeature.Employees, action: AppAction.Update)]
    [HttpPut]
    public async Task<IActionResult> UpdateEmployee([FromBody] UpdateEmployeeRequest updateEmployee)
    {
        var response = await _sender.Send(new UpdateEmployeeCommand { UpdateEmployeeRequest = updateEmployee });
        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    [MustHavePermission(feature: AppFeature.Employees, action: AppAction.Delete)]
    [HttpDelete("{employeeId}")]
    public async Task<IActionResult> DeleteEmployee([FromRoute]int employeeId)
    {
        var response = await _sender.Send(new DeleteEmployeeCommand {  EmployeeId = employeeId });
        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return NotFound(response);
    }

    [MustHavePermission(feature: AppFeature.Employees, action: AppAction.Read)]
    [HttpGet]
    public async Task<IActionResult> GetEmployeeList()
    {
        var response = await _sender.Send(new GetEmployeesQuery());
        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return NotFound(response);
    }

    [MustHavePermission(feature: AppFeature.Employees, action: AppAction.Read)]
    [HttpGet("{employeeId}")]
    public async Task<IActionResult> GetEmployeeById([FromRoute] int employeeId)
    {
        var response = await _sender.Send(new GetEmployeeByIdQuery { EmployeesId = employeeId });
        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return NotFound(response);
    }
}
