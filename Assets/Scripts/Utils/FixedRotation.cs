using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedRotation : MonoBehaviour
{

    void Update()
    {
        transform.rotation = Quaternion.identity;
    }
}
