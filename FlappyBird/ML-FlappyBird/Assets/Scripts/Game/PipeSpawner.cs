using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeSpawner : MonoBehaviour
{

    [SerializeField] private float spawnTime = 1f;
    private float timer = 0f;
    [SerializeField] private GameObject pipePrefab;
    [SerializeField] private float height = 10f;

    [SerializeField] private float spawnDecrease = .05f;
    [SerializeField] private float speedIncrease = .1f;

    [SerializeField] private float margin = 4.25f;

    private List<Pipe> activePipes;

    private void Start()
    {
        activePipes = new List<Pipe>();
        timer = spawnTime + 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > spawnTime)
        {
            SpawnPipe(pipePrefab, margin, height);

            speedIncrease += .1f;
            spawnTime -= spawnDecrease;
            spawnDecrease *= .95f;

            timer = 0;
        }
        timer += Time.deltaTime;
    }

    private void SpawnPipe(GameObject pipePrefab, float margin, float height)
    {
        Pipe newPipe = Instantiate(pipePrefab, transform, true).GetComponent<Pipe>();
        newPipe.SetMargin(margin);
        newPipe.transform.position = transform.position + new Vector3(0, Random.Range(-height, height) + .5f, 0);
        newPipe.IncreaseSpeed(speedIncrease);
        Destroy(newPipe.gameObject, 6);
    }

    public (float, float) GetPipeInput()
    {
        foreach(Pipe pipe in transform.GetComponentsInChildren<Pipe>())
        {
            if (pipe.transform.position.x < -0.8f) continue;
            return (pipe.transform.position.x - 0.8f, pipe.transform.position.y);
        }
        return (0, 0);
    }
}
