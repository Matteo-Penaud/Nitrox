using Nitrox.Server.Subnautica.Models.GameLogic;
using Nitrox.Server.Subnautica.Models.Packets.Core;

namespace Nitrox.Server.Subnautica.Models.Packets.Processors;

internal sealed class ResourceTrackerEntriesChangedProcessor(ResourceTrackerManager resourceTrackerManager) : IAuthPacketProcessor<ResourceTrackerEntriesChanged>
{
    private readonly ResourceTrackerManager resourceTrackerManager = resourceTrackerManager;

    public async Task Process(AuthProcessorContext context, ResourceTrackerEntriesChanged packet)
    {
        resourceTrackerManager.ApplyChanges(packet.Added, packet.Removed);
        await context.SendToOthersAsync(packet);
    }
}
