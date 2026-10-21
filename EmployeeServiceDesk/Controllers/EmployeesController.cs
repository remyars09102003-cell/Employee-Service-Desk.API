using System;
using System.Threading.Tasks;
using EmployeeServiceDesk.Application.DTOs.Employees;
using EmployeeServiceDesk.Application.ServiceInterface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeServiceDesk.API.Controllers
{
    [ApiController]
    [Route("api/employees")]
    [Authorize]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _service;

        public EmployeesController(IEmployeeService service)
        {
            _service = service;
        }

        // GET: api/employees
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var employees = await _service.GetAllAsync();

            return Ok(employees);
        }

        // GET: api/employees/1
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var employee = await _service.GetByIdAsync(id);

            if (employee == null)
            {
                return NotFound(new
                {
                    message = "Employee not found."
                });
            }

            return Ok(employee);
        }

        // POST: api/employees
        [HttpPost]
        [Authorize(Roles = "Administrator,ServiceDeskManager")]
        public async Task<IActionResult> Create(
            [FromBody] CreateEmployeeDto dto)
        {
            try
            {
                var employee = await _service.CreateAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = employee.Id },
                    employee);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }

        // PUT: api/employees/1
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Administrator,ServiceDeskManager")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateEmployeeDto dto)
        {
            try
            {
                var employee = await _service.UpdateAsync(id, dto);

                if (employee == null)
                {
                    return NotFound(new
                    {
                        message = "Employee not found."
                    });
                }

                return Ok(employee);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }

        // DELETE: api/employees/1
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Administrator,ServiceDeskManager")]
        public async Task<IActionResult> Deactivate(int id)
        {
            var result = await _service.DeactivateAsync(id);

            if (!result)
            {
                return NotFound(new
                {
                    message = "Employee not found."
                });
            }

            return NoContent();
        }
    }
}