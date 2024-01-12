using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GURU.Model.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GURU.Model.JSON.Converters
{
    public class GradedTypesConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            var id = jsonObject[nameof(BindableEntityBase.Id)].Value<string>();

            object desrldObj = null;

            if (objectType == typeof(GradedInitialConditionType))
            {
                desrldObj = new GradedInitialConditionType(MainModel.Instance.InitialConditionTypes.FirstOrDefault(e => e.Id == id));
                (desrldObj as GradedInitialConditionType).Grade = jsonObject[nameof(GradedInitialConditionType.Grade)].Value<int>();
            }
            else if (objectType == typeof(GradedFailureModeType))
            {
                desrldObj = new GradedFailureModeType(MainModel.Instance.FailureModeTypes.FirstOrDefault(e => e.Id == id));
                (desrldObj as GradedFailureModeType).Grade = jsonObject[nameof(GradedInitialConditionType.Grade)].Value<int>();
            }
            else if (objectType == typeof(GradedInitialStressType))
            {
                desrldObj = new GradedInitialStressType(MainModel.Instance.InitialStressTypes.FirstOrDefault(e => e.Id == id));
                (desrldObj as GradedInitialStressType).Grade = jsonObject[nameof(GradedInitialConditionType.Grade)].Value<int>();
            }
            else throw new Exception("There is a new type that inherits from ConfiguredBindableEntityBase and is not being initialized in " + nameof(ConfiguredTypesConverter));

            return desrldObj;
        }

        public override bool CanConvert(Type objectType)
        {
            if (objectType == null) return false;

            var baseType = objectType.BaseType;
            if (baseType?.IsGenericType == true && baseType.GetGenericTypeDefinition() == typeof(GradedBindableEntityBase<>)) return true;
            else return false;
        }

        public override bool CanWrite { get; } = false;
        public override bool CanRead { get; } = true;
    }
}
