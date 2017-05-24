using UnityEngine;
using System.Collections;
[ExecuteInEditMode]

public class DisplayTextureFullScreen : MonoBehaviour 
{
public _GUIClasses.TextureGUI graphic = new _GUIClasses.TextureGUI(); //(28,23);
public Color GUIColor;

void OnGUI() 
{
	GUI.color = GUIColor;
	if (graphic.texture) 
	{
		GUI.DrawTexture(new Rect(graphic.offset.x,graphic.offset.y, Screen.width,Screen.height), graphic.texture,ScaleMode.StretchToFill,true);
	}
}

void AlphaUp(float change) 
{
	GUIColor.a += change;
}

void setStartColor(Color color) 
{
	GUIColor = color;
}


void setDelay(float delay)
{
	if (GUIColor.a > .5) 
	{
		GUIColor.a += delay;
	} 
	else 
	{
		GUIColor.a -= delay;
	}
}

void AlphaDown(float change) 
{
	GUIColor.a -= change;
}
	
}
