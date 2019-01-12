using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loading : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    public float rotationSpeed = 10f;

    void Update()
    {
        transform.Rotate(Vector3.up * Time.deltaTime * rotationSpeed);
    }
}
