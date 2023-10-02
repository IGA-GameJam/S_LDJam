using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System;

namespace Jahaha.SceneTransition
{
    public class M_SceneTransition : MonoBehaviour
    {
        public static M_SceneTransition instance;
        private int targetSceneIndex;

        public SO_TransitionEffect[] effectArray;
        public Action TriggerSceneTransition;

        private void Awake()
        {
            if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); }
            else Destroy(gameObject);
        }

        void Start()
        {
            TriggerSceneTransition = new Action(() => { SceneManager.LoadScene(targetSceneIndex); });
        }


        public void TriggerTransition(int _targetSceneIndex)
        {

            targetSceneIndex = _targetSceneIndex;
            int randomType = UnityEngine.Random.Range(0, effectArray.Length);
            effectArray[randomType].ExcuteEffect();
        }
    }

}
