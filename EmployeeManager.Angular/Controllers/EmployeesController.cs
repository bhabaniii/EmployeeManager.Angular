using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManager.Angular.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManager.Angular.Controllers
{
    [Route("api/[controller]")]
    //[Authorize(Roles ="Manager")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly AppDbContext _appDbContext = null;

        public EmployeesController(AppDbContext appDbContext)
        {
            this._appDbContext = appDbContext;
        }



        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            List<Employee> employees = await _appDbContext.Employees.ToListAsync();

            return Ok(employees);

        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            Employee emp = await _appDbContext.Employees.FindAsync(id);

            return Ok(emp);

        }



        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] Employee emp)
        {
            if (ModelState.IsValid)
            {
                await _appDbContext.Employees.AddAsync(emp);
                await _appDbContext.SaveChangesAsync();

                return CreatedAtAction("Get", new { id = emp.EmployeeID }, emp);

            }
            else
            {

                return BadRequest();
            }

        }



        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(int id, [FromBody] Employee emp)
        {

            if (ModelState.IsValid)
            {
                _appDbContext.Employees.Update(emp);

                await _appDbContext.SaveChangesAsync();

                return NoContent();

            }

            else
            {

                return BadRequest();
            }
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id )
        {
            Employee emp = await _appDbContext.Employees.FindAsync(id);
            _appDbContext.Employees.Remove(emp);
            await _appDbContext.SaveChangesAsync();

            return NoContent();


        }
    }
}
