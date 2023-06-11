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
 
    void Start()
    {
        // Calculate the animation duration based on the time difference between frames
        animationDuration = command.frames[command.frames.Count - 1].timestamp - command.frames[0].timestamp;
    }

    // Update is called once per frame
    void Update()
    {
        if (isAnimating)
        {
            // Update the animation timer
            animationTime += Time.deltaTime;

            // Calculate the interpolation factor based on the animation time and duration
            float t = Mathf.Clamp01(animationTime / animationDuration);

            // Find the current frame index
            while (currentFrameIndex < command.frames.Count - 1 && command.frames[currentFrameIndex + 1].timestamp <= t)
            {
                currentFrameIndex++;
            }

            // Get the current and next frames
            AnimationFrame currentFrame = command.frames[currentFrameIndex];
            AnimationFrame nextFrame = command.frames[currentFrameIndex + 1];

            // Interpolate the position and rotation between the current and next frames
            Vector3 targetPosition = Vector3.Lerp(currentFrame.position, nextFrame.position, t);
            Quaternion targetRotation = Quaternion.Lerp(currentFrame.rotation, nextFrame.rotation, t);

            // Update the transform of the brick
            transform.localPosition = targetPosition;
            transform.localRotation = targetRotation;

            // Check if the animation is complete
            if (animationTime >= animationDuration)
            {
                currentFrameIndex = 0;
                animationTime = 0f;
                //isAnimating = false;
            }
        }
    }

    public void StartAnimation()
    {
        isAnimating = true;
        currentFrameIndex = 0;
    }
}
