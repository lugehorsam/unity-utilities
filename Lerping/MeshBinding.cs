﻿using UnityEngine;

namespace Utilities.Meshes
{
	
	public class MeshBinding : Vector3Binding<MeshFilter>
	{
		public Vertex VertexToLerp
		{
			get;
			set;
		}

		public bool AutoWrite { get; set; } = true;
		
		private readonly IProceduralMesh _proceduralMesh;
		private MeshFilter _meshFilter;
		
		public MeshBinding(IProceduralMesh proceduralMesh, MeshFilter meshFilter) : base(meshFilter)
		{
			_proceduralMesh = proceduralMesh;
			_meshFilter = meshFilter;
		}

		public override void SetProperty(Vector3 property)
		{
			VertexToLerp.Set(property);
			if (AutoWrite)
			{
				WriteToMesh();
			}
		}

		public override Vector3 GetProperty()
		{
			return VertexToLerp.AsVector3;
		}

		public void WriteToMesh()
		{
			_meshFilter.mesh = _proceduralMesh.ToUnityMesh();
		}
	}
}
