﻿using APIRest_ASPNET5.Data.VO;
using APIRest_ASPNET5.Models;
using APIRest_ASPNET5.Models.Context;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace APIRest_ASPNET5.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly MySQLContext _context;

        public EmployeeRepository(MySQLContext context)
        {
            _context = context;
        }

        public Employee ValidateCredentials(EmployeeVO employee)
        {
            var pass = ComputeHash(employee.Password, new SHA256CryptoServiceProvider());
            return _context.Employees.FirstOrDefault(e=> (e.Username == employee.Username) && (e.Password == pass));
        }

        private string ComputeHash(string input, SHA256CryptoServiceProvider algorithm)
        {
            Byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            Byte[] hashedBytes = algorithm.ComputeHash(inputBytes);
            return BitConverter.ToString(hashedBytes);
        }
    }
}