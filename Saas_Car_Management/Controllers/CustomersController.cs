using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Saas_Car_Management.API.Filters;
using Saas_Car_Management.Core.DTOs;
using Saas_Car_Management.Core.Interfaces;

namespace Saas_Car_Management.Controllers
{
    [Authorize]
    public class CustomersController : BaseApiController
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomersController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomers()
        {
            var customers = await _customerRepository.GetCustomersAsync(GetTenantId());
            return Ok(customers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomer(int id)
        {
            var c = await _customerRepository.GetCustomerByIdAsync(id, GetTenantId());
            if (c == null) return NotFound();
            return Ok(c);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerDto dto)
        {
            var result = await _customerRepository.CreateCustomerAsync(GetTenantId(), dto);
            if (result == null) return BadRequest("Could not create customer profile.");
            return CreatedAtAction(nameof(GetCustomer), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] CreateCustomerDto dto)
        {
            var success = await _customerRepository.UpdateCustomerAsync(id, GetTenantId(), dto);
            if (!success) return NotFound();
            return Ok(new { message = "Customer profile updated successfully." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var success = await _customerRepository.DeleteCustomerAsync(id, GetTenantId());
            if (!success) return NotFound();
            return Ok(new { message = "Customer deleted." });
        }
    }
}
