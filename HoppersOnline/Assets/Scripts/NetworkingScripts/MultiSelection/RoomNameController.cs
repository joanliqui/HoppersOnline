using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class RoomNameController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField] TextMeshProUGUI roomName;
    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    public void CopyRoomName()
    {
        TextEditor editor = new TextEditor();
        editor.text = roomName.text;
        editor.SelectAll();
        editor.Copy();
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        anim.SetTrigger("Highlighted");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        anim.SetTrigger("Normal");
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        anim.SetTrigger("Pressed");
        CopyRoomName();
    }
}
