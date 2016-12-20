using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ObjectGroupId : NetworkBehaviour {
    [SyncVar]
    public int id = -1;

}
