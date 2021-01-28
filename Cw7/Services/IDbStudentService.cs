using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cw7.DTOs;
using Cw7.Models;
using Cw7.Services;

namespace Cw7.Services
{
    public interface IDbStudentService
    {
        public IEnumerable<Student> GetAllStudents();
        public GetSingleStudentResponse GetStudent(string indexNumber);
        public EnrollmentResult EnrollStudent(EnrollStudentRequest newStudent);
        public Enrolled PromoteStudents(PromoteStudentsRequest promoteStudentsRequest);
        public SingleStudentAuthenticationData GetStudentsAuthenticationData(string indexNumber);
        public bool UpdateRefreshToken(string username, string refreshToken);
    }
}