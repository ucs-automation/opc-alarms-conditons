using Opc.Ua;

namespace JbiOpcUa;

public class FilterDefinition
{
    /// <summary>
    /// The area that should be filtered.
    /// </summary>
    public NodeId AreaId { get; set; }
}