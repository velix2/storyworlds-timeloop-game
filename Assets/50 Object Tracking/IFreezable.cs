namespace Object_Tracking
{
    public interface IFreezable
    {
        /// <summary>
        /// Returns a string identifier that uniquely refers to this freezable object across scene loads.
        /// </summary>
        /// <returns>a string identifier</returns>
        string GetFreezableIdentifier();

        bool CheckForStoredState(out int stateIndex)
        {
            return ObjectTracker.Instance.CheckForStoredState(this, out stateIndex); 
        }
        
        void StoreState(int stateIndex)
        {
             ObjectTracker.Instance.StoreState(this, stateIndex);
        }
        
        void UnfreezeState()
        {
            ObjectTracker.Instance.RemoveState(this);
        }
    }
}