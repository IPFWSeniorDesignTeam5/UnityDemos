/*! 
 * \file
 * \author Stig Olavsen <stig.olavsen@freakshowstudio.com>
 * \author http://www.freakshowstudio.com
 * \date © 2015
 */

using System;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;
using System.Reflection;
#if UNITY_EDITOR_WIN
using WintabDN;
#endif

namespace FreakLibEditor.TerrainPressurePaint
{
	/// <summary>
	/// Terrain pressure paint implements terrain painting with
	/// pressure sensitivity.
	/// 
	/// Presure sensitivity is set directly in the terrain inspector
	/// by using reflection.
	/// </summary>
	public class TerrainPressurePaint : EditorWindow
	{
		/// <summary>
		/// The current pressure.
		/// </summary>
		private float m_CurrentPressure = 0f;

#if UNITY_EDITOR_WIN
        private int m_WTMaxPressure = 1023;
        private CWintabContext m_WTContext;
        private CWintabData m_WTData;
#endif
	
		/// <summary>
		/// The detail opacity minimum.
		/// </summary>
		private float m_DetailOpacityMin = 0f;
		/// <summary>
		/// The detail opacity maximum.
		/// </summary>
		private float m_DetailOpacityMax = 1f;
		/// <summary>
		/// The detail strength minimum.
		/// </summary>
		private float m_DetailStrengthMin = 0f;
		/// <summary>
		/// The detail strength maximum.
		/// </summary>
		private float m_DetailStrengthMax = 1f;
		/// <summary>
		/// The splat alpha minimum.
		/// </summary>
		private float m_SplatAlphaMin = 0f;
		/// <summary>
		/// The splat alpha maximum.
		/// </summary>
		private float m_SplatAlphaMax = 1f;
		/// <summary>
		/// The strength minimum.
		/// </summary>
		private float m_StrengthMin = 0f;
		/// <summary>
		/// The strength maximum.
		/// </summary>
		private float m_StrengthMax = 1f;
		/// <summary>
		/// The size minimum.
		/// </summary>
		private float m_SizeMin = 1f;
		/// <summary>
		/// The size maximum.
		/// </summary>
		private float m_SizeMax = 100f;
		/// <summary>
		/// The tree brush size minimum.
		/// </summary>
		private float m_TreeSizeMin = 1f;
		/// <summary>
		/// The tree brush size maximum.
		/// </summary>
		private float m_TreeSizeMax = 100f;
		/// <summary>
		/// The tree spacing minimum.
		/// </summary>
		private float m_TreeSpacingMin = 0.3f;
		/// <summary>
		/// The tree spacing maximum.
		/// </summary>
		private float m_TreeSpacingMax = 3f;
	
		/// <summary>
		/// Enable for detail opacity.
		/// </summary>
		private bool m_EnableForDetailOpacity = true;
		/// <summary>
		/// Enable for detail strength.
		/// </summary>
		private bool m_EnableForDetailStrength = false;
		/// <summary>
		/// Enable for splat alpha.
		/// </summary>
		private bool m_EnableForSplatAlpha = true;
		/// <summary>
		/// Enable for strength.
		/// </summary>
		private bool m_EnableForStrength = true;
		/// <summary>
		/// Enable for brush size.
		/// </summary>
		private bool m_EnableForSize = false;
		/// <summary>
		/// Enable for tree brush size.
		/// </summary>
		private bool m_EnableForTreeSize = false;
		/// <summary>
		/// Enable for tree spacing.
		/// </summary>
		private bool m_EnableForTreeSpacing = true;
	
		/// <summary>
		/// A reference to the UnityEditor assembly, used
		/// for reflection.
		/// </summary>
		private Assembly m_EditorAssembly;
		/// <summary>
		/// The type of the terrain inspector.
		/// </summary>
		private Type m_TerrainInspectorType;
		/// <summary>
		/// The type of the tree painter.
		/// </summary>
		private Type m_TreePainterType;
		/// <summary>
		/// The type of UnityEngine.SavedFloat, which the
		/// terrain inspector uses for its variables.
		/// </summary>
		private Type m_SavedFloatType;
		/// <summary>
		/// The type of UnityEngine.SavedInt, which the
		/// terrain inspector uses for its variables.
		/// </summary>
		private Type m_SavedIntType;
		/// <summary>
		/// The saved float property.
		/// </summary>
		private PropertyInfo m_SavedFloatProperty;
		/// <summary>
		/// The saved int property.
		/// </summary>
		private PropertyInfo m_SavedIntProperty;
	
