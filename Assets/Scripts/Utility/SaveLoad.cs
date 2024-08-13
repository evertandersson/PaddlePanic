using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveLoad
{
    private static SaveFile save;
    public static int Stars { get { return save.stars; } set { save.stars = value; } }

    public static SaveFile Load() {
        string savePath = Application.persistentDataPath + "/gamedata.json";

        if (File.Exists(savePath)) {
            string json = File.ReadAllText(savePath);
            if (!string.IsNullOrEmpty(json)) {
                save = JsonUtility.FromJson<SaveFile>(json);
            } else {
                InitializeSaveFile();
                Save();
            }
        } else {
            File.Create(savePath).Close();
            InitializeSaveFile();
            Save();
        }
        return save;
    }

    private static void InitializeSaveFile() {
        save = new SaveFile();
        save.unlocks = new List<string>();
        Unlock("Kayak 1");
    }

    public static void Save() {
        string savePath = Application.persistentDataPath + "/gamedata.json";
        string json = JsonUtility.ToJson(save);
        File.WriteAllText(savePath, json);
    }

    public static void AddStars(int stars) {
        save.stars += stars;
    }
    public static void SetStars(int stars) {
        save.stars = stars;
    }

    public static void Unlock(string name) {
        if(!save.unlocks.Contains(name)) 
            save.unlocks.Add(name);
    }
    public static void Lock(string name) {
        if (save.unlocks.Contains(name))
            save.unlocks.Remove(name);
    }
    public static bool IsUnlocked(string name) {
        return save.unlocks.Contains(name);
    }
    public static void SelectBoat(int boat) {
        save.selectedBoat = boat;
    }
    public static int GetSelectedBoat() {
        return save.selectedBoat;
    }
    public static void SelectColor(int color)
    {
        save.selectedColor = color;
    }
    public static int GetSelectedColor()
    {
        return save.selectedColor;
    }
    public static List<string> GetUnlocked()
    {
        Debug.Log(save.unlocks);
        return save.unlocks;
    }
}

public struct SaveFile {
    public int stars;
    public List<string> unlocks;
    public int selectedBoat;
    public int selectedColor;
}