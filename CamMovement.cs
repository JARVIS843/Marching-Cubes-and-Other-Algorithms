using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMovement : MonoBehaviour
{
    public Camera cam;
    // Update is called once per frame
    void Update()
    {
          transform.position = Vector3.MoveTowards(transform.position, transform.position + (cam.transform.forward * Input.GetAxis("Vertical")) + (transform.right * Input.GetAxis("Horizontal")), Time.deltaTime * 10f);
        cam.transform.Rotate(new Vector3(-Input.GetAxis("Mouse Y"), 0, 0 ));
        cam.transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X"), 0));

        if (Input.GetMouseButtonDown(0)) {

            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1f));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
                if (hit.transform.tag == "Terrain")
                    hit.transform.GetComponent<Chunk>().PlaceTerrain(hit.point);
            }

        }

       if (Input.GetMouseButtonDown(1)) {
           Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1f));
           RaycastHit hit;
           if (Physics.Raycast(ray, out hit)) {
               if (hit.transform.tag == "Terrain")
                   hit.transform.GetComponent<Chunk>().RemoveTerrain(hit.point);
               
           }
       }
    }

   
}
