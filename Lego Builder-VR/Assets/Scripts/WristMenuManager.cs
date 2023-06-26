using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WristMenuManager : MonoBehaviour
{
    public PokeButton undoButton;
    public PokeButton redoButton;
    public PokeButton replayButton;

    private void Start()
    {
        undoButton.onClick.AddListener(OnUndo);
        redoButton.onClick.AddListener(OnRedo);
        replayButton.onClick.AddListener(OnReplay);
        UpdateButtonStates();
    }

    public void OnUndo()
    {
        if (Recorder.Instance.CanUndo())
        {
            Recorder.Instance.Undo();
        }
    }

    public void OnRedo()
    {
        if (Recorder.Instance.CanRedo())
        {
            Recorder.Instance.Redo();
        }
    }

    public void OnReplay()
    {
        Recorder.Instance.ReplayLastCommand();
    }

    private void Update()
    {
        UpdateButtonStates();
    }

    private void UpdateButtonStates()
    {
        undoButton.disabled = !Recorder.Instance.CanUndo();
        redoButton.disabled = !Recorder.Instance.CanRedo();
        replayButton.disabled = !Recorder.Instance.CanReplay();
    }
}
