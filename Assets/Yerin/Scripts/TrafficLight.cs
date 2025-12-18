using UnityEngine;
using System.Collections;

public class TrafficLight : MonoBehaviour
{
    [Header("신호등 설정")]
    public LightType lightType;
    public float greenDuration = 10f;
    public float yellowDuration = 3f;
    public float redDuration = 10f;

    [Header("신호등 오브젝트 (옵션)")]
    public GameObject greenLightObj;
    public GameObject yellowLightObj;
    public GameObject redLightObj;

    [Header("신호등 Light 컴포넌트")]  // 🔴 추가
    public Light greenLight;
    public Light yellowLight;
    public Light redLight;

    public LightState currentState { get; private set; }

    [Header("외부 제어")]
    public bool useExternalControl = false;

    void Start()
    {
        if (!useExternalControl)
        {
            StartCoroutine(TrafficLightCycle());
        }
    }

    IEnumerator TrafficLightCycle()
    {
        while (true)
        {
            // 초록불
            SetLightState(LightState.Green);
            yield return new WaitForSeconds(greenDuration);

            // 노란불
            SetLightState(LightState.Yellow);
            yield return new WaitForSeconds(yellowDuration);

            // 빨간불
            SetLightState(LightState.Red);
            yield return new WaitForSeconds(redDuration);
        }
    }

    public void SetLightStateExternal(LightState state)
    {
        SetLightState(state);
    }

    void SetLightState(LightState state)
    {
        currentState = state;

        // 🔴 GameObject 활성화/비활성화
        if (greenLightObj != null)
            greenLightObj.SetActive(state == LightState.Green);
        if (yellowLightObj != null)
            yellowLightObj.SetActive(state == LightState.Yellow);
        if (redLightObj != null)
            redLightObj.SetActive(state == LightState.Red);

        // 🔴 Light 컴포넌트 켜기/끄기 (추가!)
        if (greenLight != null)
            greenLight.enabled = (state == LightState.Green);
        if (yellowLight != null)
            yellowLight.enabled = (state == LightState.Yellow);
        if (redLight != null)
            redLight.enabled = (state == LightState.Red);

        Debug.Log($"{lightType} 신호등: {state}");
    }
}

public enum LightType
{
    Vehicle,    // 차량용
    Pedestrian  // 보행자용
}
