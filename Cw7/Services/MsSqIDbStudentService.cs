using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Cw7.DTOs;
using Cw7.Models;



namespace Cw7.Services
{
    public class MsSqlDbStudentService : IDbStudentService
    {
        private const string ConnectionString = "Data Source=db-mssql;Initial Catalog=s19841;Integrated Security=True";
        private readonly IEncryptionService _encryptionService;

        public MsSqlDbStudentService(IEncryptionService encryptionService)
        {
            _encryptionService = encryptionService;
        }

        public IEnumerable<Student> GetAllStudents()
        {
            using var sqlConnection = new SqlConnection(ConnectionString);
            using var sqlCommand = new SqlCommand { Connection = sqlConnection };
            sqlConnection.Open();
            return new MsSqlDbGetStudentService(sqlCommand).GetAllStudents();
        }

        public GetSingleStudentResponse GetStudent(string indexNumber)
        {
            using var sqlConnection = new SqlConnection(ConnectionString);
            using var sqlCommand = new SqlCommand { Connection = sqlConnection };
            sqlConnection.Open();
            return new MsSqlDbGetStudentService(sqlCommand).GetStudent(indexNumber);
        }

        public EnrollmentResult EnrollStudent(EnrollStudentRequest newStudent)
        {
            using var sqlConnection = new SqlConnection(ConnectionString);
            using var sqlCommand = new SqlCommand { Connection = sqlConnection };
            sqlConnection.Open();
            sqlCommand.Transaction = sqlConnection.BeginTransaction();
            try
            {
                var enrollmentResult = new MsSqlDbEnrollmentStudentService(sqlCommand, newStudent,
                    _encryptionService.Encrypt).Enroll();
                if (enrollmentResult.Successful) sqlCommand.Transaction.Commit();
                else sqlCommand.Transaction.Rollback();
                return enrollmentResult;
            }
            catch (Exception exception)
            {
                sqlCommand.Transaction.Rollback();
                return new EnrollmentResult { Error = $"Exeption - {exception}!" };
            }
        }

        public Enrolled PromoteStudents(PromoteStudentsRequest promoteStudentsRequest)
        {
            using var sqlConnection = new SqlConnection(ConnectionString);
            using var sqlCommand = new SqlCommand("PromoteStudents", sqlConnection)
            { CommandType = CommandType.StoredProcedure };
            sqlCommand.Parameters.AddWithValue("@Studies", promoteStudentsRequest.Studies);
            sqlCommand.Parameters.AddWithValue("@Semester", promoteStudentsRequest.Semester);
            sqlConnection.Open();
            var sqlDataReader = sqlCommand.ExecuteReader();
            if (!sqlDataReader.Read()) return null;
            return new Enrolled
            {
                IdEnrollment = (int)sqlDataReader["IdEnrollment"],
                Semester = (int)sqlDataReader["Semester"],
                IdStudy = (int)sqlDataReader["IdStudy"],
                StudiesName = sqlDataReader["Name"].ToString(),
                StartDate = DateTime.Parse(sqlDataReader["StartDate"].ToString()!)
            };
        }

        public SingleStudentAuthenticationData GetStudentsAuthenticationData(string indexNumber)
        {
            using var sqlConnection = new SqlConnection(ConnectionString);
            using var sqlCommand = new SqlCommand { Connection = sqlConnection };
            sqlConnection.Open();
            return new MsSqlDbGetStudentService(sqlCommand).GetStudentsAuthenticationData(indexNumber);
        }

        public bool UpdateRefreshToken(string username, string refreshToken)
        {
            using var sqlConnection = new SqlConnection(ConnectionString);
            using var sqlCommand = new SqlCommand
            {
                Connection = sqlConnection,
                CommandText = "UPDATE Student SET RefreshToken = @RefreshToken WHERE IndexNumber = @Username"
            };
            sqlCommand.Parameters.AddWithValue("@RefreshToken", refreshToken);
            sqlCommand.Parameters.AddWithValue("@Username", username);
            sqlConnection.Open();
            return sqlCommand.ExecuteNonQuery() != 0;
        }
    }
}