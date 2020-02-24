using UnityEngine;

public class Fader : MonoBehaviour
{
    [SerializeField] private Animator animator;

    void Start()
    {
        GameEvents.Instance.OnSpikeTriggerEntered += FadeOut;
    }

    public void FadeOut()
    {
        animator.SetTrigger("FadeOut");
    }
}
