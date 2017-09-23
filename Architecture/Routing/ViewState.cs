﻿using UnityEngine;

namespace Utilities
{
	using System.Collections;
	using System.Collections.Generic;
	
	public abstract class ViewState : IState
	{
		protected readonly EnumeratorQueue _enter = new EnumeratorQueue();
		protected readonly EnumeratorQueue _exit = new EnumeratorQueue();
		private readonly List<View> _views = new List<View>();
		private readonly Transform _root;
		
		protected ViewState(Transform root) {}

		public IEnumerator Exit()
		{
			Diag.Crumb(this, "Exiting state.");
			
			foreach (var view in _views)
				view.Destroy();

			_exit.Reset();
			return _exit;
		}

		public IEnumerator Enter()
		{
			Diag.Crumb(this, "Entering state.");
			
			_enter.Reset();
			return _enter;
		}

		protected void RegisterView(View view)
		{
			view.Transform.SetParent(_root);
			_views.Add(view);
		}
	}	
}
