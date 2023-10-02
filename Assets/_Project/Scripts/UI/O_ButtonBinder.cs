using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using Jahaha.SceneTransition;

public class O_ButtonBinder : MonoBehaviour,IPointerClickHandler,IPointerEnterHandler,IPointerExitHandler
{
    public enum ButtonType { Start, Exit, Level1, Level2, Level3, Level4, Tutorial, Pause, Continue }
    public Sprite sprite_Deselected;
    public Sprite sprite_Hovering;
    public ButtonType type;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void TriggerButtonEffect()
    {
        switch (type)
        {
            case ButtonType.Start:
                M_SceneTransition.instance.TriggerTransition(1);
                break;
            case ButtonType.Exit:
               Application.Quit();
                break;
            case ButtonType.Level1:
                M_SceneTransition.instance.TriggerTransition(2);
                break;
            case ButtonType.Level2:
                M_SceneTransition.instance.TriggerTransition(3);
                break;
            case ButtonType.Level3:
                M_SceneTransition.instance.TriggerTransition(4);
                break;
            case ButtonType.Level4:
                M_SceneTransition.instance.TriggerTransition(5);
                break;
            case ButtonType.Tutorial:
              
                break;
        }
    }

    public void TriggerOnHovering()
    {
        transform.DOScale(1.15f, 0.3f);
        transform.GetComponent<Image>().sprite = sprite_Hovering;
    }

    public void TriggerOnDeselected()
    {
        transform.DOScale(1f, 0.3f);
        transform.GetComponent<Image>().sprite = sprite_Deselected;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        TriggerButtonEffect();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        TriggerOnHovering();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TriggerOnDeselected();
    }
}
