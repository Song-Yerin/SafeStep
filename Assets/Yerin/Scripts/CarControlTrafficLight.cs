using HealthbarGames;
using UnityEngine;

public class CarControlTrafficLight : TrafficLightBase
{
    [Header("차량 제어")]
    public TrafficLightStop[] stopLines;

    private LightState currentState = LightState.Red;

    public override void OnLightStateChanged(bool redLightState, bool yellowLightState, bool greenLightState)
    {
        //Debug.Log($"🚦 [CarControlTrafficLight] OnLightStateChanged 호출 - R:{redLightState} Y:{yellowLightState} G:{greenLightState}");

        // 상태 업데이트
        if (redLightState && !yellowLightState && !greenLightState)
        {
            currentState = LightState.Red;
            //Debug.Log("🔴 [CarControlTrafficLight] 빨강으로 변경");
        }
        else if (!redLightState && yellowLightState && !greenLightState)
        {
            currentState = LightState.Yellow;
            //Debug.Log("🟡 [CarControlTrafficLight] 노랑으로 변경");
        }
        else if (!redLightState && !yellowLightState && greenLightState)
        {
            currentState = LightState.Green;
            //Debug.Log("🟢 [CarControlTrafficLight] 초록으로 변경");
        }

        //Debug.Log($"✅ [CarControlTrafficLight] 최종 상태: {currentState}");

        // 정지선들에 전달
        foreach (var stopLine in stopLines)
        {
            if (stopLine != null)
            {
                stopLine.UpdateTrafficLightState(currentState);
            }
        }
    }

    public LightState GetCurrentState()
    {
        //Debug.Log($"📡 [CarControlTrafficLight] GetCurrentState 호출됨 → {currentState}");
        return currentState;
    }
}

public enum LightState
{
    Red,
    Yellow,
    Green
}