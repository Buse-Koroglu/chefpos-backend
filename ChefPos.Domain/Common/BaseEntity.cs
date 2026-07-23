namespace ChefPos.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public DateTime? UpdatedAt { get; private set; }


    protected BaseEntity()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }
    
    protected void Touch() => UpdatedAt = DateTime.UtcNow;
    
    public override bool Equals(object? obj)
    {
        if (obj is not BaseEntity other) return false;
        if (ReferenceEquals(this, other)) return true;
        if (GetType() != other.GetType()) return false;
        return Id == other.Id;
    }

    public override int GetHashCode() => (GetType().ToString() + Id).GetHashCode();

    public static bool operator ==(BaseEntity? a, BaseEntity? b)
        => a is null && b is null || (a is not null && a.Equals(b));

    public static bool operator !=(BaseEntity? a, BaseEntity? b) => !(a == b);
}