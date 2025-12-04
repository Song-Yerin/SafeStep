using System.Collections.Generic;
using UnityEngine;
using System.Collections;  // ← 코루틴용

public class TrafficLight : MonoBehaviour
{
    public float greenTime = 10f;
    public float redTime = 10f;
    public Material greenMaterial, redMaterial;

    private bool isGreen = true;
    private List<CarAI> carsInRange = new List<CarAI>();

    void Start()
    {
        StartCoroutine(TrafficLightCycle());
    }

    IEnumerator TrafficLightCycle()
    {
        while (true)
        {
            isGreen = true;
            GetComponent<Renderer>().material = greenMaterial;
            foreach (var car in carsInRange) car.SetCanMove(true);
            yield return new WaitForSeconds(greenTime);

            isGreen = false;
            GetComponent<Renderer>().material = redMaterial;
            foreach (var car in carsInRange) car.SetCanMove(false);
            yield return new WaitForSeconds(redTime);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        CarAI car = other.GetComponent<CarAI>();
        if (car != null)
        {
            carsInRange.Add(car);
            if (!isGreen) car.SetCanMove(false);
        }
    }

    void OnTriggerExit(Collider other)
    {
        CarAI car = other.GetComponent<CarAI>();
        if (car != null) carsInRange.Remove(car);
    }
}