using Backend;
using UnityEngine;
using UnityEngine.UI;

// Original Author: Andy Wang
// Used to make a button advance phase on click
public class AdvancePhaseOnClick : MonoBehaviour
{
    [SerializeField] [Range(1, 3)] int phase;  // should be from 1 to 3, but it's zero-indexed

    void Start() {
        // If on an object with a button component, assign it automatically
        Button btn = GetComponent<Button>();

        if (btn) {
            btn.onClick.AddListener(OnClick);
        }
    }

    public void OnClick() {

        if (phase == 1) {
            GameState.AdvanceToPhaseTwo();
        } else if (phase == 2) {
            GameState.AdvanceToPhaseThree();
        } else {
            GameState.AdvanceToPhaseOne();
        }
        phase = GameState.s_Phase;

        UpdateGUI();
    }

    private void UpdateGUI() {
        FarmGUIManager[] guiManagers = FindObjectsOfType<FarmGUIManager>();
        foreach (FarmGUIManager manager in guiManagers) {
            manager.UpdateVisibility();
        }
    }
}
