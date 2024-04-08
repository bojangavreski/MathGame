using System.Diagnostics.CodeAnalysis;

namespace MathGame.Models.Models.UserInGameSession;
public class EnqueuedUser : IEqualityComparer<EnqueuedUser>, IComparable<EnqueuedUser>
{
    public string UserEmail { get; set; }

    public DateTime InsertedAt { get; set; }

    public int CompareTo(EnqueuedUser? other)
    {
        if (other == null) return 0;

        return InsertedAt.CompareTo(other.InsertedAt);
    }

    public bool Equals(EnqueuedUser? x, EnqueuedUser? y) => x.UserEmail == y.UserEmail;

    public int GetHashCode([DisallowNull] EnqueuedUser obj) => obj.UserEmail.GetHashCode();
}
