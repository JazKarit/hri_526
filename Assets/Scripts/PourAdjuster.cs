using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformTools
{
    public static Vector3 InverseTransformPointUnscaled(this Transform transform, Vector3 position)
    {
        var worldToLocalMatrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one).inverse;
        return worldToLocalMatrix.MultiplyPoint3x4(position);
    }
}

public class PourAdjuster : MonoBehaviour
{
    public GameObject table;
    public GameObject parent;
    public GameObject gripper;
    public Vector3 test;
    private bool set = false;

    


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.identity;
        transform.position = new Vector3(transform.position.x, transform.parent.position.y - ((Mathf.Abs(transform.parent.position.y - table.transform.position.y) /2)), transform.position.z);
        transform.localScale = new Vector3(0.1f, (Mathf.Abs(transform.parent.position.y - table.transform.position.y)/2)*25, 0.1f);
        
        if (set)
        {
            SendUpdatedTransform();
        }

        

        //Vector3 offset = TransformTools.InverseTransformPointUnscaled(gripper.transform, transform.position);

        //Debug.Log(gripper.transform.InverseTransformPoint(transform.position)/25);
        /*
        lineRenderer.SetPositions(new Vector3[2] {
             new Vector3(transform.position.x, transform.position.y, transform.position.z),
            new Vector3(transform.position.x, table.transform.position.y, transform.position.z)
         });
        boxCollider.center = new Vector3(0, -((transform.position.y + table.transform.position.y)/2), 0) *25f;
        boxCollider.size = new Vector3(0.05f, Mathf.Abs(transform.position.y - table.transform.position.y), 0.05f)*25f;

        test = transform.parent.InverseTransformPoint(transform.position)/25f;
        */
    }

    public void SendUpdatedTransform()
    {
        Vector3 offset = TransformTools.InverseTransformPointUnscaled(gripper.transform, transform.position);
        RobotActions.instance.PourAdjust(offset);
        parent.SetActive(false);
        this.set = true;
    }

    
}
