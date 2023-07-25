using Contracts;
using Entities;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Repository
{
    public class ProfileRepository : RepositoryBase<Profile>, IProfileRepository
    {
        public ProfileRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public IEnumerable<Profile> GetAllProfiles()
        {
            return FindAll()
                .OrderBy(c => c.Id)
                .ToList();
        }

        public IEnumerable<Profile> GetProfiles(Expression<Func<Profile, bool>> predicate)
        {
            return FindByCondition(predicate);
        }

        public Profile GetProfileById(int testId)
        {
            return FindByCondition(test => test.Id.Equals(testId))
                .FirstOrDefault();
        }

        public Profile CreateProfile(Profile profile)
        {
            try
            {
                Create(profile);
                return profile;
            }
            catch (Exception ex)
            {
                return profile;
            }           
        }

        public bool UpdateProfile(Profile profile)
        {
            try
            {
                Update(profile);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }           
        }

        public bool DeleteProfile(Profile profile)
        {
            try
            {
                Delete(profile);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }            
        }
    }
}
