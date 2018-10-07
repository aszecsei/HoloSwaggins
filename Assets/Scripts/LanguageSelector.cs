using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using TMPro;
using UnityEngine;

public class LanguageSelector : MonoBehaviour, IFocusable, IInputClickHandler
{

	public string Language;
	public PhotoTaker PhotoTaker;
	private TextMeshPro _text;

	void Awake()
	{
		_text = transform.GetComponentInChildren<TextMeshPro>();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnFocusEnter()
	{
		_text.color = Color.yellow;
	}

	public void OnFocusExit()
	{
		_text.color = Color.white;
	}

	public void OnInputClicked(InputClickedEventData eventData)
	{
		PhotoTaker.lang = Language;
	}
}
