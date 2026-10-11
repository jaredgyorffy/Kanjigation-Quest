using UnityEngine;

public class PlayerInventory : MonoBehaviour, ILocatableService
{
    public int Hints { get; private set; }
    public bool ShouldUnregisterOnSceneChange => false;
    private void Awake()
    {
        ServiceLocator.Instance.Register(this);
    }

    public void IncreaseHints(int hintAmount = 1)
    {
        Hints += hintAmount;
    }
    public bool TryUseHint()
    {
        if (Hints > 0)
        {
            Hints--;
            return true;
        }
        return false;
    }
}
