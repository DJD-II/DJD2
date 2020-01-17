using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePositions : MonoBehaviour
{
    [SerializeField] public List<Vector3> positions = new List<Vector3>(100);
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject startHolo;

    private void Start()
    {
        player = GameObject.Find("Player");
    }
    private IEnumerator SavePos()
    {
        WaitForSecondsRealtime waitTimer = new WaitForSecondsRealtime(0.5f);

        while (true)
        {
            positions.Add(player.transform.position);

            yield return waitTimer;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
            StartCoroutine(SavePos());
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Player")
        {
            StopAllCoroutines();
            startHolo.SetActive(true);
            gameObject.SetActive(false);
        }
    }
    public List<Vector3> GetPositions()
    {
        return positions;
    }
}
