using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

public class World : MonoBehaviour
{
    private List<Gadget> gadgetsInWorld;
    private bool isWorldStateModified = false;

    private string CurrentSaveSlot;

    private readonly string AUTO_SAVE_FILE = "autosave.dat";
    private readonly string SAVED_GAME_DIR = "SavedGames/";
    private GameObject mGadgetShelf;
    private Vector3[] shelfGadgetContainersPositions;
    private Vector3[] shelfFileContainersPositions;

#if RUGO_AR
    private readonly int NUM_REQUIRED_GOALS = 0;    
#elif RUGO_VR
    private readonly int NUM_REQUIRED_GOALS = 2;
#endif

    public Transform[] GoalSpawnLocations;

    public GameObject CubeRoomGeo;
    public GameObject BubblePrefab;
    public static World Instance = null;

    /* Shelf control variables */
    private readonly float shelfRadius = 0.7f;
    private readonly float ShiftRateMin = 2.0f;
    private readonly float ShiftRateMax = 2.5f;
    private readonly float GadgetOffsetMax = 0.2f;
    private readonly float rotationRate = 0.8f;
    private readonly int NUM_SAVE_SLOTS = 4;

    public Material[] RoomMaterials;

    public Color[] RoomColors;

    public Light RoomLight;

    public bool AllGoalsComplete
    {
        get
        {
            List<Gadget> goalGadgets = gadgetsInWorld.FindAll((Gadget obj) => obj is GoalGadget);
            return goalGadgets.TrueForAll((Gadget g) => ((GoalGadget)g).IsGoalComplete);
        }
    }

