using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

public class World : MonoBehaviour
{
    private List<Gadget> gadgetsInWorld;

    public static String DEFAULT_SAVE_FILE = "./world.dat";
    public static String AUTO_SAVE_FILE = "./autosave.dat";

    void Start()
    {
        gadgetsInWorld = new List<Gadget>();
    }


    public void Reset()
    {
        this.Load(AUTO_SAVE_FILE);
    }

    public void InsertGadget(Gadget g) {
        gadgetsInWorld.Add(g);
        Save(AUTO_SAVE_FILE);
    }

    public void Save(String fileName) {
        Debug.Log("Saving Data to: " + fileName);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(fileName);

        List<GadgetSaveData> saveData = gadgetsInWorld.ConvertAll<GadgetSaveData>((Gadget input) => input.GetSaveData());

        bf.Serialize(file, saveData);
        file.Close();
    }

    public void Load(String fileName)
    {
        Debug.Log("Loading Data from: " + fileName);
        if (File.Exists(fileName)) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(fileName, FileMode.Open);

            List<GadgetSaveData> savedGadgets = (List<GadgetSaveData>)bf.Deserialize(file);

            foreach (Gadget g in gadgetsInWorld) {
                g.RemoveFromScene();
            }

            gadgetsInWorld = savedGadgets.ConvertAll<Gadget>(ConvertSavedDataToGadget);

            file.Close();
        }
    }

    private Gadget ConvertSavedDataToGadget(GadgetSaveData savedGadgetData)
    {
        String prefabName = savedGadgetData.name;
        GameObject gadgetPrefab = Resources.Load(prefabName) as GameObject;
        GameObject savedGameObject = Instantiate(gadgetPrefab, this.transform);

        Gadget g = savedGameObject.GetComponent<Gadget>();
        g.RestoreStateFromSaveData(savedGadgetData);

        return g;
    }

    public void CreateGadgetFromTemplate(Gadget gadgetTemplate)
    {
        Debug.Log("A new gameObject has been created and inserted in the World.");

        GameObject gadgetObj = Instantiate(gadgetTemplate.gameObject, this.transform);
        Gadget gadget = gadgetObj.GetComponent<Gadget>();
        gadget.MakeSolid();
        InsertGadget(gadget);
    }

    public void RemoveGadget(Gadget gadget)
    {
        gadgetsInWorld.Remove(gadget);
        gadget.RemoveFromScene();
        this.Save(AUTO_SAVE_FILE);
    }
}

