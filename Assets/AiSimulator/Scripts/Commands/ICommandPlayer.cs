using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieDevTools.Commands
{
    /// <summary>
    /// Interface for the command player used to create two dimensional collections of commands.
    /// </summary>
    public interface ICommandPlayer : ICommandEnumerator, ICommandLayerCollection { }
}

