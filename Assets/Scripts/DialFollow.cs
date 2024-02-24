using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialFollow : MonoBehaviour
{
    public GameObject parent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(parent.transform.position.x, parent.transform.position.y + 0.1f, parent.transform.position.z);
        


        //transform.eulerAngles = new Vector3(90, transform.eulerAngles.y, parent.transform.eulerAngles.z);
        
    }
}
