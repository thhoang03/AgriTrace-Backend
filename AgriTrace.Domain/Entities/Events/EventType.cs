using AgriTrace.Domain.Common;

namespace AgriTrace.Domain.Entities.Events;

public class EventType : BaseEntity
{
    public string Code { get; private set; }

    public string Name { get; private set; }

    private readonly List<SupplyChainEvent> _events = new();

    public IReadOnlyCollection<SupplyChainEvent> SupplyChainEvents
        => _events.AsReadOnly();

    private EventType()
    {

    }

    public EventType(
        string code,
        string name)
    {
        Validate(code, name);

        Code = code.Trim();
        Name = name.Trim();
    }

    public void Update(
        string code,
        string name)
    {
        Validate(code, name);

        Code = code.Trim();
        Name = name.Trim();
    }

    private static void Validate(
        string code,
        string name)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Code is required.");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.");
    }
}
