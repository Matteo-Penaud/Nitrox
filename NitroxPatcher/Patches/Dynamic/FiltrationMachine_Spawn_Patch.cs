using System.Reflection;
using NitroxClient.GameLogic;

namespace NitroxPatcher.Patches.Dynamic;

/// <summary>
/// Every player runs the filtration machine's timers so each of them was spawning its own water and salt, overflowing the container.
/// Only the player simulating the base spawns them, the other players will receive the spawned items.
/// </summary>
public sealed partial class FiltrationMachine_Spawn_Patch : NitroxPatch, IDynamicPatch
{
    private static readonly MethodInfo TARGET_METHOD = Reflect.Method((FiltrationMachine t) => t.Spawn(default));

    public static bool Prefix(FiltrationMachine __instance, ref bool __result)
    {
        if (Resolve<SimulationOwnership>().IsSimulatingBaseOf(__instance))
        {
            return true;
        }

        __result = false;
        return false;
    }
}