		/// <summary>
		/// Shows the window.
		/// </summary>
		[MenuItem("Window/Terrain Pressure Paint", false, 2015)]
		public static void ShowWindow()
		{
			TerrainPressurePaint wnd = GetWindow<TerrainPressurePaint>();
			wnd.title = "Pressure Paint";
			wnd.Show();
		}
	
		/// <summary>
		/// Override for Unitys OnEnable method.
		/// </summary>
		void OnEnable()
		{
			SceneView.onSceneGUIDelegate += OnSceneGUI;
	
			m_EditorAssembly = typeof(UnityEditor.Editor).Assembly;
	
			m_TerrainInspectorType = 
				m_EditorAssembly.GetType("UnityEditor.TerrainInspector");
			m_TreePainterType =
				m_EditorAssembly.GetType("UnityEditor.TreePainter");
			
			m_SavedFloatType = 
				m_EditorAssembly.GetType("UnityEditor.SavedFloat");
			m_SavedIntType =
				m_EditorAssembly.GetType("UnityEditor.SavedInt");
			
			m_SavedFloatProperty = 
				m_SavedFloatType.GetProperty("value");
			m_SavedIntProperty = 
				m_SavedIntType.GetProperty("value");

#if UNITY_EDITOR_WIN
            try
            {
                CloseContext();
                SetupContext();
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());            
            }
#endif
		}
	
