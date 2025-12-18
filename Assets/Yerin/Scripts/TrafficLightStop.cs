using UnityEngine;

public class TrafficLightStop : MonoBehaviour
{
    public CarControlTrafficLight carControlLight;

    private LightState currentLightState = LightState.Red;

    void Start()
    {
        if (carControlLight == null)
        {
            Debug.LogError($"[TrafficLightStop] {gameObject.name}에 CarControlTrafficLight가 연결 안됨!");
        }
        else
        {
            Debug.Log($"[TrafficLightStop] {gameObject.name} 초기화 완료");
        }
    }

    public void UpdateTrafficLightState(LightState newState)
    {
        currentLightState = newState;
        Debug.Log($"[TrafficLightStop] {gameObject.name} 신호 업데이트: {newState}");
    }

    void Update()
    {
        if (carControlLight != null)
        {
            LightState newState = carControlLight.GetCurrentState();
            if (newState != currentLightState)
            {
                currentLightState = newState;
                Debug.Log($"★★★ [TrafficLightStop Update] {gameObject.name} 신호 변경: {newState} ★★★");
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"■■■ [ENTER] {other.name} 진입! 현재 신호: {currentLightState} ■■■");

        CarAI car = other.GetComponent<CarAI>();
        if (car != null)
        {
            if (currentLightState == LightState.Red || currentLightState == LightState.Yellow)
            {
                car.SetCanMove(false);
                Debug.Log($"🔴 [ENTER] {other.name} 정지 명령!");
            }
            else if (currentLightState == LightState.Green)
            {
                car.SetCanMove(true);
                Debug.Log($"🟢 [ENTER] {other.name} 통과 명령!");
            }
        }
        else
        {
            Debug.LogWarning($"[ENTER] {other.name}에 CarAI 없음!");
        }
    }

    void OnTriggerStay(Collider other)
    {
        Debug.Log($"▶▶▶ [STAY] {other.name} 체류 중, 신호: {currentLightState}");  // 🔴 이게 떠야 함!

        CarAI car = other.GetComponent<CarAI>();
        if (car != null)
        {
            if (currentLightState == LightState.Green)
            {
                car.SetCanMove(true);
                Debug.Log($"🟢🟢🟢 [STAY] {other.name} 초록불! 출발 명령!");
            }
            else
            {
                car.SetCanMove(false);
                Debug.Log($"🔴 [STAY] {other.name} 정지 유지");
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log($"■■■ [EXIT] {other.name} 이탈! ■■■");

        CarAI car = other.GetComponent<CarAI>();
        if (car != null)
        {
            car.SetCanMove(true);
            Debug.Log($"🟢 [EXIT] {other.name} 완전 출발!");
        }
    }
}