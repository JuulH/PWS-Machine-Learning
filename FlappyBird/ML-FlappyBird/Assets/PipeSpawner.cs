using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeSpawner : MonoBehaviour
{

    [SerializeField] private float spawnTime = 1f;
    private float timer = 0f;
    [SerializeField] private GameObject pipe;
    [SerializeField] private float height = 10f;

    [SerializeField] private float spawnDecrease = .05f;
    [SerializeField] private float speedIncrease = .1f;

    // Update is called once per frame
    void Update()
    {
        if (timer > spawnTime)
        {
            GameObject newPipe = Instantiate(pipe);
            newPipe.transform.position = transform.position + new Vector3(0, Random.Range(-height, height) + .5f, 0);

            newPipe.SendMessage("increaseSpeed", speedIncrease);
            speedIncrease += .1f;
            spawnTime -= spawnDecrease;
            spawnDecrease *= .95f;

            Destroy(newPipe, 6);

            timer = 0;
        }
        timer += Time.deltaTime;
    }
}
