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
    string jsonFile = "";

    [SerializeField]
    List<GameObject> cars_prefabs = new List<GameObject>();
    [SerializeField]
    List<Renderer> semaforos = new List<Renderer>();


    string json;
    int time_index = 0;
    
     //Control variables
    private Coroutine coroutine_ofchange = null;
    JSONNode simulation;
    [SerializeField]
    float actu_time;
    [SerializeField]
    float car_vel;

    // Update is called once per frame
    void Start()
    {
        json = File.ReadAllText(Application.dataPath + "/json/"+jsonFile+".json");
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
                createCar(id,frame["cars"][i]["x"], frame["cars"][i]["y"] ,  frame["cars"][i]["speedX"], frame["cars"][i]["speedY"] );
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

        for(int i = 0; i < semaforos.Count; i++)
        {
            semaforos[i].material.SetColor("_BaseColor", getColorString(frame["lights"]["state"][i]) );
        }

        yield return new WaitForSeconds(actu_time);
        coroutine_ofchange = null;
    }



    Color getColorString(string input)
    {   
        if(input == "R")
        {
            return Color.red;
        }else if( input == "G")
        {
            return Color.green;
        }else if(input == "Y")
        {
            return Color.yellow;
        }else
        {
            return Color.grey;
        }
    }

    void createCar(int index, int x, int y, int dx, int dy)
    {
        Vector3 nextPos = getPosByIndex(x+dx, dy+y);
        Vector3 pos = getPosByIndex(x,y);
        var new_car = Instantiate( randomCar() , pos ,Quaternion.LookRotation(nextPos-pos) );
        car_agents[index] = new_car.GetComponent<FollowPath>();
        new_car.GetComponent<FollowPath>().path = getPosByIndex(x,y);
        new_car.GetComponent<FollowPath>().linear_vel = car_vel;
        
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
