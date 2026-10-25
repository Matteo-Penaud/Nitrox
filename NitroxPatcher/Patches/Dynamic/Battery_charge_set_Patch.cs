using System;
using System.Reflection;
using NitroxClient.GameLogic;
using Nitrox.Model.DataStructures;

namespace NitroxPatcher.Patches.Dynamic;

public sealed partial class Battery_charge_set_Patch : NitroxPatch, IDynamicPatch
{
    public static readonly MethodInfo TARGET_METHOD = Reflect.Property((Battery t) => t.charge).SetMethod;

    public static void Prefix(Battery __instance, float value, out bool __state)
    {
        // Broadcast update only once per integer change
        __state = Math.Abs(Math.Floor(__instance.charge) - Math.Floor(value)) > 0.0;
    }

    public static void Postfix(Battery __instance, bool __state)
    {
        // Metadata must be extracted after the setter ran, otherwise the previous charge is sent
        if (__state && __instance.TryGetIdOrWarn(out NitroxId id))
        {
            Resolve<Entities>().EntityMetadataChanged(__instance, id);
        }
    }
}
