using System.ComponentModel.DataAnnotations;

namespace ChatService.Dtos;

public record ConversationsList([Required] ConversationDto[] Conversations, [Required] string NextUri)
{
    public virtual bool Equals(ConversationsList? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        if (Conversations.Length != other.Conversations.Length) return false;
        return !Conversations.Where((t, i) => t != other?.Conversations[i]).Any() && NextUri == other.NextUri;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Conversations, NextUri);
    }
}
    