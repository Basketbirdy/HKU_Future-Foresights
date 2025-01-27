using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class TeleportUI : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private PlayerInput input;
    private InputAction openUI;

    [Header("UI")]
    [SerializeField] private Transform[] points;
    [Space]
    [SerializeField] private Transform buttonArea;
    [SerializeField] private GameObject buttonPrefab;

    [Header("Other")]
    [SerializeField] private Transform target;
    [SerializeField] private float popupSpeed;
    private EventSystem eventSystem;

    public static Action OnOpenUI;

    private List<Button> buttons = new List<Button>();
    private bool isActive = false;
    private Coroutine activeLerp;
    private RectTransform RectTransform;

    private void OnEnable()
    {
        openUI.Enable();
    }

    private void OnDisable()
    {
        openUI.Disable();
    }

    private void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
        eventSystem = FindFirstObjectByType<EventSystem>();
        openUI = input.actions.FindAction("Interact");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach(Transform tf in points)
        {
            GameObject button = Instantiate(buttonPrefab, buttonArea);
            Debug.Log($"Button is here: {button.transform.position}");

            buttons.Add(button.GetComponent<Button>());

            TeleportButton tp = button.GetComponent<TeleportButton>();

            if(tp == null ) { continue; }

            tp.Setup(tf, target);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(openUI.triggered) 
        {


            Debug.Log($"Triggered openui");
            ShowUI(); 
        }
    }

    private void ShowUI()
    {
        isActive = !isActive;

        OnOpenUI?.Invoke();

        if(activeLerp != null) { StopCoroutine(activeLerp); }

        if(isActive) 
        { 
            activeLerp = StartCoroutine(LerpLocalPosition(0f - (485f + 475f), popupSpeed));
            buttons[0].Select();
        }
        else 
        { 
            activeLerp = StartCoroutine(LerpLocalPosition(-475f - (485f + 475f), popupSpeed));
            eventSystem.SetSelectedGameObject(null);
        }
    }

    private IEnumerator LerpLocalPosition(float _localXPos, float _speed, float _graceDistance = 0.1f)
    {
        bool reachedEnd = false;
        Vector3 target = new Vector3(_localXPos, 0f, 0f);

        while (!reachedEnd)
        {
            float step = _speed * Time.deltaTime;

            RectTransform.localPosition = Vector3.MoveTowards(RectTransform.localPosition, target, step);

            if(RectTransform.localPosition.x > target.x - _graceDistance && RectTransform.localPosition.x <= target.x + _graceDistance)
            {
                reachedEnd = true;
            }

            yield return null;
        }
    }
}
