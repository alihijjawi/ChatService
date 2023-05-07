using System.ComponentModel.DataAnnotations;
using Microsoft.Azure.Cosmos.Serialization.HybridRow;

namespace ChatService.Dtos;

public record MessagesList(
    [Required] MessageDto[] Messages,
    [Required] string NextUri)
{
    public virtual bool Equals(MessagesList? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return !Messages.Where((t, i) => t != other?.Messages[i]).Any() && NextUri == other.NextUri;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Messages, NextUri);
    }
};