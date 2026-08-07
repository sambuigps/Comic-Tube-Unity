using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

#region structs

[System.Serializable]
public struct UI_Menu
{
    public UI_Type type;
    public UI_BaseClass reference;

    public UI_Menu(UI_Type type, UI_BaseClass reference)
    {
        this.type = type;
        this.reference = reference;
    }
}

#endregion

[System.Serializable]
public class UI_Manager_Data: BaseDataClass
{
    public List<UI_Menu> allMenus = new List<UI_Menu>();
    public GameObject parent;
}

public class UI_Manager : BaseService<UI_Manager_Data>
{
    public UI_Type currentMenu;

    private UI_BaseClass currentActiveMenu;

    private bool isTransitioning;
    private readonly Queue<UI_Type> pendingMenus = new();

    private Dictionary<UI_Type, UI_BaseClass> menus = new();

    public override void Awake()
    {
        foreach (var menu in data.allMenus)
        {
            menus.Add(menu.type, menu.reference);
            menu.reference?.gameObject.SetActive(false);
        }
    }

    public override void Start()
    {
        SetUI(UI_Type.SignUp);
    }

    public void Disable()
    {
        data.parent?.SetActive(false);
    }

    public void Enable()
    {
        data.parent?.SetActive(true);
    }

    public void SetUI(UI_Type uI_Menu)
    {
        if (uI_Menu == currentMenu && !isTransitioning) return;

        if (isTransitioning)
        {
            pendingMenus.Enqueue(uI_Menu);
            return;
        }

        StartTransition(uI_Menu);
    }
    public T GetUIScript<T>() where T : UI_BaseClass
    {
        foreach (var menu in data.allMenus)
        {
            if (menu.reference is T ui)
                return ui;
        }
        return null;
    }

    #region transistion logic
    private void StartTransition(UI_Type uI_Menu)
    {
        UI_BaseClass nextMenu = menus[uI_Menu];
        currentMenu = uI_Menu;

        NextPanel(nextMenu);
        currentActiveMenu = nextMenu;

        if (nextMenu != null) nextMenu.Init();
    }

    private void NextPanel(UI_BaseClass nextPanel)
    {
        TransitionToNextPanel(currentActiveMenu?.gameObject, nextPanel?.gameObject, false);
    }

    public void TransitionToNextPanel(GameObject prevPanel, GameObject nextPanel, bool keepDisabled)
    {
        if (nextPanel == null)
        {
            prevPanel?.SetActive(false);
            return;
        }

        if (prevPanel == null)
        {
            nextPanel.SetActive(true);
            return;
        }

        isTransitioning = true;

        nextPanel.transform.SetAsLastSibling();

        prevPanel.SetActive(false);
        nextPanel.SetActive(true);

        CanvasGroup nextGroup = nextPanel.GetComponent<CanvasGroup>();
        if (nextGroup == null)
            nextGroup = nextPanel.AddComponent<CanvasGroup>();

        nextGroup.alpha = 1;
        nextGroup.interactable = !keepDisabled;
        nextGroup.blocksRaycasts = !keepDisabled;

        isTransitioning = false;

        while (pendingMenus.Count > 0)
        {
            UI_Type nextRequest = pendingMenus.Dequeue();
            if (nextRequest == currentMenu) continue;
            StartTransition(nextRequest);
            break;
        }
    }
    #endregion
}

[System.Serializable]
public enum UI_Type
{
    None,
    SignIn,
    SignUp
}