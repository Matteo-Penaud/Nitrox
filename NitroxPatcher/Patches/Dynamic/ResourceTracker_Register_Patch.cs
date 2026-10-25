using System.Reflection;
using NitroxClient.GameLogic;

namespace NitroxPatcher.Patches.Dynamic;

/// <summary>
/// Broadcasts resources newly added to the ResourceTrackerDatabase so that scanner rooms can find them in later sessions or on other clients.
/// </summary>
public sealed partial class ResourceTracker_Register_Patch : NitroxPatch, IDynamicPatch
{
    private static readonly MethodInfo TARGET_METHOD = Reflect.Method((ResourceTracker t) => t.Register());

    public static void Prefix(ResourceTracker __instance, out bool __state)
    {
        __state = ResourceTrackers.IsRegistered(__instance);
    }

    public static void Postfix(ResourceTracker __instance, bool __state)
    {
        // Register is also called to update the position of already registered resources, which we don't sync
        if (!__state && ResourceTrackers.IsRegistered(__instance))
        {
            Resolve<ResourceTrackers>().BroadcastRegistered(__instance);
        }
    }
}
