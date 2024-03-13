using Backend;
using TMPro;
using UnityEngine;

// Original Author: Andy Wang
// Populate the view children scene with your children
public class ChildPopulator : MonoBehaviour
{
    [SerializeField] GameObject childPrefab;
    [SerializeField] GameObject gridContent;  // grid container for the buttons

    // Start is called before the first frame update
    void Start()
    {
        gridContent.transform.SetParent(gridContent.transform.parent, false); // do this so scaling works

        foreach (Child child in GameState.s_Player.Family.Children)
        {
            GameObject btnClone = Instantiate(childPrefab);
            SetUpChild(btnClone, child);
            btnClone.transform.SetParent(gridContent.transform, false);

            // todo: resize the grid content
        }
    }

    void SetUpChild(GameObject clone, Child child)
    {

    }
}