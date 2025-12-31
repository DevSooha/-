using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float interactionRadius = 2f;

    [Header("UI 연결")]
    public GameObject craftUIWindow; // 선택 UI Panel

    private CircleCollider2D interactionCollider;
    private NPC currentNPC;

    private bool canInteract = false;
    private bool isCampfire = false;

    public bool IsInteractable => canInteract;


    void Start()
    {
        interactionCollider = GetComponent<CircleCollider2D>();
        interactionCollider.radius = interactionRadius;
        interactionCollider.isTrigger = true;

        if (craftUIWindow != null)
            craftUIWindow.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.F))
        {
            if (!canInteract) return;

            // 모닥불 우선
            if (isCampfire)
            {
                OpenCraftingUI();
                return;
            }

            // NPC는 그 다음
            if (currentNPC != null)
            {
                if (DialogueManager.Instance == null) return;

                if (!DialogueManager.Instance.IsDialogueActive())
                    StartDialogue();
                else
                    DialogueManager.Instance.AdvanceDialogue();
            }
        }
    }


    void StartDialogue()
    {
        Vector2 dir = (transform.position - currentNPC.transform.position).normalized;
        currentNPC.FaceDirection(dir);
        DialogueManager.Instance.StartDialogue(currentNPC.dialogueData);
    }

    void OpenCraftingUI()
    {
        Debug.Log("OpenCraftingUI CALLED");
        if (craftUIWindow != null) craftUIWindow.SetActive(true);
    }


    // ================= 버튼용 =================

    public void GoPotion()
    {
        StartCoroutine(LoadSceneWithFade("Potions"));
    }

    public void GoCrafting()
    {
        StartCoroutine(LoadSceneWithFade("Crafting"));
    }

    IEnumerator LoadSceneWithFade(string sceneName)
    {
        // ★ [추가] 씬 떠나기 전에 "나 여기 있었다"고 저장!
        if (Player.Instance != null)
        {
            Player.Instance.SaveCurrentPosition();
        }

        // 1. 화면 어두워지기 (페이드 아웃)
        if (FadeManager.Instance != null)
            yield return StartCoroutine(FadeManager.Instance.FadeOut(0.5f));

        // 2. 씬 이동
        SceneManager.LoadScene(sceneName);
    }

    // ================= Trigger =================

    void OnTriggerEnter2D(Collider2D other)
    {
        NPC npc = other.GetComponent<NPC>();
        if (npc != null)
        {
            currentNPC = npc;
            canInteract = true;
            return;
        }

        if (other.CompareTag("Campfire"))
        {
            isCampfire = true;
            canInteract = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        NPC npc = other.GetComponent<NPC>();
        if (npc != null && npc == currentNPC)
        {
            currentNPC = null;
            if (!isCampfire) canInteract = false;
        }

        if (other.CompareTag("Campfire"))
        {
            isCampfire = false;
            if (currentNPC == null) canInteract = false;

            if (craftUIWindow != null)
                craftUIWindow.SetActive(false);
        }
    }
}
