using System.Collections.Generic;
using System.Linq;
using Nitrox.Model.Subnautica.DataStructures.GameLogic;

namespace Nitrox.Server.Subnautica.Models.GameLogic;

/// <summary>
///     Thread-safe store of the world's ResourceTrackerDatabase, which is what scanner rooms look up.
/// </summary>
/// <remarks>
///     Subnautica persists this database in its save. It's filled with resources as they get loaded around players
///     so we need to keep it on the server for scanner rooms to detect resources that aren't currently loaded.
/// </remarks>
internal sealed class ResourceTrackerManager
{
    private readonly Lock entriesLock = new();
    private readonly Dictionary<string, ResourceTrackerEntry> entriesById = [];

    public void SetEntries(IEnumerable<ResourceTrackerEntry> entries)
    {
        lock (entriesLock)
        {
            entriesById.Clear();
            foreach (ResourceTrackerEntry entry in entries)
            {
                entriesById[entry.UniqueId] = entry;
            }
        }
    }

    public List<ResourceTrackerEntry> GetEntries()
    {
        lock (entriesLock)
        {
            return entriesById.Values.ToList();
        }
    }

    public void ApplyChanges(IEnumerable<ResourceTrackerEntry> added, IEnumerable<ResourceTrackerEntry> removed)
    {
        lock (entriesLock)
        {
            foreach (ResourceTrackerEntry entry in added)
            {
                entriesById[entry.UniqueId] = entry;
            }
            foreach (ResourceTrackerEntry entry in removed)
            {
                entriesById.Remove(entry.UniqueId);
            }
        }
    }
}
