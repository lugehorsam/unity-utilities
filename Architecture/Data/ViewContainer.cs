﻿using UnityEngine;
using Utilities.Observable;

namespace Utilities
{
	public class ViewContainer<T> : View<Observables<T>> {

		public ViewContainer()
		{
			Data = new Observables<T>();
		}

		public ViewContainer(Transform parent) : base(parent)
		{
			Data = new Observables<T>();
		}

		protected sealed override void HandleDatumChanged(Observables<T> oldData, Observables<T> newData)
		{
			if (oldData != null)
			{
				oldData.OnAfterItemAdd -= HandleItemAdd;
				oldData.OnAfterItemRemove -= HandleItemRemove;
				foreach (var item in oldData)
				{
					HandleItemRemove(item);
				}
			}
            
			if (newData != null)
			{
				newData.OnAfterItemAdd += HandleItemAdd;
				newData.OnAfterItemRemove += HandleItemRemove;
				foreach (var item in newData)
				{
					HandleItemAdd(item);
				}
			}            
		}
		
		protected virtual void HandleItemAdd(T item)
		{
		}

		protected virtual void HandleItemRemove(T item)
		{
		}
	}	
}