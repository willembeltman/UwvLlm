using gAPI.Core.Attributes;
using gAPI.Core.Dtos;

namespace UwvLlm.Shared.Public.Dtos;

[GenerateSerializer]
[IsStateDto]
public class State : AuthStateDto
{
    public List<string> roles { get; set; } = new List<string>();
}
