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
    public class ConfiguredTypesConverter : JsonConverter
    {

        // this custom writer is disabled so that we can benefit of the serializer settings selections
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // This object is casted to BindableEntityBase because only two properties (Id and Name) are needed for serialization
            if (!(value is BindableEntityBase configdObj)) throw new Exception();
            var jObj = new JObject();
            jObj.Add(nameof(configdObj.Id), configdObj.Id);
            jObj.Add(nameof(configdObj.Name), configdObj.Name);
            jObj.WriteTo(writer);
        }

        public Dictionary<int, object> References { get; set; } = new Dictionary<int, object>();
        public JsonSerializer PreviousSerializer { get; set; }
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // it is assumed that a new deserialization will be done with a new serializer object. !!!Danger, Will Robinson!!!
            if (PreviousSerializer != serializer)
            {
                // clear references "$ref" to previous objects
                References.Clear();
                PreviousSerializer = serializer;
            }

            var jsonObject = JObject.Load(reader);
            object desrldObj;

            if (jsonObject.ContainsKey("$id")) desrldObj = ReadJsonWithIdKey(jsonObject, objectType, existingValue, serializer);
            else if (jsonObject.ContainsKey("$ref")) desrldObj = ReadJsonWithRefKey(jsonObject, objectType, existingValue, serializer);
            else throw new Exception("both $id and $ref can not be found in a JsonObject during deserialization");

            return desrldObj;
        }

        public object ReadJsonWithIdKey(JObject jsonObject, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonId = jsonObject["$id"].Value<int>();
            var id = jsonObject[nameof(BindableEntityBase.Id)].Value<string>();

            object desrldObj = null;

            if (objectType == typeof(ElementType)) desrldObj = MainModel.Instance.ElementTypes.FirstOrDefault(e => e?.Id == id);
            else if (objectType == typeof(FailureModeType)) desrldObj = MainModel.Instance.FailureModeTypes.FirstOrDefault(e => e.Id == id);
            else if (objectType == typeof(InitialConditionType)) desrldObj = MainModel.Instance.InitialConditionTypes.FirstOrDefault(e => e.Id == id);
            else if (objectType == typeof(InitialStressType)) desrldObj = MainModel.Instance.InitialStressTypes.FirstOrDefault(e => e.Id == id);
            else throw new Exception("There is a new type that inherits from ConfiguredBindableEntityBase and is not being initialized in " + nameof(ConfiguredTypesConverter));

            References.Add(jsonId, desrldObj);
            return desrldObj;
        }

        public object ReadJsonWithRefKey(JObject jsonObject, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken tokenValue;
            object desrldObj = null;
            if (jsonObject.TryGetValue("$ref", out tokenValue) == false) return null;

            if (References.ContainsKey(tokenValue.Value<int>())) {
                desrldObj = References[tokenValue.Value<int>()];
                return desrldObj;
            }

            return desrldObj;
        }
        public override bool CanConvert(Type objectType)
        {
            if (objectType == null) return false;

            var baseType = objectType.BaseType;
            if (baseType?.IsGenericType == true && baseType.GetGenericTypeDefinition() == typeof(ConfiguredBindableEntityBase<,>)) return true;
            else return false;
        }

        public override bool CanWrite { get; } = false;
        public override bool CanRead { get; } = true;
    }
}
