using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AutoDestroyEffect : MonoBehaviour
{
    void Start()
    {
        Animator animator = GetComponent<Animator>();

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        float animationLength = stateInfo.length;

        Destroy(gameObject, animationLength);
    }
}