using UnityEngine;
using System.Collections;
using UnityEngine.UI; // Required when Using UI elements.
using TMPro;
using Fusion;
using System.Xml;
using UnityEngine.InputSystem;
using System;

public class ChatSystem : NetworkBehaviour
{
    public PlayerInputAction playerControls;
    private InputAction sumit;
    

    [Networked(OnChanged = nameof(OnChangeChatLog))]
    public NetworkString<_16> myChat { get; set; }

    [Networked(OnChanged = nameof(OnChangeChatLog))]
    public string sendName { get; set; }

    public TMP_Text myNameText;

    public string myName;

    [SerializeField] InputField mainInputField;

    bool isEnter = false;
    bool chatOnOff = false;

    public TMP_Text chatLog;
    public Scrollbar scrollV;

    bool chatDown = false;



    //클라이언트에서 쳇이 onoffonoff반복하는거 방지
    float inputTime = 0f;
    // Checks if there is anything entered into the input field.
    public void Awake()
    {
        //이걸 어웨이크에서 안하면 null이라 오류남 
        playerControls = new PlayerInputAction();
        chatLog = GameObject.FindWithTag("ChatDisplay").GetComponentInChildren<TMP_Text>();
        scrollV = GameObject.FindWithTag("ScrollV").GetComponent<Scrollbar>();

    }
    public void Start()
    {


        mainInputField.characterLimit = 1024;
        if (Object.HasInputAuthority)
        {
            mainInputField.enabled = true;
        }
        Debug.Log("chatLog = " + chatLog);



    }

    public override void FixedUpdateNetwork()
    {

    }
    private void Update()
    {

    }
    private void FixedUpdate()
    {
        
        if (chatDown)
        {
            Debug.Log("chatDown");

            Summit();
            Debug.Log("Enter로  Summit 실행");
            chatDown = false;
        }
        scrollV.value = 0;

    }
    public void Summit()
    {

        if (mainInputField.interactable)
        {
            if (mainInputField.text != "" && mainInputField.text != " ")
            {
                Debug.Log(mainInputField.text.Length);
                myChat = mainInputField.text;
                chatLog.text += $"\n {myName} : {myChat}";
                RPC_SetChat(myChat.ToString(), myName);
                Debug.Log($"Send MyChat = {myChat}");
                mainInputField.text = "";

            }
            mainInputField.interactable = false;

        }
        else
        {
            Debug.Log("채팅 활성화");
            mainInputField.interactable = true;
            mainInputField.Select();
        }

    }

    static void OnChangeChatLog(Changed<ChatSystem> changed)
    {
        Debug.Log("mychat = " + changed.Behaviour.myChat);
        if (changed.Behaviour.myChat == "")
        {
            return;
        }


        changed.Behaviour.PushMessage();

    }


    public void PushMessage()
    {
        string nullcheck = null;
        foreach (var chatSystem in myChat)
        {
            nullcheck += chatSystem;

            Debug.Log($"이거{chatSystem}이거");
        }
        if (Object.HasInputAuthority)
        {
            return;
        }
        if (chatLog == null)
        {
            chatLog = GameObject.FindWithTag("ChatDisplay").GetComponent<TMP_Text>();
        }

        chatLog.text += $"\n {sendName} : {myChat}";
        myChat = "";
        scrollV.value = 0;

    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RPC_SetChat(string mychat, string _sendName, RpcInfo info = default)
    {

        sendName = _sendName;
        Debug.Log($"[RPC] SetNickname : {mychat}");
        this.myChat = mychat;
    }

    private void OnEnable()
    {
        Debug.Log("chatDown change true ");
        sumit = playerControls.Player.Sumit;
        sumit.Enable();

        sumit.performed += Sumit;
    }
    private void OnDisable()
    {
        sumit.Disable();
    }
    private void Sumit(InputAction.CallbackContext context)
    {
        Debug.Log("chatDown change true ");
        chatDown = true;
    }
}