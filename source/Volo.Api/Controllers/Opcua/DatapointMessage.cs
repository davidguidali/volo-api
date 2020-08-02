using System.ComponentModel.DataAnnotations;

namespace Volo.Api.Controllers.Opcua
{
    public class DatapointMessage
    {
        [Required]
        public string Identifier { get; set; }

        public int Value { get; set; }
    }
}