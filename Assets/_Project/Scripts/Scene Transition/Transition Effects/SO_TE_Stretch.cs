using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.TimeZoneInfo;
using UnityEngine.SceneManagement;

namespace Jahaha.SceneTransition
{
    public class SO_TE_Stretch : SO_TransitionEffect
    {
        public override void ExcuteEffect()
        {
            InitializeValue();
            //Transform trans = canvas.transform.Find("Stretch");
            //bool isHorizontal = _isHorizontal == "Horizontal" ? true : false;
            //trans.localScale = isHorizontal ? new Vector3(0, 1, 1) : new Vector3(1, 0, 1);

            //if (isHorizontal)
            //{
            //    Sequence s = DOTween.Sequence();
            //    s.Append(trans.DOScaleX(1.2f, transitionTime));
            //    s.AppendCallback(() => SceneManager.LoadScene(targetSceneIndex));
            //    s.Append(trans.DOScaleX(0, transitionTime)).OnComplete(() => SetEffectObjStateTo("Stretch", false));

            //else
            //    {
            //        Sequence s = DOTween.Sequence();
            //        s.Append(trans.DOScaleY(1.2f, transitionTime));
            //        s.AppendCallback(() => SceneManager.LoadScene(targetSceneIndex));
            //        s.Append(trans.DOScaleY(0, transitionTime)).OnComplete(() => SetEffectObjStateTo("Stretch", false));
            //    }
            //}
        }
    }
}

