using UnityEngine;
using System.Collections;

public class IntersectionManager : MonoBehaviour
{
    [Header("차량 제어 신호등")]
    public CarControlTrafficLight[] vehicleControlLights;  // 🔴 CarControlTrafficLight
    public CarControlTrafficLight[] pedestrianControlLights;

    [Header("비주얼 신호등 (옵션)")]
    public TrafficLight[] vehicleVisualLights;  // 🔴 TrafficLight (비주얼만)
    public TrafficLight[] pedestrianVisualLights;

    [Header("TTS 시스템")]  // 🔴 추가
    public TrafficLightTTS[] pedestrianTTS;

    [Header("타이밍 설정")]
    public float vehicleGreenTime = 15f;
    public float pedestrianGreenTime = 10f;
    public float yellowTime = 3f;

    void Start()
    {
        if (vehicleControlLights == null || vehicleControlLights.Length == 0)
        {
            Debug.LogError("[IntersectionManager] Vehicle Control Lights가 비어있습니다!");
            return;
        }

        // 비주얼 신호등의 자체 코루틴 중지
        if (vehicleVisualLights != null)
        {
            foreach (var light in vehicleVisualLights)
            {
                if (light != null) light.StopAllCoroutines();
            }
        }

        if (pedestrianVisualLights != null)
        {
            foreach (var light in pedestrianVisualLights)
            {
                if (light != null) light.StopAllCoroutines();
            }
        }

        Debug.Log("[IntersectionManager] 시작!");
        StartCoroutine(ManageIntersection());
    }

    IEnumerator ManageIntersection()
    {
        while (true)
        {
            // 1️⃣ 차량 초록, 보행자 빨강
            Debug.Log("=== 차량 초록 / 보행자 빨강 ===");
            SetControlLights(vehicleControlLights, LightState.Green);
            SetControlLights(pedestrianControlLights, LightState.Red);
            SetVisualLights(vehicleVisualLights, LightState.Green);
            SetVisualLights(pedestrianVisualLights, LightState.Red);
            NotifyTTS(LightState.Red);
            yield return new WaitForSeconds(vehicleGreenTime);

            // 2️⃣ 차량 노랑, 보행자 빨강
            Debug.Log("=== 차량 노랑 / 보행자 빨강 ===");
            SetControlLights(vehicleControlLights, LightState.Yellow);
            SetControlLights(pedestrianControlLights, LightState.Red);
            SetVisualLights(vehicleVisualLights, LightState.Yellow);
            SetVisualLights(pedestrianVisualLights, LightState.Red);
            yield return new WaitForSeconds(yellowTime);

            // 3️⃣ 차량 빨강, 보행자 초록
            Debug.Log("=== 차량 빨강 / 보행자 초록 ===");
            SetControlLights(vehicleControlLights, LightState.Red);
            SetControlLights(pedestrianControlLights, LightState.Green);
            SetVisualLights(vehicleVisualLights, LightState.Red);
            SetVisualLights(pedestrianVisualLights, LightState.Green);
            NotifyTTS(LightState.Green);
            yield return new WaitForSeconds(pedestrianGreenTime);

            // 4️⃣ 차량 빨강, 보행자 노랑
            Debug.Log("=== 차량 빨강 / 보행자 노랑 ===");
            SetControlLights(vehicleControlLights, LightState.Red);
            SetControlLights(pedestrianControlLights, LightState.Yellow);
            SetVisualLights(vehicleVisualLights, LightState.Red);
            SetVisualLights(pedestrianVisualLights, LightState.Yellow);
            yield return new WaitForSeconds(yellowTime);
        }
    }

    // 🔴 CarControlTrafficLight 직접 제어
    void SetControlLights(CarControlTrafficLight[] lights, LightState state)
    {
        if (lights == null) return;

        foreach (var light in lights)
        {
            if (light != null)
            {
                // OnLightStateChanged 직접 호출
                bool red = (state == LightState.Red);
                bool yellow = (state == LightState.Yellow);
                bool green = (state == LightState.Green);

                light.OnLightStateChanged(red, yellow, green);
                Debug.Log($"🎯 [IntersectionManager] {light.name} CarControl → {state}");
            }
        }
    }

    // 🔴 TrafficLight 비주얼 제어
    void SetVisualLights(TrafficLight[] lights, LightState state)
    {
        if (lights == null) return;

        foreach (var light in lights)
        {
            if (light != null)
            {
                light.SetLightStateExternal(state);
            }
        }
    }

    void NotifyTTS(LightState state)
    {
        if (pedestrianTTS == null) return;

        foreach (var tts in pedestrianTTS)
        {
            if (tts != null)
            {
                tts.OnLightStateChanged(state);
            }
        }
    }
}