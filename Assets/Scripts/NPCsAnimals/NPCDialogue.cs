using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCDialogue : MonoBehaviour, IInteractable
{
    [SerializeField] private string _npcName = "Villager";
    [SerializeField] private DialogueNode _rootNode;
    [SerializeField] private NPCData _npcData;
    [SerializeField] private GameObject _dialoguePanel;
    [SerializeField] private Text _npcNameText;
    [SerializeField] private Text _npcText;
    [SerializeField] private GameObject _choiceButtonPrefab;
    [SerializeField] private Transform _choicesContainer;

    private readonly List<GameObject> activeButtons = new List<GameObject>();
    private SpriteRenderer spriteRenderer;
    private GameObject prompt;
    private Color baseColor = Color.white;
    private bool isActive;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) baseColor = spriteRenderer.color;

        prompt = transform.Find("press tp enter_0")?.gameObject;
        hideChildSprite();

        if (_dialoguePanel != null) _dialoguePanel.SetActive(false);
        if (FriendshipManager.Instance != null) FriendshipManager.Instance.RegisterNPC(_npcName);
        if (_npcData != null) _npcData.npcName = _npcName;
    }

    public bool CanInteract()
    {
        return GetDialogueRoot() != null;
    }

    public void Interact()
    {
        if (!isActive && CanInteract()) StartDialogue();
    }

    [ContextMenu("Start Dialogue")]
    public void StartDialogue()
    {
        DialogueNode root = GetDialogueRoot();
        if (root == null)
        {
            Debug.LogWarning($"No dialogue tree set up for {_npcName}.", this);
            return;
        }

        isActive = true;
        if (_dialoguePanel != null) _dialoguePanel.SetActive(true);
        if (_npcNameText != null) _npcNameText.text = _npcName;
        GameEvents.OnDialogueStart.Invoke();
        ShowNode(root);
    }

    [ContextMenu("End Dialogue")]
    public void EndDialogue()
    {
        EndDialogue(true);
    }

    private void EndDialogue(bool increaseFriendship)
    {
        isActive = false;
        if (_dialoguePanel != null) _dialoguePanel.SetActive(false);
        ClearChoiceButtons();

        if (increaseFriendship && FriendshipManager.Instance != null) FriendshipManager.Instance.OnTalkedToNPC(_npcName);
        GameEvents.OnDialogueEnd.Invoke();
    }

    public void onFocusOn()
    {
        if (spriteRenderer != null) spriteRenderer.color = Color.yellow;
    }

    public void onFocusOff()
    {
        if (spriteRenderer != null) spriteRenderer.color = baseColor;
        if (isActive) EndDialogue(false);
    }

    public void hideChildSprite()
    {
        if (prompt != null) prompt.SetActive(false);
    }

    public void showChildSprite()
    {
        if (prompt != null) prompt.SetActive(true);
    }

    private DialogueNode GetDialogueRoot()
    {
        if (_npcData != null && FriendshipManager.Instance != null)
        {
            _npcData.friendshipPoints = FriendshipManager.Instance.GetFriendship(_npcName);
            DialogueNode tierNode = _npcData.GetDialogueForCurrentTier();
            if (tierNode != null) return tierNode;
        }

        return _rootNode;
    }

    private void ShowNode(DialogueNode node)
    {
        if (_npcText != null) _npcText.text = node.npcText;
        ClearChoiceButtons();

        if (node.IsEndNode())
        {
            SpawnChoiceButton("[ Close ]", null);
            return;
        }

        foreach (DialogueChoice choice in node.choices)
        {
            SpawnChoiceButton(choice.choiceText, choice.nextNode);
        }
    }

    private void SpawnChoiceButton(string label, DialogueNode nextNode)
    {
        if (_choiceButtonPrefab == null || _choicesContainer == null) return;

        GameObject buttonObject = Instantiate(_choiceButtonPrefab, _choicesContainer);
        buttonObject.name = "Choice_" + label;
        buttonObject.SetActive(true);

        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchorMin = new Vector2(0f, 0.5f);
            rect.anchorMax = new Vector2(1f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(0f, 38f);
        }

        LayoutElement layout = buttonObject.GetComponent<LayoutElement>();
        if (layout == null) layout = buttonObject.AddComponent<LayoutElement>();
        layout.minHeight = 38f;
        layout.preferredHeight = 38f;
        layout.flexibleWidth = 1f;

        Image image = buttonObject.GetComponent<Image>();
        if (image != null) image.color = new Color(0.20f, 0.38f, 0.62f, 0.96f);

        Text buttonText = buttonObject.GetComponentInChildren<Text>();
        if (buttonText != null)
        {
            buttonText.text = label;
            buttonText.fontSize = 17;
            buttonText.fontStyle = FontStyle.Bold;
            buttonText.alignment = TextAnchor.MiddleCenter;
            buttonText.color = Color.white;
            buttonText.horizontalOverflow = HorizontalWrapMode.Wrap;
            buttonText.verticalOverflow = VerticalWrapMode.Truncate;
        }

        Button button = buttonObject.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() =>
            {
                if (nextNode == null) EndDialogue();
                else ShowNode(nextNode);
            });
        }

        activeButtons.Add(buttonObject);
    }

    private void ClearChoiceButtons()
    {
        foreach (GameObject button in activeButtons)
        {
            if (button != null) Destroy(button);
        }

        activeButtons.Clear();
    }
}
