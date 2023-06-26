using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PokeButton : MonoBehaviour
{
    public Image image;
    public Color normalColor;
    public Color pressedColor;
    public Color disabledColor;
    public UnityEvent onClick;

    private bool _disabled = false;
    public bool disabled
    {
        get => _disabled;
        set
        {
              _disabled = value;
            if (_disabled)
            {
                image.color = disabledColor;
            }
            else
            {
                image.color = normalColor;
            }
        }
    } 

    public void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("WristButtons");
    }

    public void PokePressed()
    {
        if(!_disabled)
            image.color = pressedColor;
    }

    public void PokeReleased()
    {
        if (!_disabled)
        {
            image.color = normalColor;
            onClick?.Invoke();
        }
    }

    private void OnDisable()
    {
        if(!_disabled)
            image.color = normalColor;
    }

}
