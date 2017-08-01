﻿using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Input;

namespace Utilities
{
    public abstract class Layout<T, TList> : View<TList> where TList : ObservableCollection<T> where T : ILayoutMember
    {
        public T this[int i]
        {
            get { return Data[i]; }
            set { Data[i] = value; }
        }

        protected Layout(TList list)
        {
            Data = list;
        }
        
        public IEnumerable<TouchDispatcher> GetTouchDispatchers()
        {
            var iTouchDispatchers = Data.OfType<ITouchDispatcher>();
            return iTouchDispatchers.Select(dispatcher => dispatcher.TouchDispatcher);
        }
        
        public void DoLayout()
        {
            for (int i = 0; i < Data.Count; i++)
            {
                T behavior = Data[i];

                if (behavior == null)
                    continue;
                                
                behavior.GameObject.transform.SetParent(GameObject.transform);
                behavior.GameObject.transform.SetSiblingIndex(i);
                behavior.OnLocalLayout(GetIdealLocalPosition(behavior));
            }
        }

        protected sealed override void HandleDatumChanged(TList oldData, TList newData)
        {           
            if (oldData != null)
            {
                oldData.OnAfterItemAdd -= HandleOnAdd;
                oldData.OnAfterItemRemove -= HandleOnRemove;
            }
            
            if (newData != null)
            {
                newData.OnAfterItemAdd += HandleOnAdd;
                newData.OnAfterItemRemove += HandleOnRemove;
            }
            
            DoLayout();
            HandleLayoutDatumChanged(oldData, newData);
        }

        protected virtual void HandleLayoutDatumChanged(TList oldData, TList newData)
        {
            
        }
        
        void HandleOnAdd(T datum)
        {
            datum.RectTransform.SetSiblingIndex(Data.IndexOf(datum));
            DoLayout();
        }
        
        void HandleOnRemove(T datum)
        {
            DoLayout();
        }

        protected abstract Vector2 GetIdealLocalPosition(T element);
    }      
}