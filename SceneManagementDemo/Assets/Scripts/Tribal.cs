using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Tribal
{
	public static class TribeControl {

		private const float RAYCAST_DELAY = 0.1f;
		private static float raycastTime = 0f;
		private static bool lastHit = false;
		private static GameObject lastUIHit = null;

		public static System.Random RandomGen = new System.Random(0);

		public static GameObject FindInChildren( Transform t, string searchName )
		{
			GameObject g = null;

			foreach( Transform c in t )
			{
				if( c.name == searchName )
					return c.gameObject;
				else foreach( Transform c1 in c )
					 {
						g = FindInChildren( c1, searchName );
						if( null != g )
							return g;
					 }
							
			}
			return null;
		}

		public static bool RaycastAll( ref RaycastHit hit, ref GameObject UIHit, int layer = 0 )
		{
			raycastTime += Time.deltaTime;

			if( raycastTime > RAYCAST_DELAY )
			{
				bool physCast = Raycast( ref hit, layer, false );

				UIHit = UIRaycast( "UI", false );

				if( null != UIHit && physCast )
					lastHit = true;

				if( null != UIHit )
					lastHit = true;

				if( physCast )
					lastHit = true;

				if( null == UIHit && !physCast )
					lastHit = false;
			}

			return lastHit;
		}
			
		public static bool Raycast( ref RaycastHit hit, int layer = 1, bool limitTime = true )
		{
			if( limitTime ) raycastTime += Time.deltaTime;

			if( (raycastTime > RAYCAST_DELAY) || !limitTime )
			{
				if( limitTime) raycastTime = 0f;

				Vector3 origin, direction;

				Ray r;

				r = new Ray( Camera.main.transform.position, Camera.main.transform.forward );

				origin = r.origin;
				direction = r.direction;

				lastHit = Physics.Raycast( origin, direction, out hit, 200f, layer, QueryTriggerInteraction.Collide );
			}

			return lastHit;
		}

		public static GameObject UIRaycast( string layerName, bool limitTime = false )
		{
			if( limitTime ) raycastTime += Time.deltaTime;

			if( ( raycastTime > RAYCAST_DELAY ) || !limitTime )
			{
				if( limitTime ) raycastTime = 0f;

				PointerEventData ped = new PointerEventData(EventSystem.current);

				#if UNITY_EDITOR
					ped.position = new Vector2( Screen.width / 2, Screen.height / 2 );
				#else
					ped.position = new Vector2( UnityEngine.VR.VRSettings.eyeTextureWidth / 2, UnityEngine.VR.VRSettings.eyeTextureHeight / 2 );
					ped.delta = Vector2.zero;
				#endif
				List<RaycastResult> results = new List<RaycastResult>();
				EventSystem.current.RaycastAll( ped, results );

				if( results.Count > 0 )
					foreach( var res in results )
					{
						if( layerName == "" || res.gameObject.layer == LayerMask.NameToLayer( layerName ) )
						{
							lastUIHit = res.gameObject;
							return lastUIHit;
						}
					}

				lastUIHit = null;
			}

			return lastUIHit;
		}
	}
}
