using System;

namespace Nekoyume.Model.State
{
    [Serializable]
    public class DeletedAvatarState : AvatarState
    {
        public long deletedAt;

        public DeletedAvatarState(AvatarState avatarState, long blockIndex)
        {
            deletedAt = blockIndex;
        }
    }
}
