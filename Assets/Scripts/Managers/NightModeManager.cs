using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MySpace
{ 
    public class NightModeManager : MonoBehaviour
    {
        public static NightModeManager Ref { get; private set; }

        public static event System.Action NightModeStartEvent;

        [SerializeField] GameObject NightModeUI;

        #region MonoMethods

        protected void OnEnable()
        {
            if (!Ref)
                Ref = this;
            else
            {
                Destroy(gameObject);
            }
        }

        protected void OnDisable()
        {
            if (Ref == this)
                Ref = null;
        }

        protected void Start()
        {
            NightModeUI.SetActive(false);
        }

        #endregion


        public void EnableNightMode()
        {
            //NightModeUI.SetActive(true);

            //This is where the tarot card animation/event will be called and the fuctionality will start. Might have to enable a translucent panel
            //to disable all UI functionality for this in case the player tries to click things during the animation.
            //NightModeStartEvent?.Invoke();

            //Might want to change the behavior of guests here? The best way would probably be with a "night mode" bool, that way guests can retain 
            //functionality in the same states they currently use, and those states can just have "night mode" functionality added to them.
            
        }

    }
}