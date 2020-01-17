using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestHolo : MonoBehaviour
{
    [SerializeField] private GameObject holoObject;
    [SerializeField] private GameObject holoTrigger;

    private SavePositions save;
    [SerializeField] private int i = 0;
    [SerializeField] private int timer = 0;
    private List<Vector3> positions = new List<Vector3>();
    private CharacterController hologram;
    private Vector3 moveVector;
    private Vector3 oldPos;
    private bool updatePos = true;

    private void Awake()
    {
        hologram = holoObject.GetComponent<CharacterController>();
        save = holoTrigger.GetComponent<SavePositions>();
        positions = save.GetPositions();
        holoObject.transform.position = save.positions[i];
    }

    private void Update()
    {
        if (updatePos)
        {
            holoObject.transform.LookAt(save.positions[i]);
            transform.position = save.positions[i];

            moveVector = holoObject.transform.forward * 1f;
            hologram.Move(moveVector * Time.deltaTime);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "hologram" && i < positions.Count - 1)
        {
            i++;
        }
        else if (positions.Count - 1 >= i)
        {
            updatePos = false;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.name == "hologram")
        {
            timer++;
        }
        if (timer > 100f)
            i++;
    }
}
