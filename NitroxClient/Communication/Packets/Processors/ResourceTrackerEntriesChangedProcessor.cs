using Nitrox.Model.Subnautica.Packets;
using NitroxClient.Communication.Packets.Processors.Core;
using NitroxClient.GameLogic;

namespace NitroxClient.Communication.Packets.Processors;

internal sealed class ResourceTrackerEntriesChangedProcessor : IClientPacketProcessor<ResourceTrackerEntriesChanged>
{
    public Task Process(ClientProcessorContext context, ResourceTrackerEntriesChanged packet)
    {
        ResourceTrackers.Apply(packet.Added, packet.Removed);
        return Task.CompletedTask;
    }
}
