using UnityEngine;
using Backend;
using UnityEngine.UI;
using TMPro;
using System;

// Original Author: Andy Wang
public class FarmPlotCell : MonoBehaviour
{
    [SerializeField] Sprite normalSprite;
    [SerializeField] Sprite normalSelectedSprite;
    [SerializeField] Sprite wheatSprite;
    [SerializeField] Sprite wheatSelectedSprite;

    private Button _btn;
    private GameObject _hycImage;
    private GameObject _lowImage;
    private GameObject _highImage;
    private GameObject _irrigatedOverlay;
    private Image _image;

    public FarmPlot Plot { get; set; }

    void Start()
    {
        _btn = GetComponent<Button>();
        _btn.onClick.AddListener(HandleClick);

        _hycImage = gameObject.transform.Find("HYC").gameObject;
        _lowImage = gameObject.transform.Find("Low").gameObject;
        _highImage = gameObject.transform.Find("High").gameObject;
        _irrigatedOverlay = gameObject.transform.Find("Irrigated").gameObject;
        _image = GetComponent<Image>();
    }

    // Update this cell visually based on the attributes of the assigned farm plot
    void Update()
    {
        RefreshVisuals();
        RefreshStatus();
    }

    // Refreshes visual state of the farm plot cell
    public void RefreshVisuals()
    {
        if (GameState.s_Phase == 2)
        {
            if (FarmManager.SelectedCells.Contains(this))
            {
                _image.sprite = wheatSelectedSprite;
            }
            else
            {
                _image.sprite = wheatSprite;
            }
        }
        else
        {
            if (FarmManager.SelectedCells.Contains(this))
            {
                _image.sprite = normalSelectedSprite;
            }
            else
            {
                _image.sprite = normalSprite;
            }
        }
    }

    public void RefreshStatus()
    {
        _hycImage.SetActive(Plot.SeedType == SeedType.HYC);

        if (Plot.FertilizerType == FertilizerType.Low)
        {
            _lowImage.SetActive(true);
            _highImage.SetActive(false);
        }
        else if (Plot.FertilizerType == FertilizerType.High)
        {
            _lowImage.SetActive(false);
            _highImage.SetActive(true);
        }
        else
        {
            _lowImage.SetActive(false);
            _highImage.SetActive(false);
        }

        _irrigatedOverlay.SetActive(Plot.Irrigated);
    }

    // Select or deselect this farm cell on click
    // Also handle planting/harvesting/irrigating depending on phase (read FarmManager fields)
    // Precondition: if the player has a tool selected, that means they can use it in the first place
    void HandleClick()
    {
        Household owner = Plot.Owner;
        Inventory inventory = owner.Inventory;

        // Phase 3 - planting phase
        if (GameState.s_Phase == 3)
        {
            // Add or remove HYC Seed
            if (FarmManager.SelectedTool == "HYC Seed")
            {
                if (Plot.SeedType == SeedType.Regular && inventory.Contains("HYC Seed"))
                {
                    Plot.SeedType = SeedType.HYC;
                    inventory.RemoveItem("HYC Seed");
                }
                else if (Plot.SeedType == SeedType.HYC)
                {
                    Plot.SeedType = SeedType.Regular;
                    inventory.AddItem("HYC Seed");
                }
            }

            // Add or remove fertilizer
            // Must replace current fertilizer type with selected one, must put it back in inventory
            // If cell already has selected fertilizer, tapping it should remove it
            if (FarmManager.SelectedTool == "Low Fertilizer" || FarmManager.SelectedTool == "High Fertilizer")
            {
                // if this becomes too cumbersome, create a static util function to convert between the two
                string selectedFertilizerName = FarmManager.SelectedTool;
                FertilizerType selectedFertilizer = selectedFertilizerName == "Low Fertilizer" ? FertilizerType.Low : FertilizerType.High;
                string otherFertilizerName = selectedFertilizerName == "High Fertilizer" ? "Low Fertilizer" : "High Fertilizer";

                // no fertilizer? simply add new fertilizer type
                if (Plot.FertilizerType == FertilizerType.None && inventory.Contains(selectedFertilizerName))
                {
                    Plot.FertilizerType = selectedFertilizer;
                    inventory.RemoveItem(selectedFertilizerName);
                }
                else if (Plot.FertilizerType == selectedFertilizer)
                {  // the plot already has the fertilizer you selected, so it should be removed
                    Plot.FertilizerType = FertilizerType.None;
                    inventory.AddItem(selectedFertilizerName);
                }
                else if (inventory.Contains(selectedFertilizerName))
                {  // the plot has the other fertilizer type, so replace it
                    Plot.FertilizerType = selectedFertilizer;
                    inventory.RemoveItem(selectedFertilizerName);
                    inventory.AddItem(otherFertilizerName);
                }
            }
            return;
        }

        // else - phase 1 - irrigation phase, or phase 2 - harvest phase
        int numTubewells = inventory.GetAmount("Tubewell");
        int maxSelectedCells = GameState.s_Phase == 1 ? Math.Min(Farmland.MaxPlots, numTubewells * Market.PlotsPerTubewell) : Farmland.MaxPlots;
        int labourCost = GameState.s_Phase == 1 ? FarmManager.IrrigationLabour : FarmManager.HarvestLabour;

        // Updates selection status in FarmManager, only adds if there is still labour remaining
        if (!FarmManager.SelectedCells.Contains(this))
        {
            if (FarmManager.LabourPoints >= labourCost && FarmManager.SelectedCells.Count < maxSelectedCells)
            {
                FarmManager.SelectedCells.Add(this);
                FarmManager.LabourPoints -= labourCost;
            }
        }
        else
        {
            FarmManager.SelectedCells.Remove(this);
            FarmManager.LabourPoints += labourCost;
        }
    }
}