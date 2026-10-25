using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace NitroxPatcher.Patches.Dynamic;

public sealed partial class SubControl_Update_Patch : NitroxPatch, IDynamicPatch
{
    public static readonly MethodInfo TARGET_METHOD = Reflect.Method((SubControl t) => t.Update());

    /*
     * When piloting, the floodlights are toggled with:
     *     transform.parent.BroadcastMessage("ToggleFloodlights", null, SendMessageOptions.DontRequireReceiver);
     * SubControl is on the Cyclops' root so its parent is the Global Root, which toggles the floodlights of every Cyclops.
     * We remove the ".parent" so that the message is only broadcasted to the piloted Cyclops:
     *     transform.BroadcastMessage("ToggleFloodlights", null, SendMessageOptions.DontRequireReceiver);
     */
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return new CodeMatcher(instructions).MatchStartForward(new CodeMatch(OpCodes.Callvirt, Reflect.Property((Transform t) => t.parent).GetMethod),
                                                               new CodeMatch(OpCodes.Ldstr, nameof(CyclopsLightingPanel.ToggleFloodlights)))
                                            .ThrowIfInvalid($"Could not find the floodlights broadcast in {nameof(SubControl)}.{nameof(SubControl.Update)}")
                                            .RemoveInstruction()
                                            .InstructionEnumeration();
    }
}
