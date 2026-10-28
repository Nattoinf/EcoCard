using UnityEngine;

/// <summary>
/// HMD (Main Camera) の視界（画面右下など）に完全同期して追従させるコンポーネント
/// </summary>
[ExecuteAlways]
public class BinderFollower : MonoBehaviour
{
    public enum FollowMode
    {
        ScreenFixed,   // 視界（画面）の右下に常に完全固定（上下左右どこを見ても画面内に留まる）
        BodyHorizontal // 身体の右横（腰あたり）に固定（頭の上下の傾きは無視）
    }

    [Header("Target Camera")]
    [Tooltip("追従対象のカメラ（未設定の場合は Main Camera を自動検索）")]
    public Transform targetCamera;

    [Header("Follow Mode")]
    [Tooltip("ScreenFixed: 視界の右下に常に張り付く / BodyHorizontal: 身体の右側に張り付く")]
    public FollowMode followMode = FollowMode.ScreenFixed;

    [Header("Position & Rotation Offset")]
    [Tooltip("カメラからの相対位置 (X: 右/左, Y: 上/下, Z: 前/後)")]
    public Vector3 offset = new Vector3(0.28f, -0.18f, 0.45f);

    [Tooltip("カードの角度オフセット (X: 上下の傾き, Y: 左右の向き, Z: 傾き)")]
    public Vector3 rotationOffset = new Vector3(15f, -15f, 0f);

    [Header("Speed Settings")]
    [Tooltip("完全同期（遅延ゼロで画面に張り付く）")]
    public bool instantFollow = true;

    [Tooltip("スムーズ追従時の速度 (instantFollow が false の場合のみ有効)")]
    [Range(1f, 50f)]
    public float smoothSpeed = 25f;

    [Header("Advanced")]
    [Tooltip("実行時にカメラの子オブジェクトとして直接マウントする（最も確実・ブレなし）")]
    public bool attachDirectlyToCamera = false;

    private void Awake()
    {
        EnsureCameraReference();
    }

    private void Start()
    {
        EnsureCameraReference();

        if (Application.isPlaying && attachDirectlyToCamera && targetCamera != null)
        {
            transform.SetParent(targetCamera, false);
            transform.localPosition = offset;
            transform.localRotation = Quaternion.Euler(rotationOffset);
            return;
        }

        UpdatePositionAndRotation(true);
    }

    private void Update()
    {
        if (attachDirectlyToCamera && transform.parent == targetCamera) return;
        UpdatePositionAndRotation(instantFollow);
    }

    private void LateUpdate()
    {
        if (attachDirectlyToCamera && transform.parent == targetCamera) return;
        UpdatePositionAndRotation(instantFollow);
    }

    public void UpdatePositionAndRotation(bool instant)
    {
        if (targetCamera == null)
        {
            EnsureCameraReference();
            if (targetCamera == null) return;
        }

        Quaternion baseRot;
        if (followMode == FollowMode.ScreenFixed)
        {
            // 視界（カメラの全回転）に完全追従
            baseRot = targetCamera.rotation;
        }
        else
        {
            // 身体（水平 Yaw 回転のみ）追従
            Vector3 forward = targetCamera.forward;
            forward.y = 0f;
            if (forward.sqrMagnitude < 0.0001f) forward = Vector3.forward;
            baseRot = Quaternion.LookRotation(forward.normalized, Vector3.up);
        }

        Vector3 targetPos = targetCamera.position + baseRot * offset;
        Quaternion targetRot = baseRot * Quaternion.Euler(rotationOffset);

        if (instant || !Application.isPlaying)
        {
            transform.position = targetPos;
            transform.rotation = targetRot;
        }
        else
        {
            float dt = Time.deltaTime * smoothSpeed;
            transform.position = Vector3.Lerp(transform.position, targetPos, dt);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, dt);
        }
    }

    private void EnsureCameraReference()
    {
        if (targetCamera != null) return;

        if (Camera.main != null)
        {
            targetCamera = Camera.main.transform;
            return;
        }

        // Camera.main が見つからない場合のフォールバック検索
        Camera cam = FindFirstObjectByType<Camera>();
        if (cam != null)
        {
            targetCamera = cam.transform;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Transform cam = targetCamera != null ? targetCamera : (Camera.main != null ? Camera.main.transform : null);
        if (cam != null)
        {
            Quaternion rot = (followMode == FollowMode.ScreenFixed)
                ? cam.rotation
                : Quaternion.Euler(0f, cam.eulerAngles.y, 0f);

            Vector3 targetPos = cam.position + rot * offset;

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(targetPos, 0.06f);
            Gizmos.DrawLine(cam.position, targetPos);
        }
    }
}
