using Contracts;
using Entities;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repository
{
    public class TestRepository : RepositoryBase<Test>, ITestRepository
    {
        public TestRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public IEnumerable<Test> GetAllTests()
        {
            return FindAll()
                .OrderBy(c => c.Id)
                .ToList();
        }

        public Test GetTestById(Guid testId)
        {
            return FindByCondition(test => test.Id.Equals(testId))
                .FirstOrDefault();
        }

        public void CreateTest(Test test)
        {
            Create(test);
        }

        public void UpdateTest(Test test)
        {
            Update(test);
        }

        public void DeleteTest(Test test)
        {
            Delete(test);
        }
    }
}
