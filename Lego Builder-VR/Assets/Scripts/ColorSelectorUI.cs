using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorSelectorUI : MonoBehaviour
{
    public List<ButtonColorPair> buttons = new List<ButtonColorPair>();

    public List<BrickSpawner> spawners = new List<BrickSpawner>();

    private void Start()
    {
        foreach(var button in buttons)
        {
            button.button.onClick.AddListener(() => ChangeColor(button.color));
        }
    }
    public void ChangeColor(LegoBrick.BrickColor color)
    {
        foreach(var spawner in spawners)
        {
            spawner.SetColor(color);
        }
        foreach(var button in buttons)
        {
            button.button.interactable = button.color != color;
        }
    }
}


[System.Serializable]
public class ButtonColorPair
{
    public Button button;
    public LegoBrick.BrickColor color;
}
