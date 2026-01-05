<<<<<<< HEAD
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Required for Button

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float interactionRadius = 2f;

    [Header("UI Reference")]
    public GameObject TransportPanel;

    private CircleCollider2D interactionCollider;
    private NPC currentNPC;
    private WorldItem currentItem;

    private bool canInteract = false;
    private bool isCampfire = false;

    private Animator anim;

    public bool IsInteractable => canInteract;

    // Register Scene Loaded Event
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        interactionCollider = GetComponent<CircleCollider2D>();
        interactionCollider.radius = interactionRadius;
        interactionCollider.isTrigger = true;

        anim = GetComponent<Animator>();

        FindUIAndLinkButtons(); // Initial find
    }

    // Called every time a scene is loaded
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 1. Reset Interaction States
        isCampfire = false;
        canInteract = false;
        currentNPC = null;
        currentItem = null;

        // 2. Find UI and Re-link Buttons
        FindUIAndLinkButtons();
    }

    void FindUIAndLinkButtons()
    {
        // If already connected and active, just disable and return
        if (this.TransportPanel != null)
        {
            this.TransportPanel.SetActive(false);
            // We still need to check buttons just in case, but usually finding the window is enough if references held
            // However, safe to re-find if null
        }

        // Try to find the UI Window even if it's inactive
        if (TransportPanel == null)
        {
            GameObject foundObj = GameObject.Find("TransportPanel");
            if (foundObj == null)
            {
                // Fallback: Search inside Canvas
                Canvas canvas = FindFirstObjectByType<Canvas>();
                if (canvas != null)
                {
                    Transform t = canvas.transform.Find("TransportPanel");
                    if (t != null) foundObj = t.gameObject;
                }
            }
            TransportPanel = foundObj;
        }

        // Link Buttons if window is found
        if (this.TransportPanel != null)
        {
            // Important: Names "Btn_Craft" and "Btn_Potion" must match your Hierarchy exactly!
            Button btnCraft = this.TransportPanel.transform.Find("Btn_Craft")?.GetComponent<Button>();
            Button btnPotion = this.TransportPanel.transform.Find("Btn_Potion")?.GetComponent<Button>();

            if (btnCraft != null)
            {
                btnCraft.onClick.RemoveAllListeners(); // Clear old links
                btnCraft.onClick.AddListener(GoCrafting); // Link new method
            }

            if (btnPotion != null)
            {
                btnPotion.onClick.RemoveAllListeners();
                btnPotion.onClick.AddListener(GoPotion);
            }

            this.TransportPanel.SetActive(false); // Ensure it starts hidden
        }
    }

    // Update Method for Interaction
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.F))
        {
            if (!canInteract) return;

            if (Player.Instance != null) Player.Instance.CancelAttack();
            if (Player.Instance != null) Player.Instance.StopMoving();

            if (currentItem != null)
            {
                PickUpItem();
                return;
            }

            if (isCampfire)
            {
                if (TransportPanel != null)
                {
                    bool isActive = TransportPanel.activeSelf;
                    TransportPanel.SetActive(!isActive);
                }
                else
                {
                    // Fail-safe re-find
                    FindUIAndLinkButtons();
                    if (TransportPanel != null) TransportPanel.SetActive(true);
                }
                return;
            }

            if (currentNPC != null)
            {
                // Dialogue logic...
                if (DialogueManager.Instance == null) return;
                if (!DialogueManager.Instance.IsDialogueActive())
                    StartDialogue();
                else
                    DialogueManager.Instance.AdvanceDialogue();
            }
        }
    }

    public void GoPotion() { StartCoroutine(LoadSceneWithFade("Potions")); }
    public void GoCrafting() { StartCoroutine(LoadSceneWithFade("Crafting")); }

    IEnumerator LoadSceneWithFade(string sceneName)
    {
        if (Player.Instance != null) Player.Instance.SaveCurrentPosition();
        if (UIManager.Instance != null) yield return StartCoroutine(UIManager.Instance.FadeOut(0.5f));
        SceneManager.LoadScene(sceneName);
    }

    void PickUpItem()
    {
        //if (currentItem != null && Inventory.Instance != null)
        //{
        //    if (Inventory.Instance.AddItem(currentItem.itemData, currentItem.quantity))
        //    {
        //        Destroy(currentItem.gameObject);
        //        currentItem = null;
        //        canInteract = false;
        //    }
        //}

        if (currentItem != null)
        {
                Destroy(currentItem.gameObject);
                currentItem = null;
                canInteract = false;
        }
    }

    void StartDialogue()
    {
        Vector2 dir = (transform.position - currentNPC.transform.position).normalized;
        currentNPC.FaceDirection(dir);
        DialogueManager.Instance.StartDialogue(currentNPC.dialogueData);
    }

    void StopMoveAnimation()
    {
        if (anim != null)
        {
            anim.SetBool("IsMoving", false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<NPC>()) { currentNPC = other.GetComponent<NPC>(); canInteract = true; }
        else if (other.CompareTag("Campfire")) { isCampfire = true; canInteract = true; }
        else if (other.GetComponent<WorldItem>()) { currentItem = other.GetComponent<WorldItem>(); canInteract = true; }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<NPC>() == currentNPC) { currentNPC = null; }
        else if (other.CompareTag("Campfire"))
        {
            isCampfire = false;
            if (TransportPanel != null) TransportPanel.SetActive(false);
        }
        else if (other.GetComponent<WorldItem>() == currentItem) { currentItem = null; }

        if (currentNPC == null && !isCampfire && currentItem == null) canInteract = false;
    }
