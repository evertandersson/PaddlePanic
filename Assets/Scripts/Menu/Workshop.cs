using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Workshop : MonoBehaviour
{
    [SerializeField] public Kayak selectedKayak;
    [SerializeField] public KayakColor selectedColor;
    private Shop shop;

    public Material[] boatColorList = new Material[9];

    private void Start()
    {
        shop = FindObjectOfType<Shop>();
        
        for (int i = 0; i < shop.kayakColors[0].MaterialForBoatTypes.Length; i++)
        {
            shop.kayakColors[0].MaterialForBoatTypes[i] = boatColorList[i];
        }
        for (int i = 0; i < shop.kayakColors[1].MaterialForBoatTypes.Length; i++)
        {
            shop.kayakColors[1].MaterialForBoatTypes[i] = boatColorList[i + 3];
        }
        for (int i = 0; i < shop.kayakColors[2].MaterialForBoatTypes.Length; i++)
        {
            shop.kayakColors[2].MaterialForBoatTypes[i] = boatColorList[i + 6];
        }
        ClickButton(SaveLoad.GetSelectedBoat());
    }

    public void ClickButton(int index)
    {
        if (shop.kayaks[index].IsPurchased)
        {
            SaveLoad.SelectBoat(index);
            SaveLoad.Save();
            selectedKayak = shop.kayaks[index];
            for (int i = 0; i < shop.kayaks.Length; i++)
            {
                if (i != index)
                    shop.kayaks[i].SelectedBox.gameObject.SetActive(false);
                else
                    shop.kayaks[i].SelectedBox.gameObject.SetActive(true);
            }
            ClickColorButton(SaveLoad.GetSelectedColor());
        }
    }

    public void ClickColorButton(int index)
    {
        if (shop.kayakColors[index].IsPurchased)
        {
            SaveLoad.SelectColor(index);
            SaveLoad.Save();
            selectedColor = shop.kayakColors[selectedKayak.Index];
            selectedKayak.CurrentMaterial = selectedColor.MaterialForBoatTypes[index];
            for (int i = 0; i < shop.kayakColors.Length; i++)
            {
                if (i != index)
                    shop.kayakColors[i].SelectedBox.gameObject.SetActive(false);
                else
                    shop.kayakColors[i].SelectedBox.gameObject.SetActive(true);
            }
        }
    }
}
