/// <summary>
/// An interface for game states that can be submitted, allowing for
/// logic to be executed upon submission.
/// </summary>
public interface ISubmittableState
{
    void OnSubmit();
}
