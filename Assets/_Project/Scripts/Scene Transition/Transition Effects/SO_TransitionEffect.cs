using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jahaha.SceneTransition
{
    public abstract class SO_TransitionEffect : ScriptableObject
    {
        public float transitionTime;
        public GameObject prefab;
        protected Transform targetParent;
        protected Canvas targetCanvas;
        protected RectTransform thisRect;

        public abstract void ExcuteEffect();

        protected void InitializeValue()
        {
            targetParent = M_SceneTransition.instance.transform.Find("Canvas");
            targetCanvas = targetParent.GetComponent<Canvas>();
            thisRect = Instantiate(prefab, targetParent).GetComponent<RectTransform>();
        }
    }
}

