using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SimpleJSON;

public class SimulationManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    Dictionary<int, FollowPath> car_agents = new Dictionary<int, FollowPath>();

    [SerializeField]
    List<GameObject> cars_prefabs = new List<GameObject>();
    string json;
    int time_index = 0;
    
     //Control variables
    private Coroutine coroutine_ofchange = null;
    JSONNode simulation;

    // Update is called once per frame
    void Start()
    {
        json = File.ReadAllText(Application.dataPath + "/json/data.json");
        simulation = JSON.Parse(json);

    }
    void Update()
    {
        if(coroutine_ofchange == null)
        {
            coroutine_ofchange = StartCoroutine(changeCoord());
        }
    }
    private IEnumerator changeCoord()
    {
        time_index ++;
        HashSet<int> existing_indexes = new HashSet<int>();
        var frame =  simulation["frames"].AsArray[time_index];
        for(int i = 0; i < frame["cars"].AsArray.Count; i++)
        {
            int id = frame["cars"][i]["id"];
            existing_indexes.Add(id);
            if (car_agents.ContainsKey(id))
            {
                car_agents[id].path = getPosByIndex(frame["cars"][i]["x"], frame["cars"][i]["y"]);
            }
            else
            {
                createCar(id,frame["cars"][i]["x"], frame["cars"][i]["y"] );
            }
        }
        var newDict = new Dictionary<int, FollowPath>();
        foreach(var item in car_agents)
        {
            if (!existing_indexes.Contains(item.Key))
            {
                Destroy(item.Value.gameObject);
            }
            else
            {
                newDict.Add(item.Key, item.Value);
            }
        }
        car_agents = newDict;
        yield return new WaitForSeconds(1f);
        coroutine_ofchange = null;
    }

    void createCar(int index, int x, int y)
    {
        var new_car = Instantiate( randomCar() , getPosByIndex(x,y), Quaternion.identity  );
        car_agents[index] = new_car.GetComponent<FollowPath>();
        new_car.GetComponent<FollowPath>().path = getPosByIndex(x,y);
    }

    GameObject randomCar()
    {
        return cars_prefabs [ UnityEngine.Random.Range(0,cars_prefabs.Count)];
    }
    Vector3 getPosByIndex(int x, int y)
    {
        return transform.GetChild(y).GetChild(x).position;
    }
}
