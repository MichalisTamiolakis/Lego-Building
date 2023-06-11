using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class AnimationExecution : MonoBehaviour
{
    public Command command;
    private int currentFrameIndex = 0;
    private bool isAnimating = false;
    private float animationTime = 0f;
    private float animationDuration = 0f;
    [SerializeField] private float animationSpeed = 0.5f;

    void Start()
    {
        // Calculate the animation duration based on the time difference between frames
        animationDuration = command.frames[command.frames.Count - 1].timestamp - command.frames[0].timestamp;
        StartCoroutine(PlayAnimation());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartAnimation()
    {
        isAnimating = true;
        currentFrameIndex = 0;
    }

    public IEnumerator PlayAnimation()
    {
        while (true)
        {
            // Update the animation timer
            animationTime += Time.deltaTime;

            // Calculate the interpolation factor based on the animation time and duration
            //float t = Mathf.Clamp01(animationTime / animationDuration);
            
            // Find the current frame index
            while (currentFrameIndex < command.frames.Count - 1 && command.frames[currentFrameIndex + 1].timestamp <= animationTime)
            {
                currentFrameIndex++;
            }
            //Debug.Log("Current Frame Index: " + currentFrameIndex);
            // Get the current and next frames
            AnimationFrame currentFrame = command.frames[currentFrameIndex];
            AnimationFrame nextFrame;
            if (currentFrameIndex != command.frames.Count - 1)
                nextFrame = command.frames[currentFrameIndex + 1];
            else nextFrame = command.frames[currentFrameIndex];
            float t = Mathf.Clamp01((animationTime - currentFrame.timestamp) / (nextFrame.timestamp - currentFrame.timestamp));
            Debug.Log("T: " + t);
            // Interpolate the position and rotation between the current and next frames
            Vector3 targetPosition = Vector3.Lerp(currentFrame.position, nextFrame.position, t);
            Quaternion targetRotation = Quaternion.Lerp(currentFrame.rotation, nextFrame.rotation, t);

            // Update the transform of the brick
            transform.localPosition = targetPosition;
            transform.localRotation = targetRotation;

            // Check if the animation is complete
            if (animationTime >= animationDuration)
            {
                yield return new WaitForSeconds(2f);
                currentFrameIndex = 0;
                animationTime = 0f;
            }
            yield return new WaitForEndOfFrame();
                
        }
    }
}
