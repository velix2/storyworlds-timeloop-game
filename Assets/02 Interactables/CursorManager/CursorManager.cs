using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour
{
    
    
    private static Vector2 hotspot = new Vector2(24, -24);
    private static Sprite[,] cursorMap;

    private static Image cursorImage;
    private static Image itemPreview;
    private static GameObject cursorRoot;
    private static Vector2 itemOffset = new Vector2(65, -65);

    private static CursorManager instance;

    public static void CreateInstance()
    {
        GameObject go = new GameObject("Cursor Canvas");
        go.AddComponent<CursorManager>();
    }
    
    public void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("There already is a CursorManager instance...");
            return;
        }
        instance = this;
        DontDestroyOnLoad(this);
        
        //Canvas Setup
        Canvas canvas = gameObject.AddComponent<Canvas>();

        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;

        CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();

        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        //Cursor Root Setup
        cursorRoot = new GameObject("Cursor Root Root");
        cursorRoot.transform.SetParent(transform);
        
        //Cursor Image Setup
        cursorImage = new GameObject("Cursor Image").AddComponent<Image>();
        cursorImage.transform.SetParent(cursorRoot.transform);
        RectTransform rt = cursorImage.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = Vector2.zero;
        rt.anchoredPosition = hotspot;
        rt.sizeDelta = new Vector2(48, 48);
        
        //Item Icon Setup
        itemPreview = new GameObject("Image Holder").AddComponent<Image>();
        itemPreview.transform.SetParent(cursorRoot.transform);
        
        rt = itemPreview.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = Vector2.zero;
        rt.anchoredPosition = itemOffset;
        rt.sizeDelta = new Vector2(48, 48);
        
        //Setup Default cursor;
        LoadSpriteMap();
        Cursor.visible = false;
        ChangeCursorItem(null);
        ResetCursor();
        
    }

    private void Start()
    {
        SceneSwitcher.Instance.SceneSwitched.AddListener(ResetCursor);
    }

    public void LateUpdate()
    {
        cursorRoot.transform.position = InputManager.GetMousePosition();
    }
    
    /// <summary>
    /// Changes Cursor depending on the given Interactable.<br/>
    /// Used by InteractionChecker.
    /// </summary>
    /// <param name="interactable">Interactable the cursor should be based on</param>
    public static void ChangeCursorInteraction(Interactable interactable)
    {
        if (interactable == null) return;
        
        ApplyCursor(GetInteractionCursor(interactable));
        
        SetTransparency(interactable.PrimaryNeedsInRange && !interactable.inRange);
        Debug.Log($"Changed cursor! Primary: {interactable.Primary}, Secondary: {interactable.Secondary}");
        
    }

    /// <summary>
    /// Changes Cursor depending on given Item.<br/>
    /// Used by InteractionChecker.
    /// </summary>
    /// <param name="item">Item the cursor should be based on</param>
    /// <param name="transparent"></param>
    public static void ChangeCursorItem(ItemData item, bool transparent = false)
    {
        SetTransparency(transparent);
        if (item)
        {
            ApplyCursor(cursorMap[0, 0]);
            itemPreview.enabled = true;
            itemPreview.sprite = item.Sprite;
        }
        else itemPreview.enabled = false;
        Debug.Log($"Your cursor now shows that {item} is selected.");
    }
    
    /// <summary>
    /// Resets the Cursor to default.<br/>
    /// Used by InteractionChecker.
    /// </summary>
    public static void ResetCursor()
    {
        SetTransparency(false);
        ApplyCursor(cursorMap[0, 0]);
        itemPreview.enabled = false;
        Debug.Log("Cursor Reset!");
    }

    /// <summary>
    /// Sets tranparency of cursor.<br/>
    /// Indirectly used by InteractionChecker.
    /// </summary>
    /// <param name="transparent"></param>
    public static void SetTransparency(bool transparent)
    {
        Color c = cursorImage.color;
        c.a = transparent ? 0.5f : 1;
        cursorImage.color = c;
        itemPreview.color = c;
    }

    private static Sprite GetInteractionCursor(Interactable interactable)
    {
        if (cursorMap != null || LoadSpriteMap())
            return cursorMap[(int)interactable.Primary, (int)interactable.Secondary];
        return null;
    }
    
    private static void ApplyCursor(Sprite sprite)
    {
        cursorImage.sprite = sprite;
    }
    
    private static bool LoadSpriteMap()
    {
        Debug.Log("Loading CursorSpriteMap...");
        Sprite[] sprites = Resources.LoadAll<Sprite>("Cursor");
        int length = (int)Interactable.interactionType._enumlength;
        
        if (sprites.Length != length * length)
        {
            Debug.LogError("Number of sprites in Resources/Cursor do not equal number of possible interaction combinations! Diff: " + (sprites.Length - length * length));
            return false;
        }
        
        sprites = sprites
            .OrderByDescending(s => s.rect.y)
            .ThenBy(s => s.rect.x)
            .ToArray();

        cursorMap = new Sprite[length, length];

        int index = 0;
        for (int y = 0; y < length; y++)
        {
            for (int x = 0; x < length; x++)
            {
                cursorMap[x, y] = sprites[index++];
            }
        }

        return true;
    }
    
}
