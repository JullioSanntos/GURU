using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Converters;

namespace GURU.Model.JSON.Converters
{
    public class MainModelConverter : CustomCreationConverter<MainModel>
    {
        public override MainModel Create(Type objectType)
        {
            MainModel.Instance.Dispose();
            return MainModel.Instance;
        }
    }
}
