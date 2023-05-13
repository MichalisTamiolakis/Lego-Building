using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public Animator animator;
    public string animatorGripParameter = "Grip";
    public string animatorTriggerParameter = "Trigger";
    [SerializeField] private float animationSpeed = 1.0f;

    private float gripTarget = 0;
    private float triggerTarget = 0;

    private float currentGrip = 0;
    private float currentTrigger = 0;

    public void SetGrip(float value)
    {
        gripTarget = value;
    }

    internal void SetTrigger(float value)
    {
        triggerTarget = value;

        if(triggerTarget > 0.01)
            print("Setting Trigger");
    }


    private void Update()
    {
        Animate();   
    }

    internal void Reset()
    {
        animator = GetComponent<Animator>();
    }

    void Animate()
    {
        if (currentGrip != gripTarget)
        {
            currentGrip = Mathf.MoveTowards(currentGrip, gripTarget, Time.deltaTime * animationSpeed);

            animator.SetFloat(animatorGripParameter, currentGrip);
        }

        if (currentTrigger != triggerTarget)
        {
            currentTrigger = Mathf.MoveTowards(currentTrigger, triggerTarget, Time.deltaTime * animationSpeed);

            animator.SetFloat(animatorTriggerParameter, currentTrigger);
        }
    }

}
