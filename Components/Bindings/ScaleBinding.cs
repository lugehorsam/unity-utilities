﻿using UnityEngine;

namespace Utilities
{
    [System.Serializable]
    public class ScaleBinding : Vector3Binding<Transform> {
  
        protected ScaleBinding(MonoBehaviour coroutineRunner, GameObject gameObject) : base(coroutineRunner, gameObject, gameObject.transform)
        {        
        }
    
        public override Vector3 GetProperty ()
        {
            return GameObject.transform.localScale;
        }

        public override void SetProperty (Vector3 property)
        {
            GameObject.transform.localScale = property;
        }
    }   
}
