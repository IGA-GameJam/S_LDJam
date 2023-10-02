using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jahaha.SceneTransition
{
    [CreateAssetMenu(fileName = "New Fade", menuName = "Jahaha/Scene Transition/Fade")]
    public class SO_TE_Fade : SO_TransitionEffect
    {
        public override void ExcuteEffect()
        {
            InitializeValue();
            CanvasGroup group = thisRect.GetComponent<CanvasGroup>();
            group.alpha = 0;

            Sequence s = DOTween.Sequence();
            s.AppendCallback(() => DOTween.To(() => group.alpha, x => group.alpha = x, 1, transitionTime/2));
            s.AppendInterval(transitionTime/2);
            s.AppendCallback(() => M_SceneTransition.instance.TriggerSceneTransition());
            s.AppendCallback(() => DOTween.To(() => group.alpha, x => group.alpha = x, 0, transitionTime/2));
            s.AppendInterval(transitionTime / 2);
            s.AppendCallback(() => Destroy(thisRect.gameObject, 1));
        }
    }
}