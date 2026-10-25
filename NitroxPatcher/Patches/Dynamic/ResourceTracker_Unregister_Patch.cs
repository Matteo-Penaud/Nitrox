using System.Reflection;
using NitroxClient.GameLogic;

namespace NitroxPatcher.Patches.Dynamic;

/// <summary>
/// Broadcasts resources removed from the ResourceTrackerDatabase (picked up, broken, scanned...) because other clients won't remove them by themselves.
/// </summary>
public sealed partial class ResourceTracker_Unregister_Patch : NitroxPatch, IDynamicPatch
{
    private static readonly MethodInfo TARGET_METHOD = Reflect.Method((ResourceTracker t) => t.Unregister());

    public static void Prefix(ResourceTracker __instance, out bool __state)
    {
        __state = ResourceTrackers.IsRegistered(__instance);
    }

    public static void Postfix(ResourceTracker __instance, bool __state)
    {
        if (__state && !ResourceTrackers.IsRegistered(__instance))
        {
            Resolve<ResourceTrackers>().BroadcastUnregistered(__instance);
        }
    }
}
