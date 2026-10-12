using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float openSpeed = 3f;

    private bool isOpen;
    [SerializeField] private Quaternion closedRotation;
    [SerializeField] private Quaternion openRoatation;

    void Start()
    {
        closedRotation = transform.localRotation;
        openRoatation = closedRotation * Quaternion.Euler(0f, openAngle, 0f);
    }

    void Update()
    {
        Quaternion targetRotation;

        if (isOpen)
        {
            targetRotation = openRoatation;
        }
        else
        {
            targetRotation = closedRotation;
        }

        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, openSpeed * Time.deltaTime);
    }

    // 인터렉션 함수가 호출 되었을 때
    public void Interact()
    {
        // bool 값을 뒤집는다.
        isOpen = !isOpen;
    }

    public string GetGuideText()
    {
        if (isOpen)
        {
            return "[E] 문 닫기";
        }
        return "[E] 문 열기";
    }
}