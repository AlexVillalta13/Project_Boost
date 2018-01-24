using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour
{
    [SerializeField] Vector3 movement;
    [SerializeField] float period = 2f;

    [Range(0, 1)]
    [SerializeField] float movementfactor;

    Vector3 startingPos;

    private void Start()
    {
        startingPos = transform.position;
    }

    private void Update()
    {
        if (period <= Mathf.Epsilon) { return;  }

        float cycles = Time.time / period;

        const float tau = Mathf.PI * 2;
        float rawSinWave = Mathf.Sin(cycles * tau);

        movementfactor = rawSinWave / 2f + 0.5f;

        Vector3 offset = movement * movementfactor;
        transform.position = startingPos + offset;
    }

}
