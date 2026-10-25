using System;
using System.Runtime.Serialization;
using BinaryPack.Attributes;
using Nitrox.Model.DataStructures.Unity;

namespace Nitrox.Model.Subnautica.DataStructures.GameLogic;

/// <summary>
///     A resource known by the ResourceTrackerDatabase (which powers the scanner room).
/// </summary>
[Serializable, DataContract]
public class ResourceTrackerEntry
{
    [DataMember(Order = 1)]
    public string UniqueId { get; set; }

    [DataMember(Order = 2)]
    public NitroxTechType TechType { get; set; }

    [DataMember(Order = 3)]
    public NitroxVector3 Position { get; set; }

    [IgnoreConstructor]
    protected ResourceTrackerEntry()
    {
        // Constructor for serialization. Has to be "protected" for json serialization.
    }

    public ResourceTrackerEntry(string uniqueId, NitroxTechType techType, NitroxVector3 position)
    {
        UniqueId = uniqueId;
        TechType = techType;
        Position = position;
    }

    public override string ToString()
    {
        return $"[{nameof(ResourceTrackerEntry)} {nameof(UniqueId)}: {UniqueId}, {nameof(TechType)}: {TechType}, {nameof(Position)}: {Position}]";
    }
}
