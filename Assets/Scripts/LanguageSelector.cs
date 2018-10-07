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

    private static LanguageSelector _selectedLanguage;

	void Awake()
	{
		_text = transform.GetComponentInChildren<TextMeshPro>();
        if (Language.Equals(""))
        {
            _selectedLanguage = this;
        }
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnFocusEnter()
	{
		_text.color = _selectedLanguage == this ? Color.green : Color.yellow;
	}

	public void OnFocusExit()
	{
		_text.color = _selectedLanguage == this ? Color.green : Color.white;
	}

	public void OnInputClicked(InputClickedEventData eventData)
	{
		PhotoTaker.lang = Language;
        _selectedLanguage._text.color = Color.white;
        _selectedLanguage = this;
        _text.color = Color.green;
	}
}
