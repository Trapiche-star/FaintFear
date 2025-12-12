using System;

public interface IInteractable
{
    event Action OnComplete;
    void Interact();
}
