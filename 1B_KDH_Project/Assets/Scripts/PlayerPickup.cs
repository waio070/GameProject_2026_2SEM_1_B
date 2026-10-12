using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class PlayerPickup : MonoBehaviour
{
    [Header("직접 연결")]
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private InteractionUI interactionUI;

    [Header("아이템 탐색")]
    [SerializeField] private float detectionRadius = 2f;

    [Header("줍기 모션")]
    [SerializeField] private float pickupDuration = 1.2f;
    [SerializeField] private float pickupMoment = 0.55f;

    private PickupItem targetItem;
    private Door targetDoor;
    private NPC targetNPC;
    private bool isPickingUp;

    void Update()
    {
        if (isPickingUp) return;

        // Player 주변에서 가장 가까운 아이템, 문, NPC를 찾는다.
        targetItem = FindClosestItem();
        targetDoor = FindClosestDoor();
        targetNPC = FindClosestNPC();

        ShowGuide();

        Keyboard keyboard = Keyboard.current;

        if (keyboard == null) return;

        if (targetItem == null && targetDoor == null && targetNPC == null) return;

        if (keyboard.eKey.wasPressedThisFrame)
        {
            Interact();
        }
    }

    private void ShowGuide()
    {
        if (targetItem != null)
        {
            interactionUI.ShowGuide("[E] " + targetItem.GetItemName() + " 줍기");
        }
        else if (targetDoor != null)
        {
            interactionUI.ShowGuide(targetDoor.GetGuideText());
        }
        else if (targetNPC != null)
        {
            interactionUI.ShowGuide(targetNPC.GetGuideText());
        }
        else
        {
            interactionUI.HideGuide();
        }
    }

    // 인터랙션 함수
    private void Interact()
    {
        if (targetItem != null)
        {
            // 기존 픽업
            StartCoroutine(PickupAnimation());
        }
        else if (targetDoor != null)
        {
            // 문 오픈
            targetDoor.Interact();
        }
        else if (targetNPC != null)
        {
            // 다이얼로그 오픈
            interactionUI.ShowDialogue(targetNPC.GetNPCName(), targetNPC.GetMessage());
        }
    }

    private PickupItem FindClosestItem()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, ~0, QueryTriggerInteraction.Collide); // 구체의 충돌체를 만든다.
        PickupItem closestItem = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider itemCollider in colliders) // 충돌 구체 들을 돌아가면서 검사
        {
            PickupItem item = itemCollider.GetComponentInParent<PickupItem>(); // 컴포넌트가 PickupItem을 검사

            if (item == null) continue;

            float distance = Vector3.Distance(transform.position, item.transform.position);

            if (distance < closestDistance) // 가장 가까운 물체를 찾는다.
            {
                closestDistance = distance;
                closestItem = item;
            }
        }

        return closestItem;
    }

    private Door FindClosestDoor()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, ~0, QueryTriggerInteraction.Collide); // 구체의 충돌체를 만든다.
        Door closestDoor = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider targetCollider in colliders) // 충돌 구체 들을 돌아가면서 검사
        {
            Door door = targetCollider.GetComponentInParent<Door>(); // 컴포넌트가 Door 인지 검사

            if (door == null) continue;

            float distance = Vector3.Distance(transform.position, door.transform.position);

            if (distance < closestDistance) // 가장 가까운 문을 찾는다.
            {
                closestDistance = distance;
                closestDoor = door;
            }
        }

        return closestDoor;
    }

    private NPC FindClosestNPC()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, ~0, QueryTriggerInteraction.Collide); // 구체의 충돌체를 만든다.
        NPC closestNPC = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider targetCollider in colliders) // 충돌 구체 들을 돌아가면서 검사
        {
            NPC npc = targetCollider.GetComponentInParent<NPC>(); // 컴포넌트가 NPC 인지 검사

            if (npc == null) continue;

            float distance = Vector3.Distance(transform.position, npc.transform.position);

            if (distance < closestDistance) // 가장 가까운 NPC를 찾는다.
            {
                closestDistance = distance;
                closestNPC = npc;
            }
        }

        return closestNPC;
    }

    private IEnumerator PickupAnimation()
    {
        isPickingUp = true;

        interactionUI.HideGuide();

        // 현재 찾은 아이템을 저장한다.
        PickupItem itemToCollect = targetItem;

        // 아이템 방향으로 캐릭터를 돌린다.
        Vector3 itemDirection = itemToCollect.transform.position - transform.position;
        itemDirection.y = 0;

        if (itemDirection.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(itemDirection);
        }

        // 줍기 상태로 변경하여 이동을 막는다.
        playerController.ChangeState(PlayerState.Pickup);

        // 전환선 없이 PickUp01 상태로 이동한다.
        animator.CrossFade("PickUp01", 0.1f);

        // 손이 아이템에 닿는 시점까지 기다린다.
        yield return new WaitForSeconds(pickupMoment);

        // 아이템을 실제로 획득한다.
        if (itemToCollect != null)
        {
            itemToCollect.Collect();
        }

        // 남은 애니메이션 시간을 기다린다.
        yield return new WaitForSeconds(pickupDuration - pickupMoment);

        // 이동 Blend Tree로 돌아간다.
        animator.CrossFade("Blend Tree", 0.1f);

        // Normal 상태로 돌아가 이동을 허용한다.
        playerController.ChangeState(PlayerState.Normal);

        targetItem = null;
        targetDoor = null;
        targetNPC = null;
        isPickingUp = false;
    }
}