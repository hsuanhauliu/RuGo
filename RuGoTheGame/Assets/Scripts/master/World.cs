using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

public class World : MonoBehaviour
{
    private List<Gadget> gadgetsInWorld;

    void Start()
    {
        gadgetsInWorld = new List<Gadget>();
    }


    public void Reset()
    {
        gadgetsInWorld.ForEach((Gadget g) => g.Reset());
    }

    public void InsertGadget(Gadget g) {
        gadgetsInWorld.Add(g);
    }

    public void Save() {
        Debug.Log("Saving Data");
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create("./world.dat");

        List<GadgetSaveData> saveData = gadgetsInWorld.ConvertAll<GadgetSaveData>((Gadget input) => input.GetSaveData());

        bf.Serialize(file, saveData);
        file.Close();
    }

    public void Load()
    {
        Debug.Log("Loading Data");
        if (File.Exists("./world.dat")) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open("./world.dat", FileMode.Open);

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
        gadgetsInWorld.Add(gadget);
    }

    public void RemoveGadget(Gadget gadget)
    {
        gadgetsInWorld.Remove(gadget);
        gadget.RemoveFromScene();
    }
}

