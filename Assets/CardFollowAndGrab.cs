using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using System.Collections;


public class CardFollowAndGrab : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform controller;
    public Transform playerCamera;
    public Vector3 offset = new Vector3(0.1f, 0f, 0f);
    public float followSpeed = 10f;

    [Header("Visual")]
    public Transform visual;

    public bool IsUsed { get; private set; } = false;

    private XRGrabInteractable grab;

    private Rigidbody rb;

    private bool isGrabbed = false;

    private bool isInSlot = false;

    public bool IsGrabbed => isGrabbed;

    public bool IsInSlot => isInSlot;

    private bool isInBinder = false;

    // ===== Binder =====
    private BinderManager binder;

    public void SetBinder(BinderManager b)
    {
        binder = b;
        isInBinder = true;
    }

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();

        grab.selectEntered.AddListener(OnGrab);
        grab.selectExited.AddListener(OnRelease);
    }

    void Update()
    {

        LookAtPlayerVisualOnly();
    }

    // 見た目だけプレイヤー方向を向く
    private void LookAtPlayerVisualOnly()
    {
        if (playerCamera == null || visual == null)
            return;

        Vector3 dir = visual.position - playerCamera.position;

        dir.y = 0f;

        if (dir.sqrMagnitude < 0.0001f)
            return;

        visual.rotation = Quaternion.LookRotation(dir);
    }

    // Grab開始
    private void OnGrab(SelectEnterEventArgs args)
    {
        isGrabbed = true;

        if (binder != null && isInBinder)
        {
            BinderManager.Instance.RemoveCard(this);

            transform.SetParent(null);

            isInBinder = false;
        }

        var data = GetComponent<CardData>();

        if (data != null && SlotManager.Instance != null)
        {
            SlotManager.Instance.HighlightAvailableSlots(data);
        }
    }

    // Grab終了
    private void OnRelease(SelectExitEventArgs args)
    {
        isGrabbed = false;

        rb.isKinematic =false;
        rb.useGravity = true;
        if(SlotManager.Instance !=null){
            SlotManager.Instance.ClearHighlight ();
        }
        StartCoroutine(CheckReturnToBinder());
        /*
        if (!isInSlot)
        {
            if(BinderManager.Instance !=null){
                BinderManager.Instance.AddCard(this);
            }else{
                Debug.LogError("BinderManager is null");
            }
        }
        */
    }

    private IEnumerator CheckReturnToBinder(){
        yield return null;

        if(!isInSlot){
            BinderManager.Instance?.AddCard(this);
        }
    }

    // Slotへ配置
    public void EnterSlot()
    {
        isInSlot = true;

        if (grab != null)
            grab.enabled = false;

        if(rb != null){
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    // Slotから外れた
    public void ExitSlot()
    {
        isInSlot = false;

        if (grab != null)
            grab.enabled = true;
    }

    public void MoveToBinder(){
        
        isInBinder = true;

        rb.isKinematic =true;
        rb.useGravity = false;
    }

    public void MarkUsed(){
        IsUsed = true;
    }
}
