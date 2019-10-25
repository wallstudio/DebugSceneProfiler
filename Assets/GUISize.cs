using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GUISize : MonoBehaviour
{
	Rect windowRect;

	bool toggle;
	float slider;
	float size = 0.1f;

	void Start()
	{
		const int windowMergin = 30;
		windowRect = new Rect(
			x: windowMergin,
			y: windowMergin,
			width: Screen.width - windowMergin * 2,
			height: Screen.height - windowMergin * 2);
	}

	void OnGUI()
	{
		windowRect = GUILayout.Window(0, windowRect, _ =>
		{
			toggle = GUILayout.Toggle(toggle, "Toggle", GUILayout.Width(size), GUILayout.Height(size));
			slider = GUILayout.HorizontalSlider(
				slider, 0, 1, new GUIStyle()
				{
					fixedWidth = size * 1000,
					fixedHeight = size * 100,
				}, new GUIStyle());
			size = GUILayout.HorizontalSlider(
				size, 0, 10, GUILayout.Width(100), GUILayout.Height(10));
			GUILayout.Label(size.ToString());

			GUI.DragWindow();
		}, "Window");

	}
}
