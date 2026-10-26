using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Nitrox.Model.DataStructures;
using Nitrox.Model.Subnautica.Packets;
using NitroxClient.Communication.Abstract;
using NitroxClient.GameLogic;
using UnityEngine;

namespace NitroxPatcher.Patches.Dynamic;

/// <summary>
/// Items consumed by bioreactors were only removed locally so the server kept them forever, making them reappear on load (and overflow the container).
/// Only the player simulating the base consumes the items and broadcasts their destruction, the other players will receive the EntityDestroyed packet.
/// </summary>
public sealed partial class BaseBioReactor_ProducePower_Patch : NitroxPatch, IDynamicPatch
{
    public static readonly MethodInfo TARGET_METHOD = Reflect.Method((BaseBioReactor t) => t.ProducePower(default));

    /*
     * REPLACE:
     * this.container.RemoveItem(pickupable, true);
     * UnityEngine.Object.Destroy(pickupable.gameObject);
     * WITH:
     * BaseBioReactor_ProducePower_Patch.ConsumeItem(this, pickupable);
     */
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return new CodeMatcher(instructions).MatchStartForward([
                                                new CodeMatch(OpCodes.Ldarg_0),
                                                new CodeMatch(OpCodes.Call, Reflect.Property((BaseBioReactor t) => t.container).GetMethod),
                                                new CodeMatch(OpCodes.Ldloc_S),
                                                new CodeMatch(OpCodes.Ldc_I4_1),
                                                new CodeMatch(OpCodes.Callvirt, Reflect.Method((ItemsContainer t) => t.RemoveItem(default(Pickupable), default(bool)))),
                                                new CodeMatch(OpCodes.Pop),
                                                new CodeMatch(OpCodes.Ldloc_S),
                                                new CodeMatch(OpCodes.Callvirt, Reflect.Property((Component t) => t.gameObject).GetMethod),
                                                new CodeMatch(OpCodes.Call, Reflect.Method(() => Object.Destroy(default(Object))))
                                            ])
                                            .Advance(1)
                                            .RemoveInstruction()
                                            .Advance(1)
                                            .RemoveInstructions(6)
                                            .Insert(new CodeInstruction(OpCodes.Call, Reflect.Method(() => ConsumeItem(default, default))))
                                            .InstructionEnumeration();
    }

    public static void ConsumeItem(BaseBioReactor bioReactor, Pickupable pickupable)
    {
        // In case we don't locally simulate the base, the item will be removed by the EntityDestroyed packet from the simulating player
        if (!Resolve<SimulationOwnership>().IsSimulatingBaseOf(bioReactor))
        {
            return;
        }

        bioReactor.container.RemoveItem(pickupable, true);

        if (pickupable.TryGetIdOrWarn(out NitroxId id))
        {
            Resolve<IPacketSender>().Send(new EntityDestroyed(id));
        }

        Object.Destroy(pickupable.gameObject);
    }
}
