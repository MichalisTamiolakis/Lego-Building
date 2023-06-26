using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationPlayer : MonoBehaviour
{

    public float speed = 1f;
    public bool loop = true;
    public float loopDelay = 2f;
    public List<AnimationFrame> animation = new List<AnimationFrame>();

    public UnityEvent onLooped = new UnityEvent();


    public void StartAnimation()
    {
        StartCoroutine(PlayAnimation());
    }

    public void StopAnimation()
    {
        StopCoroutine(PlayAnimation());
    }

    public IEnumerator PlayAnimation()
    {
        if(animation.Count == 0)
        {
            yield break;
        }

        if(animation.Count < 2)
        {
            transform.localPosition = animation[0].position;
            transform.localRotation = animation[0].rotation;
            yield break;
        }


        float time = 0;
        float interpolationFactor;
        int currFrameIndex = 0;
        AnimationFrame currFrame = animation[0];
        AnimationFrame nextFrame = animation[1];
        while (true)
        {
            time +=Time.deltaTime * speed;
            if (time > nextFrame.timestamp)
            {
                currFrame = animation[currFrameIndex+1];
                nextFrame = animation[currFrameIndex + 2];
                currFrameIndex++;
            }

            interpolationFactor = Mathf.Clamp01((time - currFrame.timestamp) / (nextFrame.timestamp - currFrame.timestamp));

            transform.localPosition = Vector3.Lerp(currFrame.position, nextFrame.position, interpolationFactor);
            transform.localRotation = Quaternion.Lerp(currFrame.rotation, nextFrame.rotation, interpolationFactor);

            if (animation.Count-1 <= currFrameIndex + 1)
            {
                transform.localPosition = nextFrame.position;
                transform.localRotation = nextFrame.rotation;
                if (!loop)
                {
                    onLooped?.Invoke();
                    yield break;
                }

                currFrame = animation[0];
                nextFrame = animation[1];
                currFrameIndex = 0;
                time = 0;
                yield return new WaitForSeconds(loopDelay);
                onLooped?.Invoke();
            }
            yield return new WaitForEndOfFrame();
        }
    }
}
