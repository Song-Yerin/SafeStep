using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class PathVisualizer : MonoBehaviour
{
    [Header("연결")]
    public Transform target; // 추적할 대상 (플레이어 머리)

    [Header("설정")]
    public float minDistance = 0.1f; // 최소 이만큼 움직여야 점을 찍음 (너무 촘촘하면 성능 저하)
    public float lineWidth = 0.05f;  // 선 두께
    public Color pathColor = Color.yellow; // 선 색상
    public float heightOffset = 0.05f; // 바닥보다 살짝 띄우기 (Z-fighting 방지)

    private LineRenderer line;
    private bool isRecording = false;
    private List<Vector3> points = new List<Vector3>();

    void Awake()
    {
        // 라인 렌더러 초기 세팅
        line = GetComponent<LineRenderer>();
        line.positionCount = 0;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.material = new Material(Shader.Find("Sprites/Default")); // 기본 쉐이더 사용
        line.startColor = pathColor;
        line.endColor = pathColor;
        line.useWorldSpace = true; // 월드 좌표계 사용
    }

    // 기록 시작 (훈련 시작 시 호출)
    public void StartDrawing()
    {
        points.Clear();
        line.positionCount = 0;
        isRecording = true;
        AddPoint(); // 시작점 찍기
    }

    // 기록 중지 (훈련 종료 시 호출)
    public void StopDrawing()
    {
        isRecording = false;
    }

    void Update()
    {
        if (!isRecording || target == null) return;

        // 마지막 찍은 점과 현재 위치 거리가 일정 이상일 때만 점 추가
        if (points.Count == 0 || Vector3.Distance(GetTargetPos(), points[points.Count - 1]) > minDistance)
        {
            AddPoint();
        }
    }

    void AddPoint()
    {
        Vector3 newPos = GetTargetPos();
        points.Add(newPos);

        line.positionCount = points.Count;
        line.SetPosition(points.Count - 1, newPos);
    }

    // 플레이어의 현재 발바닥 위치 계산
    Vector3 GetTargetPos()
    {
        // 머리(HMD) 위치를 그대로 쓰되, 높이(Y)만 바닥 근처로 고정
        return new Vector3(target.position.x, heightOffset, target.position.z);
    }
}