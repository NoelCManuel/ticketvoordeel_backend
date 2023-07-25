using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Contracts
{
    public interface IProfileRepository
    {
        IEnumerable<Profile> GetAllProfiles();
        IEnumerable<Profile> GetProfiles(Expression<Func<Profile, bool>> predicate);
        Profile GetProfileById(int profileId);
        Profile CreateProfile(Profile profile);
        bool UpdateProfile(Profile profile);
        bool DeleteProfile(Profile profile);
    }
}
