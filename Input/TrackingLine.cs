﻿using UnityEngine;
using Utilities.Input;

namespace Utilities
{
	public class TrackingLine : View
	{
		public LineBinding LineBinding => _lineBinding;

		private LineBinding _lineBinding;
		
		private TouchDispatcher _touchDispatcher;
		private Camera _camera;
		private float _z;
		
		public TrackingLine(Camera camera, TouchDispatcher dispatcher, float z)
		{
			_camera = camera;
			_lineBinding = new LineBinding(GameObject, GameObject.AddComponent<LineRenderer>());
			_z = z;
			_touchDispatcher = dispatcher;
			_touchDispatcher.OnDrag += HandleDrag;
			_touchDispatcher.OnRelease += HandleRelease;

		}

		void HandleDrag(TouchEventInfo eventInfo)
		{
			_lineBinding.SetProperty(_camera.ScreenToWorldPoint(UnityEngine.Input.mousePosition).SetZ(_z));
		}

		void HandleRelease(TouchEventInfo eventInfo)
		{
			_lineBinding.SetProperty(_lineBinding.GetProperty(0));
		}
	}
}