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

    private bool spawning;

    private void Start()
    {
        timer = spawnTime + 1f;
        spawning = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (spawning)
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
    }

    private void SpawnPipe(GameObject pipePrefab, float margin, float height)
    {
        Pipe newPipe = Instantiate(pipePrefab, transform, true).GetComponent<Pipe>();
        newPipe.SetMargin(margin);
        newPipe.transform.position = transform.position + new Vector3(0, Random.Range(-height, height) + .5f, 0);
        newPipe.IncreaseSpeed(speedIncrease);
        Destroy(newPipe.gameObject, 6);
    }

    // Destroy all active pipes
    public void Clear()
    {
        spawning = false;
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void SetActive()
    {
        spawnTime = 1.75f;
        timer = spawnTime;
        spawnDecrease = .05f;
        speedIncrease = .1f;
        spawning = true;
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
