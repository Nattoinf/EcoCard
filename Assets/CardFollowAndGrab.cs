using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CardFollowAndGrab : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform controller;          // 追従するコントローラ
    public Transform playerCamera;        // プレイヤーカメラ
    public Vector3 offset = new Vector3(0.1f, 0f, 0f);
    public float followSpeed = 10f;

    [Header("Visual")]
    public Transform visual;              // ★ 見た目専用（必須）

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;
    private bool isGrabbed = false;
    private bool isInSlot = false;

    void Awake()
    {
        grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        grab.selectEntered.AddListener(OnGrab);
        grab.selectExited.AddListener(OnRelease);
    }

    void Update()
    {
        // ★ Slot に入ったら完全に何もしない
        if (isInSlot) return;

        // ★ 掴まれていない時だけ追従
        if (!isGrabbed)
        {
            Vector3 targetPos =
                controller.position + controller.rotation * offset;

            transform.position = Vector3.Lerp(
                transform.position,
                targetPos,
                Time.deltaTime * followSpeed
            );
        }

        // ★ 見た目だけプレイヤーを向く
        LookAtPlayerVisualOnly();
    }

    // ===== 見た目専用 LookAt（XR安全）=====
    private void LookAtPlayerVisualOnly()
    {
        if (playerCamera == null || visual == null) return;

        Vector3 dir = visual.position - playerCamera.position;

        // ★ Y軸は無視（カメラ回転防止の要）
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.0001f) return;

        visual.rotation = Quaternion.LookRotation(dir);
    }

    // ===== Grab =====
    private void OnGrab(SelectEnterEventArgs args)
    {
        isGrabbed = true;

        var data = GetComponent<CardData>();
        if (data != null && SlotManager.Instance != null)
        {
            SlotManager.Instance.HighlightAvailableSlots(data);
        }
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        isGrabbed = false;

        if (SlotManager.Instance != null)
        {
            SlotManager.Instance.ClearHighlight();
        }
    }


    // ===== Slot から呼ばれる =====
    public void EnterSlot()
    {
        isInSlot = true;

        // Grab を無効化（親子関係を断つ）
        if (grab != null)
            grab.enabled = false;
    }

    public void ExitSlot()
    {
        isInSlot = false;

        if (grab != null)
            grab.enabled = true;
    }
}