=======
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float interactionRadius = 16f;

    // NpcLayer는 Trigger 방식에서는 안 쓸 수도 있지만, OverlapCircle 쓸 거면 필요함
    // 여기서는 기존 Trigger 방식을 유지하므로 그냥 둡니다.
    // [SerializeField] private LayerMask npcLayer; 

    private CircleCollider2D interactionCollider;
    private Player playerMovement; // 부모에 있는 스크립트
    private NPC currentNPC;
    private bool canInteract = false;

    void Start()
    {
        interactionCollider = GetComponent<CircleCollider2D>();
        interactionCollider.radius = interactionRadius;
        interactionCollider.isTrigger = true;

        // ★ [수정됨] 이 스크립트는 이제 자식 오브젝트에 있으므로, 
        // 부모 오브젝트에서 Player 컴포넌트를 찾아와야 합니다.
        playerMovement = GetComponentInParent<Player>();
    }

    void Update()
    {
        // Z키 입력 감지
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (canInteract && currentNPC != null)
            {
                if (!DialogueManager.Instance.IsDialogueActive())
                {
                    InteractWithNPC();
                }
                else
                {
                    DialogueManager.Instance.AdvanceDialogue();
                }
            }
        }

        // (선택 사항) 센서 위치를 항상 부모(플레이어) 중심에 고정
        // 자식 오브젝트라서 보통 자동으로 따라다니지만, 확실하게 하기 위해 transform.localPosition을 0으로 둬도 됩니다.
        transform.localPosition = Vector3.zero;
    }

    void InteractWithNPC()
    {
        if (currentNPC != null)
        {
            // 방향 계산: 내 위치(센서)나 부모 위치나 거의 같음
            Vector2 direction = (transform.position - currentNPC.transform.position).normalized;
            currentNPC.FaceDirection(direction);

            DialogueManager.Instance.StartDialogue(currentNPC.dialogueData);
        }
    }

    // ★ Trigger 함수는 이제 '자식 오브젝트'의 콜라이더에 닿았을 때 실행됩니다.
    // 자식의 태그는 Untagged이므로 총알은 이 함수와 상관없이 그냥 통과합니다(총알 로직에서 Player 태그만 죽이니까).
    void OnTriggerEnter2D(Collider2D other)
    {
        NPC npc = other.GetComponent<NPC>();
        if (npc != null)
        {
            currentNPC = npc;
            canInteract = true;
            // 디버그용: 범위 들어왔는지 확인
            // Debug.Log("NPC 감지됨: " + npc.name);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        NPC npc = other.GetComponent<NPC>();
        if (npc != null && npc == currentNPC)
        {
            currentNPC = null;
            canInteract = false;
        }
    }
>>>>>>> 90c0ad74131c4196343f1315151c27ea4d457ad6
}