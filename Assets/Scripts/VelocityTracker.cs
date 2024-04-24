using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

[Serializable]
public class VelocityEvent : UnityEvent
{
}

public class VelocityTracker : MonoBehaviour
{
    public int bufferSize = 10;
    public Vector3[] velbuffers;
    public Vector3[] posBuffer;
    private int rollingIndex = 0;
    private Vector3 lastPos;
    public Vector3 avgVel;
    private LineRenderer lineRenderer;
    public float scale = 20;
    public Material lineMaterial = new Material(Shader.Find("Sprites/Default"));
    public float thresh = 0.002f;
    public float mag = 0.0f;
    public float last = 0.0f;
    public float avgSpeed = 0.0f;


    public VelocityEvent onStop = new VelocityEvent();
    
    // Start is called before the first frame update
    void Start()
    {
        velbuffers = new Vector3[bufferSize];
        posBuffer = new Vector3[bufferSize];
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = lineMaterial;
        lineRenderer.widthMultiplier = 0.005f;
        
        
    }

    public void PositionUpdated()
    {
        if (lastPos != null)
        {
            Vector3 vel = transform.position - lastPos;
            velbuffers[rollingIndex] = vel;
            rollingIndex++;
            rollingIndex %= bufferSize;
            last = avgSpeed;
            avgVel = new Vector3(0f, 0f, 0f);
            avgSpeed = 0.0f;
            int count = 0;
            foreach (Vector3 v in velbuffers)
            {
                if (v == null) continue;
                avgVel += v;
                avgSpeed += v.magnitude;
                count++;
            }
            avgVel /= count;
            avgSpeed /= count;
            Vector3 scaledVel = avgVel * scale;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position + scaledVel);
            mag = avgVel.magnitude;
            if (last >= thresh && avgSpeed < thresh)
            {
                onStop.Invoke();
                Debug.Log("stopped");
            }
        }
        lastPos = transform.position;

    }

    // Update is called once per frame
    void Update()
    {
           
    }
}
