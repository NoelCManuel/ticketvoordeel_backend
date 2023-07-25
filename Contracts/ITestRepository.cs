using Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts
{
    public interface ITestRepository
    {
        IEnumerable<Test> GetAllTests();
        Test GetTestById(Guid testId);
        void CreateTest(Test test);
        void UpdateTest(Test test);
        void DeleteTest(Test test);
    }
}
