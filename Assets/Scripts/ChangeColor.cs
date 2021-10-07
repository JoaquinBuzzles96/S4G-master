/*  ------ S4Game Consortium --------------
*  This library is free software; you can redistribute it and/or
*  modify it under the terms of the GNU Lesser General Public
*  License as published by the Free Software Foundation; either
*  version 3 of the License, or (at your option) any later version.
*  This library is distributed in the hope that it will be useful,
*  but WITHOUT ANY WARRANTY; without even the implied warranty of
*  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
*  Lesser General Public License for more details.
*  You should have received a copy of the GNU Lesser General Public
*  License along with this library; if not, write to the Free Software
*  CCMIJU, Carretera Nacional 521, Km 41.8 – 1007, Cáceres, Spain*/
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChangeColor : MonoBehaviour, IPointerClickHandler
{
	void OnEnable ()
	{
	}

	public void SetRed(float value)
	{
		OnValueChanged(value, 0);
	}
	
	public void SetGreen(float value)
	{
		OnValueChanged(value, 1);
	}
	
	public void SetBlue(float value)
	{
		OnValueChanged(value, 2);
	}
	
	public void OnValueChanged(float value, int channel)
	{
		Color c = Color.white;

		if (GetComponent<Renderer>() != null)
			c = GetComponent<Renderer>().material.color;
		else if (GetComponent<Light>() != null)
			c = GetComponent<Light>().color;
		
		c[channel] = value;

		if (GetComponent<Renderer>() != null)
			GetComponent<Renderer>().material.color = c;
		else if (GetComponent<Light>() != null)
			GetComponent<Light>().color = c;
	}

	public void OnPointerClick(PointerEventData data)
	{
		if (GetComponent<Renderer>() != null)
			GetComponent<Renderer>().material.color = new Color(Random.value, Random.value, Random.value, 1.0f);
		else if (GetComponent<Light>() != null)
			GetComponent<Light>().color = new Color(Random.value, Random.value, Random.value, 1.0f);
	}
}
