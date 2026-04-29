public interface IInteractable
{
    void Interact();
    bool CanInteract();

    void onFocusOn();
    void onFocusOff();

    void hideChildSprite();
    void showChildSprite();
}