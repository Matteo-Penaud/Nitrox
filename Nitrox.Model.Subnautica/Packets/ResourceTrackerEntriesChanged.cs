using System;
using System.Collections.Generic;
using Nitrox.Model.Packets;
using Nitrox.Model.Subnautica.DataStructures.GameLogic;

namespace Nitrox.Model.Subnautica.Packets;

/// <summary>
///     Resources registered to or unregistered from the ResourceTrackerDatabase, which is what scanner rooms look up.
/// </summary>
[Serializable]
public class ResourceTrackerEntriesChanged : Packet
{
    public List<ResourceTrackerEntry> Added { get; }
    public List<ResourceTrackerEntry> Removed { get; }

    public ResourceTrackerEntriesChanged(List<ResourceTrackerEntry> added, List<ResourceTrackerEntry> removed)
    {
        Added = added;
        Removed = removed;
    }
}