    private void MakeSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        gadgetsInWorld = new List<Gadget>();
        SpawnGadgetShelf();
        mGadgetShelf.transform.position = new Vector3(100, 100, 100);
        ShowShelf(false);
        LoadLastModifiedSaveSlot();
    }

    private void Awake()
    {
        MakeSingleton();
        mGadgetShelf = transform.Find("GadgetShelf").gameObject;
        InitializeSaveSlots();
    }

    void Update()
    {
        if (isWorldStateModified)
        {
            AutoSave();
            RespawnGadgets();
            isWorldStateModified = false;
        }
    }
    public void NotifyGoalComplete()
    {
        if (AllGoalsComplete)
        {
            GameManager.Instance.ChangeGameMode(GameMode.COMPLETE);

            GameObject[] endGameCelebrations = GameObject.FindGameObjectsWithTag("Goal");

            int celebrationChoice = UnityEngine.Random.Range(0, endGameCelebrations.Length);

            GameObject celebrationObject = endGameCelebrations[celebrationChoice];

            ParticleSystem pSystem = celebrationObject.GetComponent<ParticleSystem>();
            if (pSystem)
            {
                pSystem.Play(true);
            }

            AudioSource celebrationAudio = celebrationObject.GetComponent<AudioSource>();

            if (celebrationAudio)
            {
                celebrationAudio.Play();
            }

            RoomLight.enabled = false;
        }
    }

    public void LoadLastModifiedSaveSlot()
    {
        //TODO: Determine Last Modified Save Slot #
        LoadSaveSlot("0");
    }

    private void InitializeSaveSlots()
    {
        // If the Saved Games Directory does not exist, Create Save Slots
        if (!Directory.Exists(SAVED_GAME_DIR))
        {
            try
            {
                DirectoryInfo savedGamesDirectoryInfo = Directory.CreateDirectory(SAVED_GAME_DIR);
                if (savedGamesDirectoryInfo.Exists)
                {
                    for (int saveSlot = 0; saveSlot < NUM_SAVE_SLOTS; saveSlot++)
                    {
                        Directory.CreateDirectory(SAVED_GAME_DIR + saveSlot + "/");
                        FileStream saveSlotFile = File.Create(SAVED_GAME_DIR + saveSlot + "/" + AUTO_SAVE_FILE);
                        saveSlotFile.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                Application.Quit();
            }

        }
    }

    private void AutoSave()
    {
        string currentSaveSlotFile = SAVED_GAME_DIR + CurrentSaveSlot + "/" + AUTO_SAVE_FILE;

        if (File.Exists(currentSaveSlotFile))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(SAVED_GAME_DIR + CurrentSaveSlot + "/" + AUTO_SAVE_FILE);

            List<GadgetSaveData> saveData = gadgetsInWorld.ConvertAll<GadgetSaveData>((Gadget input) => input.GetSaveData());
            bf.Serialize(file, saveData);
            file.Close();
        }
        else
        {
            Debug.Log("Loading AutoSave Failed. File " + currentSaveSlotFile + "doesn't exist");
        }

    }

    public void LoadCurrentSaveSlot()
    {
        LoadSaveSlot(CurrentSaveSlot);
    }

    public void LoadSaveSlot(string saveSlot)
    {
        CurrentSaveSlot = saveSlot;

        string serializedAutoSaveFile = SAVED_GAME_DIR + CurrentSaveSlot + "/" + AUTO_SAVE_FILE;

        LoadSerializedGadgets(serializedAutoSaveFile);
        AutoSave();
    }

    private void DisableCelebrations()
    {
        GameObject[] endGameCelebrations = GameObject.FindGameObjectsWithTag("Goal");

        foreach (GameObject celebration in endGameCelebrations)
        {
            ParticleSystem pSystem = celebration.GetComponent<ParticleSystem>();
            if (pSystem)
            {
                pSystem.Stop(true);
            }

            AudioSource celebrationAudio = celebration.GetComponent<AudioSource>();

            if (celebrationAudio)
            {
                celebrationAudio.Stop();
            }
        }
    }

    private void LoadSerializedGadgets(string serializedFileName)
    {
        if (File.Exists(serializedFileName))
        {
#if RUGO_VR
            Renderer roomRenderer = CubeRoomGeo.GetComponent<Renderer>();
            //TODO Refactor Everything to use Integer Save Slot
            roomRenderer.material = RoomMaterials[System.Convert.ToInt32(CurrentSaveSlot)];
#elif RUGO_AR
            PlatformContainer.Instance.SetARLightColor(RoomColors[System.Convert.ToInt32(CurrentSaveSlot)]);
#endif

            RoomLight.enabled = true;

            DisableCelebrations();

            BinaryFormatter bf = new BinaryFormatter();
            FileStream fileStream = File.Open(serializedFileName, FileMode.Open);
            RemoveGadgetsFromScene();
            
            if (fileStream.Length != 0)
            {
                List<GadgetSaveData> savedGadgets = (List<GadgetSaveData>)bf.Deserialize(fileStream);
                gadgetsInWorld = savedGadgets.ConvertAll<Gadget>(ConvertSavedDataToGadget);
            }
            SpawnGoalGadgets();

            fileStream.Close();
        }
        else
        {
            Debug.Log("Loading Data failed. File " + serializedFileName + "doesn't exist");
        }
    }

    private int GetGoalGadgetCount()
    {
        return gadgetsInWorld.FindAll((Gadget g) => g is GoalGadget).Count;
    }

    private void SpawnGoalGadgets()
    {
        int numberOfGoalsInScene = GetGoalGadgetCount();


        if (numberOfGoalsInScene == 0)
        {
            HashSet<int> randomSpawnLocations = new HashSet<int>();

            while (randomSpawnLocations.Count < NUM_REQUIRED_GOALS - numberOfGoalsInScene)
            {
                randomSpawnLocations.Add(UnityEngine.Random.Range(0, GoalSpawnLocations.Length));
            }

            foreach (int spawnLocation in randomSpawnLocations)
            {
                SpawnGoalGadgetInSpawnLocation(spawnLocation);
            }
        }
    }

    private void SpawnGoalGadgetInSpawnLocation(int spawnNumber)
    {
        GameObject goalPrefab = Resources.Load("Goal") as GameObject;
        GameObject goalGameObject = Instantiate(goalPrefab, GoalSpawnLocations[spawnNumber]);
        GoalGadget goalGadget = goalGameObject.GetComponent<GoalGadget>();
        goalGadget.SetGoalInWorld();
        InsertGadget(goalGadget);
    }

    private Gadget ConvertSavedDataToGadget(GadgetSaveData savedGadgetData)
    {
        string prefabName = savedGadgetData.name;
        GameObject gadgetPrefab = Resources.Load(prefabName) as GameObject;
        GameObject savedGameObject = Instantiate(gadgetPrefab, this.transform);

        Gadget gadget = savedGameObject.GetComponent<Gadget>();
        gadget.RestoreStateFromSaveData(savedGadgetData);
        gadget.transform.position += this.transform.position;

        return gadget;
    }

    public void ClearCurrentSaveSlot()
    {
        RemoveGadgetsFromScene();
        SpawnGoalGadgets();
        MarkWorldModified();
    }

    private void RemoveGadgetsFromScene()
    {
        foreach (Gadget gadget in gadgetsInWorld)
        {
            gadget.RemoveFromScene();
        }
        gadgetsInWorld = new List<Gadget>();
    }

    public void ShowShelf(bool show)
    {
        if (!show)
        {
            BurstBubble();
            StartCoroutine(DelayHideShelf());
        }
        else
        {
            mGadgetShelf.SetActive(show);
            mGadgetShelf.GetComponent<AudioSource>().Play();

            Transform myCamera = GameManager.Instance.MainCamera.transform;
            Vector3 pureCameraRotation = myCamera.rotation.eulerAngles;

            // Get play area size
            Valve.VR.HmdQuad_t roomSize = new Valve.VR.HmdQuad_t();
            Vector3 offset = Vector3.zero;

            if (SteamVR_PlayArea.GetBounds(SteamVR_PlayArea.Size.Calibrated, ref roomSize))
            {
                Valve.VR.HmdVector3_t[] roomCorners = new Valve.VR.HmdVector3_t[] { roomSize.vCorners0, roomSize.vCorners1, roomSize.vCorners2, roomSize.vCorners3 };
                Vector3[] cornerPositions = new Vector3[roomCorners.Length];
                for (int i = 0; i < roomCorners.Length; i++)
                {
                    cornerPositions[i] = new Vector3(roomCorners[i].v0, roomCorners[i].v1, roomCorners[i].v2);
                }

                // Get two corners
                float minX = 0.0f;
                float minZ = 0.0f;
                float maxX = 0.0f;
                float maxZ = 0.0f;
                for (int i = 0; i < cornerPositions.Length; i++)
                {
                    minX = Math.Min(minX, cornerPositions[i].x);
                    maxX = Math.Max(maxX, cornerPositions[i].x);
                    minZ = Math.Min(minZ, cornerPositions[i].z);
                    maxZ = Math.Max(maxZ, cornerPositions[i].z);
                }

                // Calculate the shelf's position
                Vector3 shelfPosition = myCamera.position + myCamera.forward * shelfRadius;

                if (shelfPosition.x < 0 && shelfPosition.x < minX)
                    offset += new Vector3(minX - shelfPosition.x, 0, 0);
                else if (shelfPosition.x > 0 && shelfPosition.x > maxX)
                    offset -= new Vector3(shelfPosition.x - maxX, 0, 0);

                if (shelfPosition.z < 0 && shelfPosition.z < minZ)
                    offset += new Vector3(0, 0, minZ - shelfPosition.z);
                else if (shelfPosition.z > 0 && shelfPosition.z > maxZ)
                    offset -= new Vector3(0, 0, shelfPosition.z - maxZ);
            }

            mGadgetShelf.transform.position = myCamera.position + offset;
            mGadgetShelf.transform.rotation = Quaternion.Euler(0.0f, myCamera.rotation.eulerAngles.y - 90.0f, 0.0f);

            for (int i = 0; i < shelfGadgetContainersPositions.Length; i++)
            {
                StartCoroutine(ShiftContainer(i, shelfGadgetContainersPositions[i]));
                StartCoroutine(RotateGadget(i));
            }

            for (int i = 0; i < NUM_SAVE_SLOTS; i++)
            {
                StartCoroutine(ShiftContainer(shelfGadgetContainersPositions.Length + i, shelfFileContainersPositions[i]));
                StartCoroutine(RotateGadget(shelfGadgetContainersPositions.Length + i));
            }
        }
    }

    private void BurstBubble()
    {
        int numOfBubbles = mGadgetShelf.transform.childCount;

        for (int i = 0; i < numOfBubbles; i++)
        {
            if (mGadgetShelf.transform.GetChild(i).childCount > 1)
            {
                Gadget gadget = mGadgetShelf.transform.GetChild(i).GetChild(1).GetComponent<Gadget>();
                if (gadget != null && gadget.CurrentGadgetState == Gadget.GadgetState.FirstPlacement)
                {
                    Transform containerBubble = mGadgetShelf.transform.GetChild(i).GetChild(0);
                    containerBubble.GetChild(0).gameObject.SetActive(false);
                    containerBubble.GetChild(1).GetComponent<ParticleSystem>().Play();
                    StartCoroutine(EnableBubble(containerBubble.GetChild(0).gameObject));
                }
            }
        }
    }

    private IEnumerator RotateGadget(int containerIndex)
    {
        if (mGadgetShelf.transform.GetChild(containerIndex).childCount > 1)
        {
            Transform gadgetTransform = mGadgetShelf.transform.GetChild(containerIndex).GetChild(1);
            while (mGadgetShelf.activeSelf && gadgetTransform != null)
            {
                Vector3 currentRotation = gadgetTransform.localRotation.eulerAngles;
                gadgetTransform.localRotation = Quaternion.Euler(currentRotation.x, currentRotation.y + rotationRate, currentRotation.z);
                yield return null;
            }
        }
    }

    private IEnumerator EnableBubble(GameObject bubble)
    {
        yield return new WaitForSeconds(0.3f);
        bubble.SetActive(true);
    }

    private IEnumerator DelayHideShelf()
    {
        yield return new WaitForSeconds(0.3f);

        mGadgetShelf.SetActive(false);

        yield return new WaitForSeconds(0.3f);

        GameManager.Instance.RightInteractNearTouch.ForceStopNearTouching();
    }

    private IEnumerator ShiftContainer(int containerIndex, Vector3 containerPosition)
    {
        float startTime = Time.time;
        float fraction = 0;
        float random_x = UnityEngine.Random.Range(0, GadgetOffsetMax);
        float random_y = UnityEngine.Random.Range(0, GadgetOffsetMax);
        Vector3 startingPosition = new Vector3(random_x + shelfRadius, random_y, 0);
        float rate = UnityEngine.Random.Range(ShiftRateMin, ShiftRateMax);

        Transform childContainer = mGadgetShelf.transform.GetChild(containerIndex);
        if (childContainer.GetComponentInChildren<Gadget>() != null)
            childContainer.GetComponentInChildren<Gadget>().ShowShelf(true);

        while (fraction * rate <= 1)
        {
            fraction = Time.time - startTime;

            childContainer.localPosition = Vector3.Lerp(startingPosition, containerPosition, fraction * rate);
            childContainer.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, fraction * rate);

            yield return null;
        }
    }

    private void SpawnGadgetShelf()
    {
        float startDegree_xz = -60.0f;
        float gap = 30f;
        int counter = 0;
        float y_pos = 0.0f;

        shelfGadgetContainersPositions = new Vector3[(int)GadgetInventory.NUM];

        for (int i = 0; i < (int)GadgetInventory.NUM; i++)
        {
            // Create container and store their position
            GameObject container = new GameObject("Container " + i.ToString());
            container.transform.SetParent(mGadgetShelf.transform);

            float container_pos_x = shelfRadius * Mathf.Cos(startDegree_xz * (float)Math.PI / 180);
            float container_pos_z = shelfRadius * Mathf.Sin(startDegree_xz * (float)Math.PI / 180);

            Vector3 container_pos = new Vector3(container_pos_x, y_pos, container_pos_z);
            container.transform.localPosition = container_pos;
            shelfGadgetContainersPositions[i] = container_pos;

            // Create bubble
            GameObject bubbleObj = Instantiate(BubblePrefab, container.transform);

            // Create gadget
            string gadgetName = ((GadgetInventory)i).ToString();

            SpawnSingleGadget(gadgetName, container.transform);

            startDegree_xz += gap;
            counter += 1;

            if (counter == 5)
            {
                counter = 0;
                startDegree_xz = -60.0f;
                if (y_pos >= 0)
                {
                    y_pos += 0.3f;
                    y_pos = -y_pos;
                }
                else
                {
                    y_pos = -y_pos;
                }
            }
        }

        // spawn small bubbles for saved files
        shelfFileContainersPositions = new Vector3[NUM_SAVE_SLOTS];

        startDegree_xz = 30.0f;
        if (y_pos >= 0)
        {
            y_pos += 0.3f;
            y_pos = -y_pos;
        }
        else
        {
            y_pos = -y_pos;
        }

        for (int i = 0; i < NUM_SAVE_SLOTS; i++)
        {
            GameObject container = new GameObject("File Container " + i.ToString());
            container.transform.SetParent(mGadgetShelf.transform);

            float container_pos_x = shelfRadius * Mathf.Cos(startDegree_xz * (float)Math.PI / 180);
            float container_pos_z = Mathf.Sin(startDegree_xz * (float)Math.PI / 180);
            Vector3 container_pos = new Vector3(container_pos_x, y_pos, container_pos_z);
            container.transform.localPosition = container_pos;
            shelfFileContainersPositions[i] = container_pos;

            // load file bubble
            GameObject bubbleObj = Instantiate(BubblePrefab, container.transform);
            bubbleObj.transform.localScale *= 0.5f;

            // Create gadget
            string gadgetName = "LoadCube";
            LoadCube spawnedLoadCube = SpawnSingleGadget(gadgetName, container.transform) as LoadCube;
            Renderer r = spawnedLoadCube.GetComponent<Renderer>();
            r.material.color = RoomColors[i];
            spawnedLoadCube.Slot = i.ToString();

            startDegree_xz -= 20.0f;
        }
    }

    private void RespawnGadgets()
    {
        int StartFromHereForFile = 0;
        for (int i = 0; i < (int)GadgetInventory.NUM; i++)
        {
            Transform placeHolder = mGadgetShelf.transform.GetChild(i);
            if (placeHolder.childCount < 2) // This is set to 2 because the placeholder has a bubble now
            {
                string gadgetName = ((GadgetInventory)i).ToString();
                SpawnSingleGadget(gadgetName, placeHolder);
                return;
            }
            StartFromHereForFile += 1;
        }
    }

    public void RespawnFiles()
    {
        int countOFchildern = mGadgetShelf.transform.childCount;
        for (int i = (int)GadgetInventory.NUM; i < countOFchildern; i++)
        {
            Transform placeHolder = mGadgetShelf.transform.GetChild(i);
            if (placeHolder.childCount < 2)
            {
                int slot = i - (int)GadgetInventory.NUM;
                string gadgetName = "LoadCube";
                LoadCube spawnedLoadCube = SpawnSingleGadget(gadgetName, placeHolder) as LoadCube;
                Renderer r = spawnedLoadCube.GetComponent<Renderer>();
                r.material.color = RoomColors[slot];
                spawnedLoadCube.Slot = slot.ToString();
            }
        }
    }

    private Gadget SpawnSingleGadget(string gadgetName, Transform parentTransform)
    {
        GameObject gadgetResource = Resources.Load(gadgetName) as GameObject;
        GameObject gadgetObj = Instantiate(gadgetResource, parentTransform);
        gadgetObj.transform.localPosition = Vector3.zero;
        gadgetObj.name = gadgetName + " (OnShelf)";
        Gadget gadget = gadgetObj.GetComponent<Gadget>();

        return gadget;
    }

    public void InsertGadget(Gadget gadget)
    {
        gadget.gameObject.name = gadget.GetType().ToString() + gadgetsInWorld.Count.ToString();
        gadgetsInWorld.Add(gadget);
        MarkWorldModified();
    }

    public void RemoveGadget(Gadget gadget)
    {
        gadgetsInWorld.Remove(gadget);
        gadget.RemoveFromScene();
        MarkWorldModified();
    }

    public void MarkWorldModified()
    {
        isWorldStateModified = true;
    }


}