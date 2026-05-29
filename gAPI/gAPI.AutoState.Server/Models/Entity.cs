using System;
using System.Collections.Generic;
using System.Text;

namespace gAPI.AutoState.Server.Models;

public class Entity : SharedReference
{
    public EntityKey KeyProperty { get; } = new();
}
