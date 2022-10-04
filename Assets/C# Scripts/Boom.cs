using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Boom : MonoBehaviour
{

    void Start()
    {
        transform.localScale = new Vector3(1, 1, 1);
    }

    void Update()
    {
        if (transform.localScale.y <= 10f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(11f, 11f, 11f), (Time.deltaTime * 10));
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
