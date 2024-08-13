using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
    [SerializeField] private GameObject kayakMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject workshopMenu;
    [SerializeField] private GameObject levelMenu;
    [SerializeField] private GameObject shopMenu;
    [SerializeField] private GameObject credits;

    private Vector3 startPosition = new Vector3 (-2500, 0, 0);
    private Vector3 activePositionWorkshop = new Vector3(0, 0, 0);
    private Vector3 activePosition = new Vector3 (-Screen.width / 6, 0, 0);

    [SerializeField] private Transform targetTransform;
    private GameObject targetMenu = null;

    public delegate void OnWorkShopClicked();
    public static OnWorkShopClicked onWorkShopClicked;
    public static OnWorkShopClicked onWorkShopClosed;

    Workshop workshop;
    GameObject PlayerGO;

    private void OnEnable() 
    {
        onWorkShopClicked += DisplayBoats;
    }
    private void OnDisable()
    {
        onWorkShopClicked -= DisplayBoats;
    }

    private void Start()
    {
        workshop = FindObjectOfType<Workshop>();
    }

    public void OnEasyLevel1Pressed()
    {
        SpawnPlayer(new Vector3(-18.75f, 1.75f, -144f), "Level 1");
    }
    
    public void OnMediumLevel2Pressed()
    {
        SpawnPlayer(new Vector3(48.62f, 0.66f, 3.65f), "Level 2");
    }
    
    public void OnHardLevel3Pressed()
    {
        SpawnPlayer(new Vector3(199f, 131.81f, 118.45f), "Level 3");
    }

    private void SpawnPlayer(Vector3 spawnPos, string levelName)
    {
        PlayerGO = Instantiate(workshop.selectedKayak.PlayerPrefab, spawnPos, Quaternion.identity);
        if (workshop.selectedKayak.Index == 1)
            PlayerGO.transform.Find("boat_2/Bone/Torus.004").GetComponent<MeshRenderer>().material = workshop.selectedKayak.CurrentMaterial;
        else
            PlayerGO.GetComponentInChildren<MeshRenderer>().material = workshop.selectedKayak.CurrentMaterial;
        DontDestroyOnLoad(PlayerGO);
        PlayerGO.SetActive(false);
        SaveLoad.Save();
        LoadingList.Set(new List<string>() { levelName });
        SceneManager.LoadScene("LoadingScreen");
    }

    public void OnSettingsPressed()
    {
        settingsMenu.transform.DOLocalMove(new Vector3(activePositionWorkshop.x, transform.position.y, transform.position.z), 1, false);
        targetMenu = settingsMenu;
    }
    public void OnSettingsClosed()
    {
        targetMenu.transform.DOLocalMove(startPosition, 1, false);
    }
    public void OnKayakPressed()
    {
        kayakMenu.transform.DOLocalMove(new Vector3(activePositionWorkshop.x, transform.position.y, transform.position.z), 1, false);
        targetMenu = kayakMenu;
    }
    public void OnWorkshopPressed()
    {
        onWorkShopClicked?.Invoke();
    }
    private void DisplayBoats()
    {
        targetMenu = workshopMenu;
        workshopMenu.transform.DOLocalMove(new Vector3(activePositionWorkshop.x, transform.position.y, transform.position.z) , 1, false);
    }

    public void OnPlayPressed()
    {
        levelMenu.transform.DOLocalMove(new Vector3(activePositionWorkshop.x, transform.position.y, transform.position.z), 1, false);
        targetMenu = levelMenu;
    }
    public void OnShopPressed()
    {
        onWorkShopClosed?.Invoke();
        shopMenu.transform.DOLocalMove(new Vector3(activePositionWorkshop.x, transform.position.y, transform.position.z), 1, false);
        targetMenu = shopMenu;
    }
    public void OnShopClosed()
    {
        onWorkShopClicked?.Invoke();
        shopMenu.transform.DOLocalMove(startPosition, 1, false);
        targetMenu = workshopMenu;
    }

}
