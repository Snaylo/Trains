using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class TrainManager : MonoBehaviour
{
    public GameObject trainPrefab, carriagePrefab;
    public float[] trackPositionsX = new float[8];
    [SerializeField] private float startZLeft = -60.0f, startZRight = 320.0f;
    public bool[] tracksOccupied = new bool[8];
    private static TrainManager _instance;
    [SerializeField] private float minSpeed = 1.0f, maxSpeed = 10.0f;
    [SerializeField] private int minCarriages = 3, maxCarriages = 10;
    [SerializeField] private float frontAttachOffset = 0.2245f, backAttachOffset = -0.2245f;
    [SerializeField] private float minWaitingTime = 0.5f, maxWaitingTime = 1.0f;
    private List<Camera> cameras = new List<Camera>();
    private int camera = 0;
    public static TrainManager Instance {
        get {
            if (_instance != null) return _instance;
            var trainManager = new GameObject();
            _instance = trainManager.AddComponent<TrainManager>();
            trainManager.name = typeof(TrainManager).ToString();
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null) _instance = this;
    }

    private void Start()
    {
        
        StartCoroutine(SpawnTrains());
        cameras.Add(Camera.main);
        foreach (var drunk in GameObject.FindObjectsOfType<DrunkAgent>())
        {
            cameras.Add(drunk.gameObject.GetComponentInChildren<Camera>());
            drunk.gameObject.GetComponentInChildren<Camera>().enabled = false;
            drunk.gameObject.GetComponentInChildren<AudioListener>().enabled = false;

        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            cameras[camera].gameObject.tag = "Untagged";
            cameras[camera].enabled = false;
            cameras[camera].gameObject.transform.GetComponent<AudioListener>().enabled = false;
            if (camera >= cameras.Count-1) camera = 0;
            else camera++;
            cameras[camera].enabled = true;
            cameras[camera].gameObject.transform.GetComponent<AudioListener>().enabled = true;
            cameras[camera].gameObject.tag = "MainCamera";
        }  if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            cameras[camera].gameObject.tag = "Untagged";
            cameras[camera].enabled = false;
            cameras[camera].gameObject.transform.GetComponent<AudioListener>().enabled = false;
            if (camera <= 0) camera = cameras.Count-1;
            else camera--;
            cameras[camera].enabled = true;
            cameras[camera].gameObject.transform.GetComponent<AudioListener>().enabled = true;
            cameras[camera].gameObject.tag = "MainCamera";
        } 
    }

    IEnumerator SpawnTrains()
    {
        while (true)
        {
            var track = -1;
            for (int i = 0; i < tracksOccupied.Length; i++)
            {
                if (tracksOccupied[i] == false)
                {
                    track = i;
                    tracksOccupied[i] = true;
                    break;
                }
            }

            if (track >= 0)
            {
                var z = 0.0f;
                var minimumSpeed = minSpeed;
                var maximumSpeed = maxSpeed;
                bool right  = (Random.value > 0.5f);
                //var right = Convert.ToBoolean(Random.Range(0, 1));
                var offset = 0.0f;
                if (right)
                {
                    z = startZLeft; //Driving right speed should be positive and use back attach
                    offset = frontAttachOffset;
                }
                else
                {
                    z = startZRight; //Driving left, speed should be negative and use front attach
                    offset = backAttachOffset;
                    minimumSpeed *= -1;
                    maximumSpeed *= -1;
                }
                var startPos = new Vector3(trackPositionsX[track], 0.0f, z);
                var train = Instantiate(trainPrefab, startPos, quaternion.identity);
                var trainScript = train.GetComponent<Train>();
                var speedOfTrain = (track + 2) / 2;
                var finalSpeed = Random.Range(speedOfTrain * minimumSpeed, speedOfTrain * maximumSpeed);
                trainScript.SetSpeed(finalSpeed);
                train.GetComponent<AudioSource>().pitch = Map(finalSpeed, speedOfTrain * minimumSpeed,
                    speedOfTrain * maximumSpeed, 0.5f, 2.0f);
                var trainLength = Random.Range(minCarriages, maxCarriages);
                trainScript.numberOfCarriages = trainLength;
                trainScript.driveRight = right;
                trainScript.track = track;

                for (int i = 0; i < trainLength; i++)
                {
                    var rot = Quaternion.Euler(-90, 0, -90);
                    var carriage = Instantiate(carriagePrefab, train.transform.position, rot);
                    carriage.transform.SetParent(train.transform);
                    var pos = new Vector3(carriage.transform.position.x, carriage.transform.position.y,train.transform.position.z+ i * offset);
                    carriage.transform.position = pos;
                    //if (i == 0) carriage.AddComponent<Rigidbody>().useGravity = false;
                }
            }

            yield return new WaitForSeconds(Random.Range(minWaitingTime, maxWaitingTime));
        }
    }

    IEnumerator LateStart()
    {
        yield return new WaitForSeconds(1.0f);
       
        
    }
    public float Map(float value, float min1, float max1, float min2, float max2) {
        return min2 + (max2 - min2) * ((value - min1) / (max1 - min1));
    }


}
