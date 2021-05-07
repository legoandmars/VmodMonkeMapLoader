﻿using UnityEngine;

namespace VmodMonkeMapLoader.Behaviours
{
    [System.Serializable]
    public class ObjectTrigger : GorillaMapTriggerBase
    {
        public GameObject ObjectToTrigger;
        public bool DisableObject = false;
        public bool OnlyTriggerOnce = false;

#if PLUGIN

        private bool _triggered = false;
        void Start()
        {
            if (!DisableObject) ObjectToTrigger.SetActive(false);
            else ObjectToTrigger.SetActive(true);
        }

        void OnEnable()
        {
            if (!DisableObject) ObjectToTrigger.SetActive(false);
            else ObjectToTrigger.SetActive(true);
            
            _triggered = false;
        }

        public override void Trigger(Collider collider)
        {
            if (_triggered && OnlyTriggerOnce)
                return;

            ObjectToTrigger.SetActive(DisableObject);
            ObjectToTrigger.SetActive(!DisableObject);

            _triggered = true;

            base.Trigger(collider);
        }

#endif

    }
}
