using System.Collections;
using NitroxClient.GameLogic.InitialSync.Abstract;
using Nitrox.Model.Subnautica.Packets;

namespace NitroxClient.GameLogic.InitialSync;

/// <summary>
///     Restores the ResourceTrackerDatabase which is required for scanner rooms to detect resources that aren't loaded.
/// </summary>
public sealed class ResourceTrackerInitialSyncProcessor : InitialSyncProcessor
{
    public override IEnumerator Process(InitialPlayerSync packet, WaitScreen.ManualWaitItem waitScreenItem)
    {
        ResourceTrackers.Apply(packet.ResourceTrackerEntries, []);
        Log.Info($"Received initial sync with {packet.ResourceTrackerEntries.Count} resource tracker entries");
        yield break;
    }
}
