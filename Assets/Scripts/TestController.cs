using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestController : MonoBehaviour
{
    public GameObject highlihgt;
    private Camera _camera;
    private Ray _ray;
    private RaycastHit _hit;

    void Start()
    {
        _camera = Camera.main;
        _hit = new RaycastHit();
    }

    void FixedUpdate()
    {
        _ray = _camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(_ray, out _hit))
        {
            highlihgt.SetActive(true);
            highlihgt.transform.position = new Vector3(Mathf.RoundToInt(_hit.point.x), 0.275f, Mathf.RoundToInt(_hit.point.z));
        }
        else
        {
            highlihgt.SetActive(false);
        }
    }
}
