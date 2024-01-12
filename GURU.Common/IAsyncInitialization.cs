using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GURU.Common
{
    public interface IAsyncInitialization
    {
        Task InitializeAsync();
    }
}