		/// <summary>
		/// Override for Unitys OnDisable method.
		/// </summary>
		void OnDisable()
		{
			SceneView.onSceneGUIDelegate -= OnSceneGUI;
#if UNITY_EDITOR_WIN
            try
            {
                CloseContext();
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
#endif
		}

        void OnDestroy()
        {
            SceneView.onSceneGUIDelegate -= OnSceneGUI;
#if UNITY_EDITOR_WIN
            try
            {
                CloseContext();
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
#endif
        }

#if UNITY_EDITOR_WIN
		/// <summary>
		/// Setups the Wintab context.
		/// </summary>
        private void SetupContext()
        {
            m_WTContext = 
				CWintabInfo.GetDefaultSystemContext(
					ECTXOptionValues.CXO_SYSTEM);

            if (m_WTContext == null)
            {
                Debug.LogError("Unable to get wintab context");
                return;
            }

            m_WTContext.Options |= (uint)ECTXOptionValues.CXO_MESSAGES;
            m_WTContext.Options |= (uint)ECTXOptionValues.CXO_SYSTEM;

            m_WTContext.Open();

            m_WTData = new CWintabData(m_WTContext);

            if (m_WTData == null)
            {
                Debug.LogError("Unable to get wintab data");
                return;
            }

            m_WTMaxPressure = CWintabInfo.GetMaxPressure();
            m_WTData.SetWTPacketEventHandler(WTPacketHandler);        
        }

		/// <summary>
		/// Closes the Wintab context.
		/// </summary>
        private void CloseContext()
        {
            if (m_WTContext != null)
            {
                bool dis = m_WTContext.Enable(false);
                if (dis == false)
                {
                    Debug.LogError("Unable to disable wintab context");
                }
                
                bool res = m_WTContext.Close();
                if (res == false)
                {
                    Debug.LogError("Unable to close wintab context");
                }
                
                m_WTContext = null;
                
                if (m_WTData != null)
                {
                    m_WTData.RemoveWTPacketEventHandler(WTPacketHandler);
                    m_WTData = null;
                }
            }
            MessageEvents.CloseWindow();

        }

		/// <summary>
		/// Callback function for Wintab packets
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="eventArgs">Event arguments.</param>
        public void WTPacketHandler(System.Object sender, 
		                            MessageReceivedEventArgs eventArgs)
        {
            if (m_WTData == null || m_WTContext == null)
            {
                Debug.LogError("Wintab context or data missing");
                return;
            }
            try
            {
                uint pktID = (uint)eventArgs.Message.WParam;
                WintabPacket pkt = 
					m_WTData.GetDataPacket(
						(uint) eventArgs.Message.LParam, pktID);
                if (pkt.pkContext != 0)
                {
                    m_CurrentPressure = 
						(float) pkt.pkTangentPressure / (float) m_WTMaxPressure;
                }
                return;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
#endif
	
		/// <summary>
		/// OnSceneGUI callback, handles reading the pressure
		/// sensitivity, and setting the values in the terrain
		/// inspector.
		/// </summary>
		/// <param name="sv">Unity SceneView.</param>
		void OnSceneGUI(SceneView sv)
		{
			UnityEngine.Object[] terrainInspectorObjects = 
				Resources.FindObjectsOfTypeAll(m_TerrainInspectorType);
			
			if (terrainInspectorObjects == null || 
				terrainInspectorObjects.Length == 0)
			{
				return;
			}
			
			BindingFlags terrainFlags = 
				BindingFlags.Instance | BindingFlags.NonPublic;
			
			FieldInfo detailOpacityField = 
				m_TerrainInspectorType.GetField("m_DetailOpacity", 
				                                terrainFlags);
			FieldInfo detailStrengthField = 
				m_TerrainInspectorType.GetField("m_DetailStrength", 
				                                terrainFlags);
			FieldInfo sizeField = 
				m_TerrainInspectorType.GetField("m_Size", 
				                                terrainFlags);
			FieldInfo splatAlphaField = 
				m_TerrainInspectorType.GetField("m_SplatAlpha", 
				                                terrainFlags);
			FieldInfo strengthField = 
				m_TerrainInspectorType.GetField("m_Strength", 
				                                terrainFlags);
			
			BindingFlags treeFlags = BindingFlags.Public | BindingFlags.Static;
			
			FieldInfo treeSizeField = 
				m_TreePainterType.GetField("brushSize", treeFlags);
			FieldInfo treeSpacingField = 
				m_TreePainterType.GetField("spacing", treeFlags);
	
#if !UNITY_EDITOR_WIN
			if (Event.current.type == EventType.Used)
			{
				m_CurrentPressure = Event.current.pressure;
			}
#endif
			
			if (m_EnableForDetailOpacity || 
				m_EnableForDetailStrength ||
				m_EnableForSplatAlpha ||
				m_EnableForStrength ||
				m_EnableForSize)
			{
				foreach (var ti in terrainInspectorObjects)
				{
					var detailOpacitySV = detailOpacityField.GetValue(ti);
					var detailStrengthSV = detailStrengthField.GetValue(ti);
					var sizeSI = sizeField.GetValue(ti);
					var splatAlphaSV = splatAlphaField.GetValue(ti);
					var strengthSV = strengthField.GetValue(ti);
					
					if (m_EnableForDetailOpacity)
					{
						float range = 
							m_DetailOpacityMax - m_DetailOpacityMin;
						float v = 
							m_DetailOpacityMin + (range * m_CurrentPressure);
						m_SavedFloatProperty.SetValue(detailOpacitySV, 
						                              v, 
						                              null);
					}
					
					if (m_EnableForDetailStrength)
					{
						float range = 
							m_DetailStrengthMax - m_DetailStrengthMin;
						float v = 
							m_DetailStrengthMin + (range * m_CurrentPressure);
						m_SavedFloatProperty.SetValue(detailStrengthSV, 
						                              v, 
						                              null);
					}
					
					if (m_EnableForSplatAlpha)
					{
						float range = 
							m_SplatAlphaMax - m_SplatAlphaMin;
						float v = 
							m_SplatAlphaMin + (range * m_CurrentPressure);
						m_SavedFloatProperty.SetValue(splatAlphaSV, 
						                              v, 
						                              null);
					}
					
					if (m_EnableForStrength)
					{
						float range = 
							m_StrengthMax - m_StrengthMin;
						float v = 
							m_StrengthMin + (range * m_CurrentPressure);
						m_SavedFloatProperty.SetValue(strengthSV, 
						                              v, 
						                              null);
					}
					
					if (m_EnableForSize)
					{
						float range = m_SizeMax - m_SizeMin;
						float v = m_SizeMin + (range * m_CurrentPressure);
						m_SavedIntProperty.SetValue(sizeSI, 
						                            (int)v, 
						                            null);
					}
				}
			}
			
			if (m_EnableForTreeSize)
			{
				float range = m_TreeSizeMax - m_TreeSizeMin;
				float v = m_TreeSizeMin + (range * m_CurrentPressure);
				treeSizeField.SetValue(null, v);
			}
			
			if (m_EnableForTreeSpacing)
			{
				float range = m_TreeSpacingMax - m_TreeSpacingMin;
				float v = m_TreeSpacingMax - (range * m_CurrentPressure);
				treeSpacingField.SetValue(null, v);
			}
			
			Repaint();
		}
	
		/// <summary>
		/// Handles painting of the editor window.
		/// </summary>
		void OnGUI()
		{
#if UNITY_EDITOR_WIN
			if (!CWintabInfo.IsWintabAvailable())
			{
				EditorGUILayout.HelpBox("Wintab driver not available!", 
				                        MessageType.Error);
			}

			if (!CWintabInfo.IsStylusActive())
			{
				EditorGUILayout.HelpBox("No stylus active!", 
				                        MessageType.Error);
			}
#endif

			/*
			 * Raise/Lower 
			 * 		Brush Size = m_Size 
			 * 		Opacity = m_Strength
			 * 
			 * Paint Height
			 * 		Brush Size = m_Size
			 * 		Opacity = m_Strength
			 * 
			 * Smooth Height
			 * 		Brush Size = m_Size
			 * 		Opacity = m_Strength
			 * 
			 * Paint Texture
			 * 		Brush Size = m_Size
			 * 		Opacity = m_Strength
			 * 		Target Strength = m_SplatAlpha
			 * 
			 * Place Trees
			 * 		Brush Size = m_TreeSize
			 * 		Tree Density = m_TreeSpacing
			 * 
			 * Paint Details
			 * 		Brush Size = m_Size
			 * 		Opacity = m_DetailOpacity
			 * 		Target Strength = m_DetailStrength
			 * 
			 */
	
			DrawControl(125f, 25f, "Size", 
			            ref m_EnableForSize, 
			            ref m_SizeMin, 
			            ref m_SizeMax, 
			            1f, 100f);
			
			DrawControl(125f, 25f, "Opacity", 
			            ref m_EnableForStrength, 
			            ref m_StrengthMin, 
			            ref m_StrengthMax, 
			            0f, 1f);
			
			DrawControl(125f, 25f, "Texture Strength", 
			            ref m_EnableForSplatAlpha, 
			            ref m_SplatAlphaMin, 
			            ref m_SplatAlphaMax, 
			            0f, 1f);
			
			DrawControl(125f, 25f, "Detail Opacity", 
			            ref m_EnableForDetailOpacity, 
			            ref m_DetailOpacityMin, 
			            ref m_DetailOpacityMax, 
			            0f, 1f);
	
			DrawControl(125f, 25f, "Detail Strength", 
			            ref m_EnableForDetailStrength, 
			            ref m_DetailStrengthMin, 
			            ref m_DetailStrengthMax, 
			            0f, 1f);
	
			DrawControl(125f, 25f, "Tree Brush Size", 
			            ref m_EnableForTreeSize, 
			            ref m_TreeSizeMin, 
			            ref m_TreeSizeMax, 
			            1f, 100f);
	
			DrawControl(125f, 25f, "Tree Spacing", 
			            ref m_EnableForTreeSpacing, 
			            ref m_TreeSpacingMin, 
			            ref m_TreeSpacingMax, 
			            0.3f, 3f);
	
			DrawPressure();
		}
	
		/// <summary>
		/// Draws a control for a pressure sensitivity input.
		/// </summary>
		/// <param name="toggleWidth">Width of the toggle label.</param>
		/// <param name="numberWidth">Width of the number labels.</param>
		/// <param name="label">Label.</param>
		/// <param name="enabled">Is control enabled.</param>
		/// <param name="minVal">Minimum value.</param>
		/// <param name="maxVal">Maximum value.</param>
		/// <param name="minLimit">Minimum limit.</param>
		/// <param name="maxLimit">Maximum limit.</param>
		private void DrawControl(float toggleWidth,
		                         float numberWidth,
		                         string label,
		                         ref bool enabled,
		                         ref float minVal,
		                         ref float maxVal,
		                         float minLimit,
		                         float maxLimit)
		{
			EditorGUILayout.BeginHorizontal();
			enabled = EditorGUILayout.ToggleLeft(
				label, 
				enabled, 
				GUILayout.MaxWidth(toggleWidth));
			EditorGUILayout.LabelField(minLimit.ToString(), 
			                           GUILayout.MaxWidth(numberWidth));
			EditorGUILayout.MinMaxSlider(
				ref minVal, ref maxVal, minLimit, maxLimit);
			EditorGUILayout.LabelField(maxLimit.ToString(), 
			                           GUILayout.MaxWidth(numberWidth));
			EditorGUILayout.EndHorizontal();
		}
	
		/// <summary>
		/// Draws the pressure indicator in the GUI.
		/// </summary>
		private void DrawPressure()
		{
			GUILayout.Label("Pressure:");
			EditorGUILayout.BeginHorizontal();
			Rect r = EditorGUILayout.GetControlRect(false);
			GUI.color = Color.gray;
			GUI.Box(r, GUIContent.none);
			GUI.color = Color.blue;
			float w = r.width * m_CurrentPressure;
			Rect r2 = new Rect(r.x, r.y, w, r.height);
			GUI.Box(r2, GUIContent.none);
			EditorGUILayout.EndHorizontal();
		}
	}
}
