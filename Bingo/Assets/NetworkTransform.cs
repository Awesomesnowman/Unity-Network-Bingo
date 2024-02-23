using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
public class NetworkTransform : NetworkComponent
{
    Vector3 lastPosition;
    Vector3 lastRotation;
    public float threshold = .1f;

    public override void HandleMessage(string flag, string value)
    {
        if(IsClient)
        {
            if(flag == "POS")
            {
                lastPosition = NetworkCore.Vector3FromString(value);
            }
            if (flag == "ROT")
            {
                lastRotation = NetworkCore.Vector3FromString(value);
            }
        }
    }

    public override void NetworkedStart()
    {

    }

    public override IEnumerator SlowUpdate()
    {
        while(IsServer)
        {

            if((transform.position - lastPosition).magnitude > threshold)
            {
                SendUpdate("POS", this.transform.position.ToString());
                lastPosition = transform.position;
            }
            if((transform.rotation.eulerAngles - lastRotation).magnitude > threshold)
            {
                SendUpdate("ROT", this.transform.rotation.eulerAngles.ToString());
                lastRotation = this.transform.rotation.eulerAngles;
            }
            if (IsDirty)
            {
                SendUpdate("POS", this.transform.position.ToString());
                SendUpdate("ROT", this.transform.rotation.eulerAngles.ToString());
                IsDirty = false;
            }
            yield return new WaitForSeconds(MyId.UpdateFrequency);
        }
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float tempSpeed = (this.lastPosition - transform.position).magnitude * MyId.UpdateFrequency;
        this.transform.position = Vector3.MoveTowards(this.transform.position, this.lastPosition, tempSpeed*Time.deltaTime);
        float tempRotation = (this.lastRotation - transform.rotation.eulerAngles).magnitude * MyId.UpdateFrequency;
        //this.transform.rotation. = Vector3.MoveTowards(transform.rotation.eulerAngles, this.lastRotation, tempSpeed * Time.deltaTime);
    }
}
