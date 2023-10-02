using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Jahaha.SceneTransition
{
    [CreateAssetMenu(fileName = "Move Out", menuName = "Jahaha/Scene Transition/Move Out")]
    public class SO_TE_MoveOut : SO_TransitionEffect
    {
        public enum MoveType { LeftToRight, RightToLeft, LeftBack, RightBack, BottomToUp, UpToBottom, BottomBack, UpBack };
        public MoveType thisType;

        public override void ExcuteEffect()
        {
            //Transform targetParent = M_SceneTransition.instance.transform.Find("Canvas");
            //Canvas targetCanvas = targetParent.GetComponent<Canvas>();
            //RectTransform thisRect = Instantiate(prefab, targetParent).GetComponent<RectTransform>();
            InitializeValue();
            float offset_Hori = targetCanvas.pixelRect.width / 2 + thisRect.rect.width/2;
            float offset_Vert = targetCanvas.pixelRect.height / 2 + thisRect.rect.height/2;

            Vector2 pos_Initial = new Vector2();
            Vector2 pos_End = new Vector2();

            switch (thisType)
            {
                case MoveType.LeftToRight:
                    pos_Initial = new Vector2(-offset_Hori, 0);
                    pos_End = new Vector2(offset_Hori, 0);
                    break;
                case MoveType.RightToLeft:
                    pos_Initial = new Vector2(offset_Hori, 0);
                    pos_End = new Vector2(-offset_Hori, 0);
                    break;
                case MoveType.LeftBack:
                    pos_Initial = new Vector2(-offset_Hori, 0);
                    pos_End = new Vector2(-offset_Hori, 0);
                    break;
                case MoveType.RightBack:
                    pos_Initial = new Vector2(offset_Hori, 0);
                    pos_End = new Vector2(offset_Hori, 0);
                    break;
                case MoveType.BottomToUp:
                    pos_Initial = new Vector2(0, -offset_Vert);
                    pos_End = new Vector2(0, offset_Vert);
                    break;
                case MoveType.UpToBottom:
                    pos_Initial = new Vector2(0, offset_Vert);
                    pos_End = new Vector2(0, -offset_Vert);
                    break;
                case MoveType.BottomBack:
                    pos_Initial = new Vector2(0, -offset_Vert);
                    pos_End = new Vector2(0, -offset_Vert);
                    break;
                case MoveType.UpBack:
                    pos_Initial = new Vector2(0, offset_Vert);
                    pos_End = new Vector2(0, offset_Vert);
                    break;
            }

            thisRect.anchoredPosition = pos_Initial;

            Sequence s = DOTween.Sequence();
            s.AppendCallback(() => DOTween.To(() => thisRect.anchoredPosition, x => thisRect.anchoredPosition = x, Vector2.zero, transitionTime/2));
            s.AppendInterval(transitionTime / 2);
            s.AppendCallback(() => M_SceneTransition.instance.TriggerSceneTransition());
            s.AppendCallback(() => DOTween.To(() => thisRect.anchoredPosition, x => thisRect.anchoredPosition = x, pos_End, transitionTime / 2));
            s.AppendInterval(transitionTime / 2);
            s.AppendCallback(() => Destroy(thisRect.gameObject,1));
        }
    }
}
