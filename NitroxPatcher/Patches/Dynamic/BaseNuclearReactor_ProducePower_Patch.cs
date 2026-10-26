using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using NitroxClient.GameLogic;
using UnityEngine;

namespace NitroxPatcher.Patches.Dynamic;

/// <summary>
/// Every player runs the nuclear reactor so each of them was replacing the consumed rod with its own depleted rod.
/// Only the player simulating the base replaces the rod, the other players will receive the rod's removal and the depleted rod's spawn.
/// </summary>
public sealed partial class BaseNuclearReactor_ProducePower_Patch : NitroxPatch, IDynamicPatch
{
    public static readonly MethodInfo TARGET_METHOD = Reflect.Method((BaseNuclearReactor t) => t.ProducePower(default));

    /*
     * REPLACE:
     * UnityEngine.Object.Destroy(this.equipment.RemoveItem(text, true, false).item.gameObject);
     * base.StartCoroutine(this.AddDepletedRodToEquipmentAsync(text));
     * WITH:
     * BaseNuclearReactor_ProducePower_Patch.ConsumeRod(this, text);
     */
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return new CodeMatcher(instructions).MatchStartForward([
                                                new CodeMatch(OpCodes.Ldarg_0),
                                                new CodeMatch(OpCodes.Call, Reflect.Property((BaseNuclearReactor t) => t.equipment).GetMethod),
                                                new CodeMatch(instruction => instruction.IsLdloc()),
                                                new CodeMatch(OpCodes.Ldc_I4_1),
                                                new CodeMatch(OpCodes.Ldc_I4_0),
                                                new CodeMatch(OpCodes.Callvirt, Reflect.Method((Equipment t) => t.RemoveItem(default(string), default(bool), default(bool)))),
                                                new CodeMatch(OpCodes.Callvirt, Reflect.Property((InventoryItem t) => t.item).GetMethod),
                                                new CodeMatch(OpCodes.Callvirt, Reflect.Property((Component t) => t.gameObject).GetMethod),
                                                new CodeMatch(OpCodes.Call, Reflect.Method(() => Object.Destroy(default(Object)))),
                                                new CodeMatch(OpCodes.Ldarg_0),
                                                new CodeMatch(OpCodes.Ldarg_0),
                                                new CodeMatch(instruction => instruction.IsLdloc()),
                                                new CodeMatch(OpCodes.Call, Reflect.Method((BaseNuclearReactor t) => t.AddDepletedRodToEquipmentAsync(default))),
                                                new CodeMatch(OpCodes.Call, Reflect.Method((MonoBehaviour t) => t.StartCoroutine(default(System.Collections.IEnumerator)))),
                                                new CodeMatch(OpCodes.Pop)
                                            ])
                                            .ThrowIfInvalid($"Could not find the rod consumption in {nameof(BaseNuclearReactor)}.{nameof(BaseNuclearReactor.ProducePower)}")
                                            .Advance(1)
                                            .RemoveInstruction()
                                            .Advance(1)
                                            .RemoveInstructions(12)
                                            .Insert(new CodeInstruction(OpCodes.Call, Reflect.Method(() => ConsumeRod(default, default))))
                                            .InstructionEnumeration();
    }

    public static void ConsumeRod(BaseNuclearReactor nuclearReactor, string slotId)
    {
        // In case we don't locally simulate the base, the rod will be replaced by the packets from the simulating player
        if (!Resolve<SimulationOwnership>().IsSimulatingBaseOf(nuclearReactor))
        {
            return;
        }

        // Removing the rod from the equipment broadcasts its destruction, and adding the depleted rod broadcasts its spawn
        Object.Destroy(nuclearReactor.equipment.RemoveItem(slotId, true, false).item.gameObject);
        nuclearReactor.StartCoroutine(nuclearReactor.AddDepletedRodToEquipmentAsync(slotId));
    }
}
