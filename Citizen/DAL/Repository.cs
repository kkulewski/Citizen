using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Citizen.DAL
{
    public class Repository : IRepository
    {
        private IApplicationUserRepo _applicationUserRepo;

        public Repository(IApplicationUserRepo applicationUserRepo)
        {
            _applicationUserRepo = applicationUserRepo;
        }

        public IApplicationUserRepo ApplicationUserRepo() => _applicationUserRepo;
    }
}
