namespace NPCs.NpcCharacter.NpcCharacterState
{
    public abstract class NpcCharacterState
    {
        /// <summary>
        /// How the NPC character should behave on update.
        /// </summary>
        public abstract void OnUpdate();

        /// <summary>
        /// Invoked when this state becomes active.
        /// </summary>
        public abstract void OnStateBecameActive();
    }
}