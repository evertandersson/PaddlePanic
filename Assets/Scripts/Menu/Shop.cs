using DG.Tweening;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static Shop;

[System.Serializable]
public class Purchasable
{
    public string Name;
    public int Price;
    public bool IsPurchased;
    public Image BoughtImage;
    public RawImage PriceText;
    public RawImage LockIcon;
    public RawImage SelectedBox;
}

[System.Serializable]
public class Kayak : Purchasable
{
    public int Index;
    public GameObject PlayerPrefab;
    public Material CurrentMaterial;
}

[System.Serializable]
public class KayakColor : Purchasable
{
    public Material[] MaterialForBoatTypes = new Material[3];
}

public class Shop : MonoBehaviour
{
    public TextMeshProUGUI starText;

    public RawImage PriceDisplay;
    public RawImage ColorPriceDisplay;
    public RawImage AreYouSure;
    public RawImage AreYouSureColor;

    public Kayak[] kayaks = new Kayak[3];
    public KayakColor[] kayakColors = new KayakColor[3];

    public int currentBoat = -1;
    public int currentColor = -1;

    private void Awake()
    {
        SaveLoad.Load();
        foreach (string unlock in SaveLoad.GetUnlocked())
        {
            Debug.Log(unlock);
        }

        starText.text = "STARS: " + SaveLoad.Stars;

        kayaks[0].Name = "Kayak 1";
        kayaks[0].Price = 0;
        kayaks[0].IsPurchased = true;
        kayaks[0].Index = 0;

        kayaks[1].Name = "Kayak 2";
        kayaks[1].Price = 5;
        kayaks[1].IsPurchased = SaveLoad.IsUnlocked(kayaks[1].Name);
        kayaks[1].Index = 1;

        kayaks[2].Name = "Kayak 3";
        kayaks[2].Price = 10;
        kayaks[2].IsPurchased = SaveLoad.IsUnlocked(kayaks[2].Name);
        kayaks[2].Index = 2;

        kayakColors[0].Name = "Orange/White/Grey";
        kayakColors[0].Price = 0;
        kayakColors[0].IsPurchased = true;

        kayakColors[1].Name = "Red/Pink/Grey";
        kayakColors[1].Price = 2;
        kayakColors[1].IsPurchased = SaveLoad.IsUnlocked(kayakColors[1].Name);

        kayakColors[2].Name = "Purple/Pink/Grey";
        kayakColors[2].Price = 5;
        kayakColors[2].IsPurchased = SaveLoad.IsUnlocked(kayakColors[2].Name);

        for (int i = 1; i < kayaks.Length; i++)
        {
            if (kayaks[i].IsPurchased)
            {
                UpdateDisplay(kayaks[i]);
            }
        }
        for (int i = 1; i < kayakColors.Length; i++)
        {
            if (kayakColors[i].IsPurchased)
            {
                UpdateColorDisplay(kayakColors[i]);
            }
        }
    }

    public void ShowAreYouSureDiplay(RawImage image, Purchasable purchasable)
    {
        if (!purchasable.IsPurchased && SaveLoad.Stars >= purchasable.Price)
        {
            image.gameObject.SetActive(true);
        }
    }

    public void SetCurrentBoat(int boat)
    {
        currentBoat = boat;
        ShowAreYouSureDiplay(AreYouSure, kayaks[boat]);
    }
    public void SetCurrentColor(int color)
    {
        currentColor = color;
        ShowAreYouSureDiplay(AreYouSureColor, kayakColors[color]);
    }

    public void BuyKayak()
    {   
        for (int i = 0; i < kayaks.Length; i++)
        {
            if (i == currentBoat)
            {
                kayaks[i] = KayakBought(kayaks[i]);
            }
        }
    }
    public void BuyColor()
    {
        for (int i = 0; i < kayakColors.Length; i++)
        {
            if (i == currentColor)
            {
                kayakColors[i] = ColorBought(kayakColors[i]);
            }
        }
    }

    Kayak KayakBought(Kayak kayak)
    {
        if (SaveLoad.Stars >= kayak.Price && !kayak.IsPurchased)
        {
            kayak.IsPurchased = true;
            SaveLoad.Stars -= kayak.Price;
            
            SaveLoad.SetStars(SaveLoad.Stars);
            SaveLoad.Unlock(kayak.Name);
            SaveLoad.Save();
            
            starText.text = "STARS: " + SaveLoad.Stars;
            UpdateDisplay(kayak);

        }
        currentBoat = -1;
        return kayak;
    }
    private Kayak UpdateDisplay(Kayak kayak)
    {
        kayak.BoughtImage.gameObject.SetActive(true);
        kayak.PriceText.texture = PriceDisplay.texture;
        kayak.PriceText.SetNativeSize();
        kayak.PriceText.transform.position = new Vector3(kayak.PriceText.transform.position.x, kayaks[0].PriceText.transform.position.y, kayak.PriceText.transform.position.z);
        Destroy(kayak.PriceText.GetComponentInChildren<TextMeshProUGUI>());
        kayak.LockIcon.gameObject.SetActive(false);
        return kayak;
    }

    private KayakColor ColorBought(KayakColor color)
    {
        if (SaveLoad.Stars >= color.Price && !color.IsPurchased)
        {
            color.IsPurchased = true;
            SaveLoad.Stars -= color.Price;

            SaveLoad.SetStars(SaveLoad.Stars);
            SaveLoad.Unlock(color.Name);
            SaveLoad.Save();

            starText.text = "STARS: " + SaveLoad.Stars;
            UpdateColorDisplay(color);

        }
        currentColor = -1;
        return color;
    }
    private KayakColor UpdateColorDisplay(KayakColor color)
    {
        color.PriceText.texture = ColorPriceDisplay.texture;
        color.PriceText.SetNativeSize();
        color.PriceText.transform.position = new Vector3(kayakColors[0].PriceText.transform.position.x, color.PriceText.transform.position.y, color.PriceText.transform.position.z);
        Destroy(color.PriceText.GetComponentInChildren<TextMeshProUGUI>());
        color.LockIcon.gameObject.SetActive(false);
        return color;
    }
}
