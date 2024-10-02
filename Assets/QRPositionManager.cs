using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class QRPositionManager : MonoBehaviour
{
    public GameObject qrTracker;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BakePosition() {
        this.transform.position = qrTracker.transform.position;
        this.transform.rotation = qrTracker.transform.rotation;
    }
}
