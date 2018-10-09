using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class texto : MonoBehaviour {
	public string s1;
	public Text caja;
	public float time;
	public static bool fin = false;

	// Use this for initialization
	void Start () {
		fin = false;
		StartCoroutine (poniendoTexto(s1));
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	IEnumerator poniendoTexto(String contenido){
		string llenar = "";
		for (int i = 0; i < contenido.Length; i++) {
			if (fin) {
				break;
			}
			llenar += contenido [i];
			caja.text = llenar;
			yield return new WaitForSeconds (time*0.5f);
		}
		yield return new WaitForSeconds (5);
	}

	public void pulsar(){
		fin = true;
	}
}
