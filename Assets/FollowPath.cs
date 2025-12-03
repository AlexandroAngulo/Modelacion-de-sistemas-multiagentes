using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class FollowPath : MonoBehaviour
{

    //Input Parameters
    [SerializeField]
    public Vector3 path;
    [SerializeField]
    public float linear_vel;
    
    [SerializeField]
    float rotational_vel;

    
    void Start(){
        
        path = new Vector3(path.x, transform.position.y, path.z);
    }

    // Update is called once per frame
    void Update()
    {
        if((transform.position- path).magnitude > 1)
        {
            moveTowardsObjective();
        }
        

    }
    void moveTowardsObjective(){
        Quaternion relative_rotation = Quaternion.LookRotation(path - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, relative_rotation, rotational_vel* Time.deltaTime);
        transform.position += transform.forward * Time.deltaTime * linear_vel ;
    }
    /*
    public IEnumerator Change_Objective(){
        //Wait for a simple wait
        
        yield return new WaitForSeconds(wait_seconds);
        
        //Check all different posibilities to continue
        List<KeyValuePair<float,GameObject>> adyacencies = new List<KeyValuePair<float, GameObject>>() ;
        //Create the sum of all posibilities
        float Big_posibility = 0;
        foreach (Transform child in path){
            //if no posibility by default set it to 1
            float posibility = child.gameObject.GetComponent<Data>()?.probability() ?? 1;
            Debug.Log(posibility);
            Big_posibility += posibility;
            adyacencies.Add(new KeyValuePair<float, GameObject>(posibility, child.gameObject));
        }
        float choice = Random.Range(0,1f) * Big_posibility;
        float currentProb = 0;
        int selected_index_path = 0;
        //Iterate over the algorithm to determine the first one that passes the threshold of choice
        for(; selected_index_path < adyacencies.Count; selected_index_path++){
            currentProb += adyacencies[selected_index_path].Key;
            if(currentProb > choice)
            {
                break;
            }
        }
        //Set the selected object as objective
        path = adyacencies[selected_index_path].Value;
        Debug.Log(path.name);
        coroutine_ofchange = null;
    }*/
}
