using Newtonsoft.Json;

namespace QingzhenyunApis.EntityModels
{
    public abstract class EntityBodyBase
    {
        private bool success = true;

        [JsonProperty(PropertyName = "success")]
        public bool Success
        {
            get => success;
            set
            {
                if (value)
                {
                    success = value;
                }
                else
                {
                    throw new JsonSerializationException();
                }
            }
        }
    }

}
