using System.Collections.Generic;
using Nitrox.Model.Subnautica.DataStructures.GameLogic;
using Nitrox.Model.Subnautica.Packets;
using NitroxClient.Communication.Abstract;

namespace NitroxClient.GameLogic;

/// <summary>
///     Keeps the ResourceTrackerDatabase (which scanner rooms look up) in sync with the server.
/// </summary>
/// <remarks>
///     Subnautica persists this database in its save and only fills it with resources as they get loaded,
///     so without the server's copy, scanner rooms would only detect resources loaded during the current session.
/// </remarks>
public class ResourceTrackers
{
    private readonly IPacketSender packetSender;

    public ResourceTrackers(IPacketSender packetSender)
    {
        this.packetSender = packetSender;
    }

    public static bool IsRegistered(ResourceTracker resourceTracker)
    {
        return ResourceTrackerDatabase.resources.TryGetValue(resourceTracker.techType, out Dictionary<string, ResourceTrackerDatabase.ResourceInfo> resourcesById) &&
               resourcesById.ContainsKey(resourceTracker.uniqueId);
    }

    public void BroadcastRegistered(ResourceTracker resourceTracker)
    {
        // Objects without a NitroxEntity are local only and get a new id each session
        if (!resourceTracker.TryGetNitroxId(out _))
        {
            return;
        }
        packetSender.Send(new ResourceTrackerEntriesChanged([ToEntry(resourceTracker)], []));
    }

    public void BroadcastUnregistered(ResourceTracker resourceTracker)
    {
        if (!resourceTracker.TryGetNitroxId(out _))
        {
            return;
        }
        packetSender.Send(new ResourceTrackerEntriesChanged([], [ToEntry(resourceTracker)]));
    }

    /// <remarks>
    ///     Calls the database directly instead of <see cref="ResourceTracker" /> so that nothing is broadcasted back.
    /// </remarks>
    public static void Apply(List<ResourceTrackerEntry> added, List<ResourceTrackerEntry> removed)
    {
        foreach (ResourceTrackerEntry entry in added)
        {
            ResourceTrackerDatabase.Register(entry.UniqueId, entry.Position.ToUnity(), entry.TechType.ToUnity());
        }
        foreach (ResourceTrackerEntry entry in removed)
        {
            ResourceTrackerDatabase.Unregister(entry.UniqueId, entry.TechType.ToUnity());
        }
    }

    private static ResourceTrackerEntry ToEntry(ResourceTracker resourceTracker)
    {
        return new(resourceTracker.uniqueId, resourceTracker.techType.ToDto(), resourceTracker.transform.position.ToDto());
    }
}
